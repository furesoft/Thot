using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf
{
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

        private void ExpectKeyword(string name)
        {
            Token token = Peek();

            Consume();

            if (!token.Text.Equals(name))
            {
                Messages.Add(Message.Error($"Expected {name} but got {token.Text}", token.Line, token.Column));
            }
        }

        private Expr Expression()
        {
            var expr = Unary();

            while (PeekMatch(TokenType.Pipe))
            {
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

        private bool Match(TokenType type)
        {
            Token token = Peek();
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

        private Expr NameExpr()
        {
            return new AST.Expressions.NameExpression(Previous().Text);
        }

        private ASTNode ParseTokenSpecDefinition()
        {
            _position--;

            var node = new TokenSpecNode((RuleNode)Rule());

            _position--;

            return node;
        }

        private ASTNode ParseTokenSymbolSpec()
        {
            return new TokenSymbolNode(Previous().Text);
        }

        private Token Peek(int offset = 1)
        {
            if (_position + offset >= _tokens.Count)
            {
                return new Token(TokenType.EOF);
            }

            return _tokens[_position];
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

        private Expr Primary()
        {
            var token = Consume();

            if (token.Type == TokenType.StringLiteral)
            {
                return StringLiteral();
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
            else
            {
                Messages.Add(Message.Error($"Unknown Expression {token.Text}", token.Line, token.Column));
            }

            return new InvalidExpr();
        }

        private Block ProgramUnits()
        {
            var result = new List<ASTNode>();

            while (Peek(0).Type != (TokenType.EOF))
            {
                var token = Consume();

                if (token.Type == TokenType.At)
                {
                    result.Add(TokenSpec());
                }
                else
                {
                    result.Add(Rule());
                }

                Match(TokenType.Semicolon);
                Consume();
            }

            return new Block(result);
        }

        //[_"k]
        //[a-z]
        //[a-Z0-9]
        //[_a-z0-9]
        private Expr Range() //ToDo: implement Range Expression
        {
            var expr = RangeExpression();

            Match(TokenType.CloseSquare);
            Consume();

            return new AST.Expressions.CharacterClassExpression();
        }

        private Expr RangeExpression()
        {
            throw new NotImplementedException();
        }

        private ASTNode Rule()
        {
            Match(TokenType.Identifier, out var nameToken);
            Consume();

            Match(TokenType.GoesTo);
            Consume();

            var expr = Expression();

            return new RuleNode(nameToken.Text, new List<Expr> { expr });
        }

        private Expr StringLiteral()
        {
            var token = Previous();

            return new AST.Expressions.LiteralNode(token.Text);
        }

        private ASTNode TokenSpec()
        {
            ExpectKeyword("token");

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