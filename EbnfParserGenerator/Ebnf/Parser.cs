﻿using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf;

public class Parser : BaseParser<ASTNode, Lexer, Parser>
{
    public Parser(List<Token> tokens, List<Message> messages) : base(tokens, messages)
    {
    }

    protected override ASTNode Start()
    {
        var result = new List<ASTNode>();

        while (Peek(0).Type != (TokenType.EOF))
        {
            var token = Consume();

            if (token.Type == TokenType.At)
            {
                var keyword = Consume();

                if (keyword.Type == TokenType.TokenKeyword)
                {
                    result.Add(ParseTokenSpec());
                }
                else if (keyword.Type == TokenType.TypeKeyword)
                {
                    result.Add(ParseTypeSpec());
                }
                else
                {
                    Messages.Add(Message.Error($"Unknown option '{keyword.Text}'. Did you mean 'token' or 'type'?", token.Line, token.Column));
                }
            }
            else if (token.Type == TokenType.GrammarKeyword)
            {
                result.Add(ParseGrammarBlock());
            }
            else
            {
                Messages.Add(Message.Error($"Unknown Token '{token.Text}'. Did you mean @token, @type or grammar?", token.Line, token.Column));
            }

            Expect(TokenType.Semicolon);
        }

        return new Block(result);
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

    private ASTNode ParseGrammarBlock()
    {
        throw new NotImplementedException();
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
            Messages.Add(Message.Error($"Unknown Expression. Did you mean one of [] '' \"\" () or identifier?", token.Line, token.Column));
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
}