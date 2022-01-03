namespace EbnfParserGenerator.Ebnf;

public abstract class BaseParser<TNode, TLexer, TParser>
    where TParser : BaseParser<TNode, TLexer, TParser>
    where TLexer : BaseLexer, new()
{
    public readonly List<Message> Messages;
    protected int _position = 0;
    private readonly List<Token> _tokens;

    public BaseParser(List<Token> tokens, List<Message> messages)
    {
        _tokens = tokens;
        this.Messages = messages;
    }

    public static (TNode? Tree, List<Message> Messages) Parse(string? src)
    {
        if (string.IsNullOrEmpty(src) || src == null)
        {
            return (default, new() { Message.Error("Empty File", 0, 0) });
        }

        var lexer = new TLexer();
        var tokens = lexer.Tokenize(src);

        var parser = (TParser)Activator.CreateInstance(typeof(TParser), tokens, lexer.Messages);

        return (parser.Program(), parser.Messages);
    }

    public TNode Program()
    {
        var node = Start();

        Match(TokenType.EOF);

        return node;
    }

    protected Token Consume()
    {
        var token = Peek(0);

        _position++;

        return token;
    }

    protected Token Expect(TokenType type)
    {
        Match(type, 0);

        return Consume();
    }

    protected void ExpectKeyword(string name)
    {
        Token token = Peek();

        Consume();

        if (!token.Text.Equals(name))
        {
            Messages.Add(Message.Error($"Expected {name} but got {token.Text}", token.Line, token.Column));
        }
    }

    protected bool Match(TokenType type, int offset = 1)
    {
        Token token = Peek(offset);
        var cond = token.Type == type;

        if (!cond)
        {
            Messages.Add(Message.Error($"Expected {type} but got {token.Type}", token.Line, token.Column));
        }

        return cond;
    }

    protected bool Match(TokenType type, out Token token)
    {
        token = Peek();

        var cond = token.Type == type;

        if (!cond)
        {
            Messages.Add(Message.Error($"Expected {type} but got {token.Type}", token.Line, token.Column));
        }

        return cond;
    }

    protected Token Peek(int offset = 1)
    {
        if (_position + offset >= _tokens.Count)
        {
            return new Token(TokenType.EOF);
        }

        return _tokens[_position + offset];
    }

    protected bool PeekMatch(TokenType type, int offset = 1)
    {
        if (_position >= _tokens.Count) return false;

        return _tokens[_position - offset].Type == type;
    }

    protected Token Previous()
    {
        if (_position >= _tokens.Count) return _tokens[_tokens.Count - 1];

        return _tokens[_position - 1];
    }

    protected abstract TNode Start();
}