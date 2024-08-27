namespace Prefixr;

internal class Token
{
    private readonly TokenType _type;
    private readonly string _lexeme;
    private readonly object? _literal;
    private readonly int _line;

    internal Token(TokenType type, string lexeme, int line, object? literal = null)
    {
        _type = type;
        _lexeme = lexeme;
        _literal = literal;
        _line = line;
    }

    public override string ToString()
    {
        return $"type: {_type} lexeme: {_lexeme} value: {_literal}";
    }
}