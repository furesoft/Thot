namespace EbnfParserGenerator.Ebnf
{
    public class Lexer
    {
        public List<Message> Messages = new List<Message>();
        private int _column = 1;
        private int _line = 1;
        private int _position = 0;
        private string _source;

        private Dictionary<char, TokenType> _symbolTokens = new()
        {
            [';'] = TokenType.Semicolon,
            ['?'] = TokenType.Question,
            ['('] = TokenType.OpenParen,
            [')'] = TokenType.CloseParen,
            ['+'] = TokenType.Plus,
            ['|'] = TokenType.Pipe,
            ['!'] = TokenType.Exclamation,
            ['*'] = TokenType.Star,
            ['→'] = TokenType.GoesTo
        };

        public List<Token> Tokenize(string source)
        {
            _source = source;

            var tokens = new List<Token>();

            Token newToken = null;
            do
            {
                newToken = NextToken();

                tokens.Add(newToken);
            } while (newToken.Type != TokenType.EOF);

            return tokens;
        }

        private char current()
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

            if (this.current() == '\n' || this.current() == '\r')
            {
                _line++;
                _column = 1;
            }

            if (_symbolTokens.ContainsKey(this.current()))
            {
                return new Token(_symbolTokens[this.current()], this.current().ToString(), _position++, _position, _line, ++_column);
            }
            else if (this.current() == '-' && _source.Length >= 2)
            {
                if (peek(1) == '>')
                {
                    return new Token(TokenType.GoesTo, "->", _position, ++_position, _line, ++_column);
                }
                else
                {
                    ReportError();
                }
            }
            else if (this.current() == '\'')
            {
                int oldpos = ++_position;
                int oldColumn = _column;

                while (peek() != '\'') //ToDo: add end of file check
                {
                    _position++;
                    _column++;
                }

                _column += 2;

                return new Token(TokenType.StringLiteral, _source.Substring(oldpos, _position - oldpos), oldpos - 1, ++_position, _line, oldColumn);
            }
            else if (this.current() == '"')
            {
                int oldpos = ++_position;
                int oldColumn = _column;
                while (peek() != '"')
                {
                    _position++;
                    _column++;
                }

                _column += 2;

                return new Token(TokenType.StringLiteral, _source.Substring(oldpos, _position - oldpos), oldpos - 1, ++_position, _line, oldColumn);
            }
            else
            {
                int oldpos = _position;

                if (char.IsLetter(this.current()))
                {
                    while (char.IsLetterOrDigit(peek(0)))
                    {
                        _position++;
                    }

                    return new Token(TokenType.Identifier, _source.Substring(oldpos, _position - oldpos), oldpos, _position, _line, _column);
                }

                ReportError();
            }

            return Token.Invalid;
        }

        private char peek(int offset = 0)
        {
            if (_position >= _source.Length)
            {
                return '\0';
            }

            return _source[_position + offset];
        }

        private void ReportError()
        {
            Messages.Add(Message.Error($"Unknown Charackter '{current()}'", _line, _column++));
            _position++;
        }

        private void SkipWhitespaces()
        {
            while (char.IsWhiteSpace(current()))
            {
                _position++;
                _column++;
            }
        }
    }
}