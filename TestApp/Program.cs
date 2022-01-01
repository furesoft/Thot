using EbnfParserGenerator.Ebnf;

namespace TestApp;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            var input = Console.ReadLine();

            var result = Parser.Parse(input);

            if (result.Messages.Any())
            {
                foreach (var msg in result.Messages)
                {
                    Console.WriteLine(msg);
                }
            }

            Console.WriteLine(result.Tree);
        }
    }
}