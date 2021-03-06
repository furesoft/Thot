using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using System.Text;
using Thot.Ebnf.AST;
using Thot.Ebnf;
using Thot.Visitors;

namespace Thot;

[Generator]
public class ParserGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        bool hasError = false;

        foreach (var file in context.AdditionalFiles)
        {
            if (file.Path.EndsWith(".grammar"))
            {
                var (Tree, Messages) = Parser.Parse(new SourceDocument(file.Path));

                if (Tree != null)
                {
                    if (Messages.Any())
                    {
                        ReportMessages(context, file, Messages);
                        hasError = true;
                    }
                    else
                    {
                        context.AddSource("IVisitor.g.cs", new IVisitorGeneratorVisitor().Text(Tree));
                        context.AddSource("Nodes.g.cs", new NodeGeneratorVisitor().Text(Tree));
                        context.AddSource("PrintVisitor.g.cs", new PrintVisitorGeneratorVisitor().Text(Tree));

                        context.AddSource("TokenType.g.cs", new TokenTypeEnumVisitor().Text(Tree));
                        context.AddSource("Lexer.g.cs", new LexerGeneratorVisitor().Text(Tree));

                        var ruleAnalysisVisitor = new RuleAnalysisVisitor();
                        var analasysResult = Tree.Accept(ruleAnalysisVisitor);
                        if (!ruleAnalysisVisitor.HasNoGrammarBlock && analasysResult)
                        {
                            ReportMessages(context, file, ruleAnalysisVisitor.Messages);
                        }
                        else if (!ruleAnalysisVisitor.HasNoGrammarBlock && !analasysResult)
                        {
                            GenerateParser(context, Tree, file);
                        }
                    }
                }
            }
        }

        if (context.AdditionalFiles.Any() && !hasError)
        {
            context.AddSource("Message.g.cs", LoadFromResource<Message>("Parsing"));
            context.AddSource("Token.g.cs", LoadFromResource<Token>("Parsing"));
            context.AddSource("BaseLexer.g.cs", LoadFromResource<BaseLexer>("Parsing"));
            context.AddSource("SourceDocument.g.cs", LoadFromResource<SourceDocument>("Parsing"));
            context.AddSource("BaseParser.g.cs", LoadFromResource<BaseParser<ASTNode, Lexer, Parser>>("Parsing"));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    private static void GenerateParser(GeneratorExecutionContext context, ASTNode? Tree, AdditionalText file)
    {
        var parserGeneratorVisitor = new ParserGeneratorVisitor();

        Tree?.Accept(parserGeneratorVisitor);

        if (parserGeneratorVisitor.HasStartRule)
        {
            context.AddSource("Parser.g.cs", parserGeneratorVisitor.Text(Tree));
        }
        else
        {
            ReportError(context, file, Message.Error("A grammar has to define a 'start' rule", 1, 1));
        }
    }

    private static SourceText LoadFromResource<T>(string newNamespace)
    {
        Type type = typeof(T);

        var text = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(type.Namespace + $".{type.Name.Replace("`3", "")}.cs")).ReadToEnd();

        text = text.Replace($"namespace {type.Namespace};", $"namespace {newNamespace};");

        return SourceText.From(text, Encoding.ASCII);
    }

    private static void ReportError(GeneratorExecutionContext context, AdditionalText file, Message msg)
    {
        context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor("EBNF1", "Parse Error", msg.Text, "Parsing", DiagnosticSeverity.Error, true),
                Location.Create(file.Path, new TextSpan(msg.Line, 1), new LinePositionSpan(new LinePosition(msg.Line - 1, msg.Column), new LinePosition(msg.Line, msg.Column)))));
    }

    private static void ReportMessages(GeneratorExecutionContext context, AdditionalText file, List<Message> Messages)
    {
        foreach (var msg in Messages)
        {
            ReportError(context, file, msg);
        }
    }
}
