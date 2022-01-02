using EbnfParserGenerator.Ebnf;

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

            Console.WriteLine(Tree.Accept(new EbnfParserGenerator.Visitors.IVisitorGeneratorVisitor()));
            Console.WriteLine(Tree.Accept(new EbnfParserGenerator.Visitors.TokenTypeEnumVisitor()));
            Console.WriteLine(Tree.Accept(new EbnfParserGenerator.Visitors.NodeGeneratorVisitor()));
        }
    }
}