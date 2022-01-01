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

            foreach (var token in tokens)
            {
                Console.WriteLine(token.Type + ": " + token.Text);
            }

            var parser = new Parser(tokens);
            var ast = parser.Program();

            Console.WriteLine(ast);
        }
    }
}