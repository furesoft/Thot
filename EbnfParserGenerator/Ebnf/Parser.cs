using EbnfParserGenerator.Ebnf.AST;

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
            var keyword = Consume();

            if (keyword.Type == TokenType.TokenKeyword)
            {
                result.Add(ParseTokenSpec());
            }
            else if (keyword.Type == TokenType.TypeKeyword)
            {
                result.Add(ParseTypeSpec());
            }
            else if (keyword.Type == TokenType.GrammarKeyword)
            {
                result.Add(ParseGrammarBlock());
            }
            else
            {
                Messages.Add(Message.Error($"Unknown keyword '{keyword.Text}'. Did you mean 'token', 'type' or 'grammar'?", keyword.Line, keyword.Column));
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
        var nameToken = Expect(TokenType.Identifier);

        var forToken = Expect(TokenType.For);

        var typeToken = Expect(TokenType.Identifier);

        Expect(TokenType.OpenCurly);

        if (typeToken.Type != TokenType.Identifier && forToken.Type != TokenType.For)
        {
            Messages.Add(Message.Error("Parser can only be generated if NodeType is specified", forToken.Line, forToken.Column));

            return new InvalidNode();
        }

        var body = new Block();
        var result = new GrammarNode(nameToken.Text, typeToken.Text, body);

        while (!PeekMatch(TokenType.CloseCurly, 0))
        {
            body.Body.Add(ParseRule(result));
        }

        Expect(TokenType.CloseCurly);

        return result;
    }

    private Expr ParseGroup()
    {
        var expr = ParseExpression();

        Expect(TokenType.CloseParen);

        return new AST.Expressions.GroupExpr(expr);
    }

    private Expr ParseNameExpr()
    {
        return new AST.Expressions.NameExpression(Previous());
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

    private ASTNode ParseRule(GrammarNode parent)
    {
        var nameToken = Expect(TokenType.Identifier);

        Expect(TokenType.GoesTo);

        var expr = ParseExpression();

        Expect(TokenType.Semicolon);

        return new RuleNode(nameToken, new Block(new List<ASTNode> { expr }), parent);
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

        var node = new TokenSpecNode((RuleNode)ParseRule(new GrammarNode(null, null, new())));
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

        var token = Peek();

        if (token.Type == TokenType.Plus)
        {
            Consume();

            return new AST.Expressions.OneOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Star)
        {
            Consume();

            return new AST.Expressions.ZeroOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Question)
        {
            Consume();

            return new AST.Expressions.OptionalExpression(expr);
        }
        else if (token.Type == TokenType.Exclamation)
        {
            Consume();

            return new AST.Expressions.NotExpression(expr);
        }

        return expr;
    }
}