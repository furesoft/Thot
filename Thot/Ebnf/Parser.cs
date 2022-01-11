using Thot.Ebnf.AST;
using Thot.Ebnf;

namespace Thot.Ebnf;

public class Parser : BaseParser<ASTNode, Lexer, Parser>
{
    public Parser(SourceDocument document, List<Token> tokens, List<Message> messages) : base(document, tokens, messages)
    {
    }

    protected override ASTNode Start()
    {
        var cu = new CompilationUnit();
        while (Peek(1).Type != (TokenType.EOF))
        {
            var keyword = NextToken();

            if (keyword.Type == TokenType.TokenKeyword)
            {
                cu.Body.Body.Add(ParseTokenSpec(cu));
            }
            else if (keyword.Type == TokenType.TypeKeyword)
            {
                cu.Body.Body.Add(ParseTypeSpec(cu));
            }
            else if (keyword.Type == TokenType.GrammarKeyword)
            {
                cu.Body.Body.Add(ParseGrammarBlock(cu));
            }
            else
            {
                Messages.Add(Message.Error($"Unknown keyword '{keyword.Text}'. Did you mean 'token', 'type' or 'grammar'?", keyword.Line, keyword.Column));
            }
        }

        cu.Messages = Messages;
        cu.Body.Parent = cu;


        return cu;
    }

    private Expr ParseExpression()
    {
        var expr = ParseUnary();

        while (Current.Type == TokenType.Pipe)
        {
            NextToken();

            var right = ParseExpression();

            expr = new AlternateNode(expr, right);
        }

        return expr;
    }

    private Block ParseExpressionList(ASTNode parent)
    {
        var result = new Block(parent);

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
        var result = new GrammarNode(nameToken.Text, typeToken.Text, body, parent);
        body.Parent = result;

        while (Current.Type != TokenType.CloseCurly && Current.Type != TokenType.EOF)
        {
            body.Body.Add(ParseRule(result));
        }

        Match(TokenType.CloseCurly);
        Match(TokenType.Semicolon);

        return result;
    }

    private Expr ParseGroup()
    {
        Match(TokenType.OpenParen);

        var expr = ParseExpression();

        Match(TokenType.CloseParen);

        return new Thot.Ebnf.AST.Expressions.GroupExpr(expr);
    }

    private Expr ParseNameExpr()
    {
        return new Thot.Ebnf.AST.Expressions.NameExpression(NextToken());
    }

    private Expr ParsePrimary()
    {
        if (Current.Type == TokenType.StringLiteral)
        {
            return ParseStringOrRange();
        }
        else if (Current.Type == TokenType.OpenParen)
        {
            return ParseGroup();
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

    private Expr ParseStringOrRange()
    {
        return new Thot.Ebnf.AST.Expressions.LiteralNode(NextToken().Text);
    }

    private ASTNode ParseSubTypeSpec(ASTNode parent)
    {
        // | typename(arg : type,...)
        Match(TokenType.Pipe);

        if (Previous().Type != TokenType.Pipe)
        {
            while (Current.Type != TokenType.Semicolon)
            {
                _position++;
            }

            return new InvalidNode();
        }

        var typename = Match(TokenType.Identifier);

        var properties = new List<(string name, string type)>();

        Match(TokenType.OpenParen);

        while (Peek(1).Type != TokenType.Semicolon)
        {
            var name = Match(TokenType.Identifier);
            Match(TokenType.Colon);
            var type = Match(TokenType.Identifier);

            properties.Add((name.Text, type.Text));

            if (Current.Type == TokenType.CloseParen)
            {
                break;
            }
            else
            {
                Match(TokenType.Comma);
            }
        }

        Match(TokenType.CloseParen);

        return new SubTypeDeclaration(typename.Text, properties, parent);
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

        var node = new TokenSpecNode((RuleNode)ParseRule(new GrammarNode(null, null, new())), parent);
        return node;
    }

    private ASTNode ParseTokenSymbolSpec(ASTNode parent)
    {
        var result = new TokenSymbolNode(Previous().Text, parent);
        Match(TokenType.Semicolon);

        return result;
    }

    private ASTNode ParseTypeSpec(ASTNode parent)
    {
        var nameToken = Match(TokenType.Identifier);

        Match(TokenType.GoesTo);

        var subtypes = new List<ASTNode>();

        var typeDeclaration = new TypeDeclaration(nameToken.Text, new Block(subtypes), parent);
        while (Current.Type != TokenType.Semicolon)
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

        var token = Current;

        if (token.Type == TokenType.Plus)
        {
            NextToken();

            return new Thot.Ebnf.AST.Expressions.OneOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Star)
        {
            NextToken();

            return new Thot.Ebnf.AST.Expressions.ZeroOrMoreExpression(expr);
        }
        else if (token.Type == TokenType.Question)
        {
            NextToken();

            return new Thot.Ebnf.AST.Expressions.OptionalExpression(expr);
        }
        else if (token.Type == TokenType.Exclamation)
        {
            NextToken();

            return new Thot.Ebnf.AST.Expressions.NotExpression(expr);
        }

        return expr;
    }
}
