using EbnfParserGenerator.Ebnf;
using EbnfParserGenerator.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace EbnfParserGenerator
{
    [Generator]
    public class ParserGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            foreach (var file in context.AdditionalFiles)
            {
                if (file.Path.EndsWith(".grammar"))
                {
                    var fileContent = File.ReadAllText(file.Path);
                    var (Tree, Messages) = Parser.Parse(fileContent);

                    if (Messages.Any())
                    {
                        foreach (var msg in Messages)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    new DiagnosticDescriptor("EBNF1", "Parse Error", msg.Text, "Parsing", DiagnosticSeverity.Error, true),
                                    Location.Create(file.Path, new TextSpan(msg.Line, 1), new LinePositionSpan(new LinePosition(msg.Line - 1, msg.Column), new LinePosition(msg.Line, msg.Column)))));
                        }
                    }
                    else
                    {
                        context.AddSource("IVisitor.g.cs", new IVisitorGeneratorVisitor().Text(Tree));
                        context.AddSource("TokenType.g.cs", new TokenTypeEnumVisitor().Text(Tree));
                        context.AddSource("Nodes.g.cs", new NodeGeneratorVisitor().Text(Tree));
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}