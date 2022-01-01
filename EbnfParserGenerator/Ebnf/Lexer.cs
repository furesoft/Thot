namespace EbnfParserGenerator.Ebnf
{
    public class Lexer
    {
        public List<Message> Messages = new();

        private readonly Dictionary<char, TokenType> _symbolTokens = new()
        {
            [';'] = TokenType.Semicolon,
            ['?'] = TokenType.Question,
            ['('] = TokenType.OpenParen,
            [')'] = TokenType.CloseParen,
            ['+'] = TokenType.Plus,
            ['|'] = TokenType.Pipe,
            ['!'] = TokenType.Exclamation,
            ['*'] = TokenType.Star,
            ['→'] = TokenType.GoesTo,
            ['['] = TokenType.OpenSquare,
            [']'] = TokenType.CloseSquare,
            ['-'] = TokenType.Minus,
        };

        private int _column = 1;
        private int _line = 1;
        private int _position = 0;
        private string _source;

        public List<Token> Tokenize(string source)
        {
            _source = source;

            var tokens = new List<Token>();

            Token newToken;
            do
            {
                newToken = NextToken();

                tokens.Add(newToken);
            } while (newToken.Type != TokenType.EOF);

            return tokens;
        }

        private char Current()
        {
            if (_position >= _source.Length)
            {
                return '\0';
            }

            return _source[_position];
        }

        private Token? NextToken()
        {
            if (_position >= _source.Length)
            {
                return new Token(TokenType.EOF);
            }

            SkipWhitespaces();

            if (this.Current() == '\n' || this.Current() == '\r')
            {
                _line++;
                _column = 1;
            }

            if (_symbolTokens.ContainsKey(this.Current()))
            {
                return new Token(_symbolTokens[this.Current()], this.Current().ToString(), _position++, _position, _line, ++_column);
            }
            else if (this.Current() == '-' && _source.Length >= 2)
            {
                if (Peek(1) == '>')
                {
                    return new Token(TokenType.GoesTo, "->", _position, ++_position, _line, ++_column);
                }
                else
                {
                    ReportError();
                }
            }
            else if (this.Current() == '\'')
            {
                int oldpos = ++_position;
                int oldColumn = _column;

                while (Peek() != '\'') //ToDo: add end of file check
                {
                    _position++;
                    _column++;
                }

                _column += 2;

                return new Token(TokenType.StringLiteral, _source.Substring(oldpos, _position - oldpos), oldpos - 1, ++_position, _line, oldColumn);
            }
            else if (this.Current() == '"')
            {
                int oldpos = ++_position;
                int oldColumn = _column;
                while (Peek() != '"')
                {
                    _position++;
                    _column++;
                }

                _column += 2;

                return new Token(TokenType.StringLiteral, _source.Substring(oldpos, _position - oldpos), oldpos - 1, ++_position, _line, oldColumn);
            }
            else if (char.IsDigit(this.Current()))
            {
                int oldpos = _position;

                while (char.IsDigit(Peek(0)))
                {
                    _position++;
                }

                return new Token(TokenType.Number, _source.Substring(oldpos, _position - oldpos), oldpos, _position, _line, _column);
            }
            else
            {
                int oldpos = _position;

                if (char.IsLetter(this.Current()))
                {
                    while (char.IsLetterOrDigit(Peek(0)))
                    {
                        _position++;
                    }

                    return new Token(TokenType.Identifier, _source.Substring(oldpos, _position - oldpos), oldpos, _position, _line, _column);
                }

                ReportError();
            }

            return Token.Invalid;
        }

        private char Peek(int offset = 0)
        {
            if (_position >= _source.Length)
            {
                return '\0';
            }

            return _source[_position + offset];
        }

        private void ReportError()
        {
            Messages.Add(Message.Error($"Unknown Charackter '{Current()}'", _line, _column++));
            _position++;
        }

        private void SkipWhitespaces()
        {
            while (char.IsWhiteSpace(Current()))
            {
                _position++;
                _column++;
            }
        }
    }
}