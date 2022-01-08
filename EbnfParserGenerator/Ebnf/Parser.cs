using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf;

public class Parser : BaseParser<ASTNode, Lexer, Parser>
{
    public Parser(List<Token> tokens, List<Message> messages) : base(tokens, messages)
    {
    }

    protected override ASTNode Start()
    {
        var result = new Block(new List<ASTNode>());

        while (Peek(1).Type != (TokenType.EOF))
        {
            var keyword = NextToken();

            if (keyword.Type == TokenType.TokenKeyword)
            {
                result.Body.Add(ParseTokenSpec(result));
            }
            else if (keyword.Type == TokenType.TypeKeyword)
            {
                result.Body.Add(ParseTypeSpec(result));
            }
            else if (keyword.Type == TokenType.GrammarKeyword)
            {
                result.Body.Add(ParseGrammarBlock(result));
            }
            else
            {
                Messages.Add(Message.Error($"Unknown keyword '{keyword.Text}'. Did you mean 'token', 'type' or 'grammar'?", keyword.Line, keyword.Column));
            }
        }

        return result;
    }

    private Expr ParseExpression()
    {
        var expr = ParseUnary();

        while (Peek(0).Type == TokenType.Pipe)
        {
            NextToken();

            var right = ParseExpression();

            expr = new AlternateNode(expr, right);
        }

        return expr;
    }

    private Block ParseExpressionList()
    {
        var result = new Block();

        while (Previous().Type != TokenType.Semicolon)
        {
            _position--;
            result.Body.Add(ParseExpression());
        }

        return result;
    }

    private ASTNode ParseGrammarBlock(ASTNode parent)
    {
        var nameToken = Match(TokenType.Identifier);

        var forToken = Match(TokenType.For);

        var typeToken = Match(TokenType.Identifier);

        Match(TokenType.OpenCurly);

        if (typeToken.Type != TokenType.Identifier && forToken.Type != TokenType.For)
        {
            Messages.Add(Message.Error("Parser can only be generated if NodeType is specified", forToken.Line, forToken.Column));

            return new InvalidNode();
        }

        var body = new Block();
        var result = new GrammarNode(nameToken.Text, typeToken.Text, body);

        while (Peek(0).Type != TokenType.CloseCurly)
        {
            body.Body.Add(ParseRule(result));
        }

        Match(TokenType.CloseCurly);
        Match(TokenType.Semicolon);

        result.Parent = parent;

        return result;
    }

    private Expr ParseGroup()
    {
        var expr = ParseExpression();

        Match(TokenType.CloseParen);

        return new AST.Expressions.GroupExpr(expr);
    }

    private Expr ParseNameExpr()
    {
        return new AST.Expressions.NameExpression(NextToken());
    }

    private Expr ParsePrimary()
    {
        if (Current.Type == TokenType.StringLiteral)
        {
            return ParseStringLiteral();
        }
        else if (Current.Type == TokenType.OpenParen)
        {
            return ParseGroup();
        }
        else if (Current.Type == TokenType.OpenSquare)
        {
            return ParseRange();
        }
        else if (Current.Type == TokenType.Identifier)
        {
            return ParseNameExpr();
        }
        else
        {
            Messages.Add(Message.Error($"Unknown Expression. Expected String, Group, CharakterClass or Identifier", Current.Line, Current.Column));
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

        Match(TokenType.CloseParen);

        return new AST.Expressions.CharacterClassExpression();
    }

    private Expr ParseRangeExpression()
    {
        throw new NotImplementedException();
    }

    private ASTNode ParseRule(GrammarNode parent)
    {
        var nameToken = NextToken();

        Match(TokenType.GoesTo);

        var exprs = ParseExpression();

        Match(TokenType.Semicolon);

        return new RuleNode(nameToken, new Block(new List<ASTNode>() { exprs }), parent);
    }

    private Expr ParseStringLiteral()
    {
        return new AST.Expressions.LiteralNode(NextToken().Text);
    }

    private ASTNode ParseSubTypeSpec(ASTNode parent)
    {
        // | typename(arg : type,...)
        Match(TokenType.Pipe);

        if (Previous().Type != TokenType.Pipe) return new InvalidNode();

        var typename = Match(TokenType.Identifier);

        var properties = new List<(string name, string type)>();

        Match(TokenType.OpenParen);

        while (Peek(1).Type != TokenType.Semicolon)
        {
            var name = Match(TokenType.Identifier);
            Match(TokenType.Colon);
            var type = Match(TokenType.Identifier);

            properties.Add((name.Text, type.Text));

            if (Peek(0).Type == TokenType.CloseParen)
            {
                break;
            }
            else
            {
                Match(TokenType.Comma);
            }
        }

        Match(TokenType.CloseParen);

        var result = new SubTypeDeclaration(typename.Text, properties);
        result.Parent = parent;

        return result;
    }

    private ASTNode ParseTokenSpec(ASTNode parent)
    {
        var token = NextToken();

        if (token.Type == TokenType.StringLiteral)
        {
            return ParseTokenSymbolSpec(parent);
        }
        else
        {
            return ParseTokenSpecDefinition(parent);
        }
    }

    private ASTNode ParseTokenSpecDefinition(ASTNode parent)
    {
        _position--;

        var node = new TokenSpecNode((RuleNode)ParseRule(new GrammarNode(null, null, new())));
        node.Parent = parent;
        return node;
    }

    private ASTNode ParseTokenSymbolSpec(ASTNode parent)
    {
        var result = new TokenSymbolNode(Previous().Text) { Parent = parent };
        Match(TokenType.Semicolon);

        return result;
    }

    private ASTNode ParseTypeSpec(ASTNode parent)
    {
        var nameToken = Match(TokenType.Identifier);

        Match(TokenType.GoesTo);

        var subtypes = new List<ASTNode>();

        var typeDeclaration = new TypeDeclaration(nameToken.Text, new Block(subtypes)) { Parent = parent };
        while (Peek(0).Type != TokenType.Semicolon)
        {
            subtypes.Add(ParseSubTypeSpec(typeDeclaration.Block));
        }

        typeDeclaration.Block.Parent = typeDeclaration;

        Match(TokenType.Semicolon);

        return typeDeclaration;
    }

    private Expr ParseUnary()
    {
        var expr = ParsePrimary();

        var token = Peek(0);

        if (token.Type == TokenType.Plus)
        {
            NextToken();

            return new AST.Expressions.OneOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Star)
        {
            NextToken();

            return new AST.Expressions.ZeroOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Question)
        {
            NextToken();

            return new AST.Expressions.OptionalExpression(expr);
        }
        else if (token.Type == TokenType.Exclamation)
        {
            NextToken();

            return new AST.Expressions.NotExpression(expr);
        }

        return expr;
    }
}