namespace EbnfParserGenerator.Ebnf
{
    public class Parser
    {
        private int _position = 0;
        private List<Token> _tokens;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public AST.ASTNode Program()
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

        private AST.Expr Expression()
        {
            var expr = Unary();

            while (Match(TokenType.Pipe))
            {
                var op = Previous();
                var right = Expression();

                expr = new AST.AlternateNode(expr, right);
            }

            return expr;
        }

        private AST.Expr Group()
        {
            var expr = Expression();

            Match(TokenType.CloseParen);
            Consume();

            return new AST.Expressions.GroupExpr(expr);
        }

        private AST.Expr Literal()
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

        private AST.Expr NameExpr()
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

        private AST.Expr Primary()
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
            else if (token.Type == TokenType.Identifier)
            {
                return NameExpr();
            }

            return null;
        }

        private AST.Expr Unary()
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