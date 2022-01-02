namespace EbnfParserGenerator.Ebnf;

public class Lexer : BaseLexer
{
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
        ['@'] = TokenType.At,
        [':'] = TokenType.Colon,
        [','] = TokenType.Comma,
    };

    protected override Token NextToken()
    {
        SkipWhitespaces();

        if (_position >= _source.Length)
        {
            return new Token(TokenType.EOF, "\0", _position, _position, _line, 0);
        }

        if (Current() == '\n' || Current() == '\r')
        {
            _line++;
            _column = 1;
        }

        if (Current() == '-' && _source.Length >= 2)
        {
            if (Peek(1) == '>')
            {
                _position++;
                return new Token(TokenType.GoesTo, "->", _position, ++_position, _line, ++_column);
            }
            else
            {
                ReportError();
            }
        }
        else if (Current() == '/' && Peek(1) == '/')
        {
            while (Peek(1) != '\n' && Peek(1) != '\r')
            {
                _position++;
            }

            _column = 1;
        }
        else if (_symbolTokens.ContainsKey(Current()))
        {
            return new Token(_symbolTokens[Current()], Current().ToString(), _position++, _position, _line, ++_column);
        }
        else if (Current() == '\'')
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
        else if (Current() == '"')
        {
            int oldpos = ++_position;
            int oldColumn = _column;
            while (Peek() != '"') // ToDo: add end of file check
            {
                _position++;
                _column++;
            }

            _column += 2;

            return new Token(TokenType.StringLiteral, _source.Substring(oldpos, _position - oldpos), oldpos - 1, ++_position, _line, oldColumn);
        }
        else if (char.IsDigit(Current()))
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

            if (char.IsLetter(Current()))
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

    private void SkipWhitespaces()
    {
        while (char.IsWhiteSpace(Current()) && _position <= _source.Length)
        {
            _position++;
            _column++;
        }
    }
}