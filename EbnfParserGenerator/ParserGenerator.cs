﻿using EbnfParserGenerator.Ebnf;
using EbnfParserGenerator.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using System.Text;

namespace EbnfParserGenerator;

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
                    //context.AddSource("Lexer.g.cs", new LexerGeneratorVisitor().Text(Tree));
                    context.AddSource("Message.g.cs", LoadFromResource<Message>("Parsing"));
                    context.AddSource("Token.g.cs", LoadFromResource<Token>("Parsing"));
                }
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    private static SourceText LoadFromResource<T>(string newNamespace)
    {
        Type type = typeof(T);

        string text = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(type.Namespace + $".{typeof(T).Name}.cs")).ReadToEnd();

        text = text.Replace($"namespace {type.Namespace};", $"namespace {newNamespace};");

        return SourceText.From(text, Encoding.ASCII);
    }
}