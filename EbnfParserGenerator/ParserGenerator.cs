using EbnfParserGenerator.Ebnf;
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
                    var result = Parser.Parse(fileContent);

                    if (result.Messages.Any())
                    {
                        foreach (var msg in result.Messages)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    new DiagnosticDescriptor("EBNF1", "Parse Error", msg.Text, "Parsing", DiagnosticSeverity.Error, true),
                                    Location.Create(file.Path, new TextSpan(msg.Line, 1), new LinePositionSpan(new LinePosition(msg.Line - 1, msg.Column), new LinePosition(msg.Line, msg.Column)))));
                        }
                    }
                    else
                    {
                        context.AddSource(Path.GetFileName(file.Path) + ".g", SourceText.From($"Console.WriteLine({result.Tree})"));
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}