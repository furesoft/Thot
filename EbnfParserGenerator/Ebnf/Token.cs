using EbnfParserGenerator;
using EbnfParserGenerator.Ebnf;
namespace EbnfParserGenerator.Ebnf
{
    public class Token
    {
        public static Token? Invalid = new Token(TokenType.Invalid);

        public Token(TokenType type, string text, int start, int end, int line)
        {
            Type = type;
            Text = text;
            Start = start;
            End = end;
            Line = line;
        }

        public Token(TokenType type)
        {
            Type = type;
        }

        public int End { get; set; }
        public int Line { get; set; }

        public int Start { get; set; }
        public string Text { get; set; }

        public TokenType Type { get; set; }
    }
}
