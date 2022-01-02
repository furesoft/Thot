namespace EbnfParserGenerator.Ebnf;

public static class TokenUtils
{
    public static TokenType GetTokenType(string name)
    {
        return name switch
        {
            "token" => TokenType.TokenKeyword,
            "type" => TokenType.TypeKeyword,
            "grammar" => TokenType.GrammarKeyword,
            _ => TokenType.Identifier,
        };
    }
}