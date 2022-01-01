using EbnfParserGenerator.Ebnf;

namespace TestApp;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            var lexer = new Lexer();
            var input = Console.ReadLine();

            var tokens = lexer.Tokenize(input);

            if (lexer.Messages.Any())
            {
                foreach (var msg in lexer.Messages)
                {
                    Console.WriteLine(msg);
                }
            }

            var parser = new Parser(tokens);
            var ast = parser.Program();

            Console.WriteLine(ast);
        }
    }
}