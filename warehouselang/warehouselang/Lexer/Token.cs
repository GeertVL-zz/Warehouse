using System;

namespace warehouselang.Lexer
{
  internal class Token
  {
    public string Literal { get; set; }
    public TokenType Type { get; set; }
    public int Line { get; set; }

    internal static TokenType LookupIdent(string literal)
    {
      throw new NotImplementedException();
    }
  }
}