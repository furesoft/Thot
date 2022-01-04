using EbnfParserGenerator.Ebnf;
using EbnfParserGenerator.Visitors;

namespace TestApp;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            var input = Console.ReadLine();

            var (Tree, Messages) = Parser.Parse(input);

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

            var ruleAnalysisVisitor = new RuleAnalysisVisitor();
            if (Tree.Accept(ruleAnalysisVisitor))
            {
                foreach (var msg in ruleAnalysisVisitor.Messages)
                {
                    Console.WriteLine(msg);
                }
            }
            else
            {
                Console.WriteLine(Tree?.Accept(new ParserGeneratorVisitor()));
            }
        }
    }
}