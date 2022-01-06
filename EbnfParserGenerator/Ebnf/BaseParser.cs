﻿namespace EbnfParserGenerator.Ebnf;

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

    protected Token Current => Peek(0);

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

    protected Token Match(TokenType kind)
    {
        if (Current.Type == kind)
            return NextToken();

        Messages.Add(Message.Error($"Expected {kind} but got {Current.Type}", Current.Line, Current.Column));

        return Token.Invalid;
    }

    protected Token NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    protected Token Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Count)
            return _tokens.Last();

        return _tokens[index];
    }

    protected Token Previous()
    {
        if (_position >= _tokens.Count) return _tokens[_tokens.Count - 1];

        return _tokens[_position - 1];
    }

    protected abstract TNode Start();
}