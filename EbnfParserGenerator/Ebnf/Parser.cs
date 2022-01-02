﻿using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf;

public class Parser
{
    public readonly List<Message> Messages;
    private readonly List<Token> _tokens;
    private int _position = 0;

    public Parser(List<Token> tokens, List<Message> messages)
    {
        _tokens = tokens;
        this.Messages = messages;
    }

    public static (ASTNode Tree, List<Message> Messages) Parse(string? src)
    {
        if (string.IsNullOrEmpty(src))
        {
            return (new InvalidNode(), new() { Message.Error("Empty File", 0, 0) });
        }

        var lexer = new Lexer();
        var tokens = lexer.Tokenize(src);

        var parser = new Parser(tokens, lexer.Messages);

        return (parser.Program(), parser.Messages);
    }

    public ASTNode Program()
    {
        var node = ProgramUnits();

        Match(TokenType.EOF);

        return node;
    }

    private Token Consume()
    {
        var token = Peek(0);

        _position++;

        return token;
    }

    private Token Expect(TokenType type)
    {
        Match(type, 0);

        return Consume();
    }

    private void ExpectKeyword(string name)
    {
        Token token = Peek();

        Consume();

        if (!token.Text.Equals(name))
        {
            Messages.Add(Message.Error($"Expected {name} but got {token.Text}", token.Line, token.Column));
        }
    }

    private bool Match(TokenType type, int offset = 1)
    {
        Token token = Peek(offset);
        var cond = token.Type == type;

        if (!cond)
        {
            Messages.Add(Message.Error($"Expected {type} but got {token.Type}", token.Line, token.Column));
        }

        return cond;
    }

    private bool Match(TokenType type, out Token token)
    {
        token = Peek();

        var cond = token.Type == type;

        if (!cond)
        {
            Messages.Add(Message.Error($"Expected {type} but got {token.Type}", token.Line, token.Column));
        }

        return cond;
    }

    private Expr ParseExpression()
    {
        var expr = ParseUnary();

        while (PeekMatch(TokenType.Pipe))
        {
            var right = ParseExpression();

            expr = new AlternateNode(expr, right);
        }

        return expr;
    }

    private Expr ParseGroup()
    {
        var expr = ParseExpression();

        Expect(TokenType.CloseParen);

        return new AST.Expressions.GroupExpr(expr);
    }

    private Expr ParseNameExpr()
    {
        return new AST.Expressions.NameExpression(Previous().Text);
    }

    private Expr ParsePrimary()
    {
        var token = Consume();

        if (token.Type == TokenType.StringLiteral)
        {
            return ParseStringLiteral();
        }
        else if (token.Type == TokenType.OpenParen)
        {
            return ParseGroup();
        }
        else if (token.Type == TokenType.OpenSquare)
        {
            return ParseRange();
        }
        else if (token.Type == TokenType.Identifier)
        {
            return ParseNameExpr();
        }
        else
        {
            Messages.Add(Message.Error($"Unknown Expression {token.Text}", token.Line, token.Column));
        }

        return new InvalidExpr();
    }

    //[_"k]
    //[a-z]
    //[a-Z0-9]
    //[_a-z0-9]
    private Expr ParseRange() //ToDo: implement Range Expression
    {
        var expr = ParseRangeExpression();

        Expect(TokenType.CloseParen);

        return new AST.Expressions.CharacterClassExpression();
    }

    private Expr ParseRangeExpression()
    {
        throw new NotImplementedException();
    }

    private ASTNode ParseRule()
    {
        var nameToken = Expect(TokenType.Identifier);

        Expect(TokenType.GoesTo);

        var expr = ParseExpression();

        return new RuleNode(nameToken.Text, new List<Expr> { expr });
    }

    private Expr ParseStringLiteral()
    {
        var token = Previous();

        return new AST.Expressions.LiteralNode(token.Text);
    }

    private ASTNode ParseSubTypeSpec()
    {
        // | typename(arg : type,...)
        Expect(TokenType.Pipe);

        if (Previous().Type != TokenType.Pipe) return new InvalidNode();

        var typename = Expect(TokenType.Identifier);

        var properties = new List<(string name, string type)>();

        Expect(TokenType.OpenParen);

        while (Peek().Type != TokenType.Semicolon)
        {
            var name = Expect(TokenType.Identifier);
            Expect(TokenType.Colon);
            var type = Expect(TokenType.Identifier);

            properties.Add((name.Text, type.Text));

            if (Peek(0).Type == TokenType.CloseParen)
            {
                break;
            }
            else
            {
                Expect(TokenType.Comma);
            }
        }

        Expect(TokenType.CloseParen);

        return new SubTypeDeclaration(typename.Text, properties);
    }

    private ASTNode ParseTokenSpec()
    {
        var token = Consume();

        if (token.Type == TokenType.StringLiteral)
        {
            return ParseTokenSymbolSpec();
        }
        else
        {
            return ParseTokenSpecDefinition();
        }
    }

    private ASTNode ParseTokenSpecDefinition()
    {
        _position--;

        var node = new TokenSpecNode((RuleNode)ParseRule());

        _position--;

        return node;
    }

    private ASTNode ParseTokenSymbolSpec()
    {
        return new TokenSymbolNode(Previous().Text);
    }

    private ASTNode ParseTypeSpec()
    {
        var nameToken = Consume();

        Expect(TokenType.GoesTo);

        var subtypes = new List<ASTNode>();

        //ParseSubTypes until peek ;
        while (Peek(0).Type != TokenType.Semicolon)
        {
            subtypes.Add(ParseSubTypeSpec());
        }

        return new TypeDeclaration(nameToken.Text, new Block(subtypes));
    }

    private Expr ParseUnary()
    {
        var expr = ParsePrimary();

        var token = Consume();

        if (token.Type == TokenType.Plus)
        {
            return new AST.Expressions.OneOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Star)
        {
            return new AST.Expressions.ZeroOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Question)
        {
            return new AST.Expressions.OptionalExpression(expr);
        }
        else if (token.Type == TokenType.Exclamation)
        {
            return new AST.Expressions.NotExpression(expr);
        }

        return expr;
    }

    private Token Peek(int offset = 1)
    {
        if (_position + offset >= _tokens.Count)
        {
            return new Token(TokenType.EOF);
        }

        return _tokens[_position + offset];
    }

    private bool PeekMatch(TokenType type)
    {
        if (_position >= _tokens.Count) return false;

        return _tokens[_position - 1].Type == type;
    }

    private Token Previous()
    {
        if (_position >= _tokens.Count) return _tokens[_tokens.Count - 1];

        return _tokens[_position - 1];
    }

    private Block ProgramUnits()
    {
        var result = new List<ASTNode>();

        while (Peek(0).Type != (TokenType.EOF))
        {
            var token = Consume();

            if (token.Type == TokenType.At)
            {
                var keyword = Consume();

                if (keyword.Type == TokenType.Identifier && keyword.Text == "token")
                {
                    result.Add(ParseTokenSpec());
                }
                else if (keyword.Type == TokenType.Identifier && keyword.Text == "type")
                {
                    result.Add(ParseTypeSpec());
                }
            }
            else
            {
                result.Add(ParseRule());
            }

            Expect(TokenType.Semicolon);
        }

        return new Block(result);
    }
}