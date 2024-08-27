using static Prefixr.TokenType;

namespace Prefixr;

internal class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = [];
    private int _start, _current, _line = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        ["let"] = Let
    };

    protected internal Scanner(string source)
    {
        _source = source;
    }

    protected internal List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }
        
        _tokens.Add(new Token(EndOfFile, string.Empty, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();

        switch (c)
        {
            case '(':
                AddToken(LeftParen);
                break;
            case ')':
                AddToken(RightParen);
                break;
            case '-':
                AddToken(Minus);
                break;
            case '+':
                AddToken(Plus);
                break;
            case '*':
                AddToken(Asterisk);
                break;
            case '/':
                // if the next character is '/', it's defined as a comment, and we skip the line.
                if (IsMatch('/')) while (Peek() != '\n' && !IsAtEnd()) Advance();
                else AddToken(Slash);
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            default:
                if (IsDigit(c)) AddNumber();
                else if (IsAlpha(c)) AddIdentifier();
                else Interpreter.Error(_line, $"Unrecognized character '{c}'");
                break;
        }
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        var text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, _line, literal));
    }

    private void AddNumber()
    {
        while (IsDigit(Peek())) Advance();
        
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
            while (IsDigit(Peek())) Advance();
        }
        
        AddToken(Number, Convert.ToDouble(_source.Substring(_start, _current - _start)));
    }

    private void AddIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();
        var text = _source.Substring(_start, _current - _start);
        AddToken(Keywords.GetValueOrDefault(text, Identifier));
    }

    private char Advance() => _source[_current++];
    private char Peek() => IsAtEnd() ? '\0' : _source[_current];
    private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
    private static bool IsDigit(char c) => c is >= '0' and <= '9';
    private static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
    private bool IsAtEnd() => _current >= _source.Length;
    
    private bool IsMatch(char expected)
    {
        if (IsAtEnd() || _source[_current] != expected) return false;
        _current++;
        return true;
    }
}