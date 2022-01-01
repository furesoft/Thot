using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf
{
    public class Parser
    {
        public readonly List<Message> Messages;
        private int _position = 0;
        private List<Token> _tokens;

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
            var node = Expression();

            Match(TokenType.EOF);

            return node;
        }

        private Token Consume()
        {
            var token = Peek(0);

            _position++;

            return token;
        }

        private Expr Expression()
        {
            var expr = Unary();

            while (Match(TokenType.Pipe))
            {
                var op = Previous();
                var right = Expression();

                expr = new AlternateNode(expr, right);
            }

            return expr;
        }

        private Expr Group()
        {
            var expr = Expression();

            Match(TokenType.CloseParen);
            Consume();

            return new AST.Expressions.GroupExpr(expr);
        }

        private Expr Literal()
        {
            var token = Previous();

            if (token.Type == TokenType.StringLiteral)
            {
                return new AST.Expressions.LiteralNode(token.Text);
            }

            return null;
        }

        private bool Match(TokenType type)
        {
            return Previous().Type == type;
        }

        private Expr NameExpr()
        {
            if (Previous().Type != TokenType.Identifier) return null;

            return new AST.Expressions.NameExpression(Previous().Text);
        }

        private Token Peek(int offset = 1)
        {
            if (_position + offset >= _tokens.Count)
            {
                return new Token(TokenType.EOF);
            }

            return _tokens[_position];
        }

        private Token Previous()
        {
            return _tokens[_position - 1];
        }

        private Expr Primary()
        {
            var token = Consume();

            if (token.Type == TokenType.StringLiteral)
            {
                return Literal();
            }
            else if (token.Type == TokenType.OpenParen)
            {
                return Group();
            }
            else if (token.Type == TokenType.OpenSquare)
            {
                return Range();
            }
            else if (token.Type == TokenType.Identifier)
            {
                return NameExpr();
            }

            return null;
        }

        //[_"k]
        //[a-z]
        //[a-Z0-9]
        //[_a-z0-9]
        private Expr Range()
        {
            var expr = Expression();

            Match(TokenType.CloseSquare);
            Consume();

            return new AST.Expressions.GroupExpr(expr);
        }

        private Expr Unary()
        {
            var expr = Primary();

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
}