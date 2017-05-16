using System.Collections.Generic;
using warehouselang.Lexer;

namespace warehouselang.Parser
{
  internal partial class Parser
  {
    private const int LOWEST = 1;
    private const int EQUALS = 2;
    private const int LESSGREATER = 3;
    private const int SUM = 4;
    private const int PRODUCT = 5;
    private const int PREFIX = 6;
    private const int INDEX = 7;
    private const int CALL = 8;

    private readonly Dictionary<TokenType, int> _precedence = new Dictionary<TokenType, int>
    {
      { TokenType.Eq, EQUALS },
      { TokenType.NotEq, EQUALS },
      { TokenType.Lt, LESSGREATER },
      { TokenType.Lte, LESSGREATER },
      { TokenType.Gt, LESSGREATER },
      { TokenType.Gte, LESSGREATER },
      { TokenType.Comp, LESSGREATER },
      { TokenType.Plus, SUM },
      { TokenType.Minus, SUM },
      { TokenType.Incr, SUM },
      { TokenType.Decr, SUM },
      { TokenType.Slash, PRODUCT },
      { TokenType.Asterisk, PRODUCT },
      { TokenType.Pow, PRODUCT },
      { TokenType.LBracket, INDEX },
      { TokenType.Dot, CALL },
      { TokenType.LParen, CALL }
    };

  }
}
