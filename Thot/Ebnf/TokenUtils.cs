namespace Thot.Ebnf;

public static class TokenUtils
{
    public static TokenType GetTokenType(string name)
    {
        return name switch
        {
            "token" => TokenType.TokenKeyword,
            "type" => TokenType.TypeKeyword,
            "grammar" => TokenType.GrammarKeyword,
            "for" => TokenType.For,
            _ => TokenType.Identifier,
        };
    }
}
