using EbnfParserGenerator;
namespace EbnfParserGenerator.Ebnf
{
    public enum TokenType
    {
        Invalid,
        EOF,
        Identifier,
        StringLiteral,
        Plus,
        Star,
        Question,
        OpenParen,
        CloseParen,
        GoesTo,
        Pipe,
        Semicolon,
        Exclamation,
        CloseSquare,
        OpenSquare,
        Minus,
        Number,
        At,
        Colon,
    }
}
