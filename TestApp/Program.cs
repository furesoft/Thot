using EbnfParserGenerator.Ebnf;
using EbnfParserGenerator.Visitors;

namespace TestApp;

public class Program
{
    public static void Main()
    {
        var (Tree, Messages) = Parser.Parse(new SourceDocument("test.grammar"));

        if (Messages.Any())
        {
            foreach (var msg in Messages)
            {
                Console.WriteLine(msg);
            }
        }

        Console.WriteLine(Tree?.Accept(new IVisitorGeneratorVisitor()));
        Console.WriteLine(Tree?.Accept(new TokenTypeEnumVisitor()));
        Console.WriteLine(Tree?.Accept(new NodeGeneratorVisitor()));
        Console.WriteLine(Tree?.Accept(new LexerGeneratorVisitor()));
        Console.WriteLine(Tree?.Accept(new PrintVisitorGeneratorVisitor()));

        var ruleAnalysisVisitor = new RuleAnalysisVisitor();
        var v = Tree?.Accept(ruleAnalysisVisitor);
        if (v.HasValue && v.Value)
        {
            foreach (var msg in ruleAnalysisVisitor.Messages)
            {
                Console.WriteLine(msg);
            }
        }
        else if (!ruleAnalysisVisitor.HasNoGrammarBlock)
        {
            Console.WriteLine(Tree?.Accept(new ParserGeneratorVisitor()));
        }
    }
}