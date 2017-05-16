using System;
using System.Collections.Generic;
using warehouselang.Ast;
using warehouselang.Lexer;

namespace warehouselang.Parser
{
  internal partial class Parser
  {
    private void CheckErrors()
    {
      if (Errors.Count != 0)
      {
        throw new Exception(string.Join("\n", Errors));
      }
    }

    private int PeekPrecedence()
    {
      if (_precedence.ContainsKey(PeekToken.Type))
      {
        return _precedence[PeekToken.Type];
      }

      return LOWEST;
    }

    private int CurPrecedence()
    {
      if (_precedence.ContainsKey(CurToken.Type))
      {
        return _precedence[CurToken.Type];
      }

      return LOWEST;
    }

    private void NextToken()
    {
      CurToken = PeekToken;
      PeekToken = _lexer.NextToken();
    }

    private bool CurTokenIs(TokenType tokenType)
    {
      return CurToken.Type == tokenType;
    }

    private bool PeekTokenIs(TokenType tokenType)
    {
      return PeekToken.Type == tokenType;
    }

    private void PeekError(TokenType tokenType)
    {
      Errors.Add($"expected next token to be {tokenType}, got {PeekToken.Type} instead. Line: {PeekToken.Line}");
    }

    private bool ExpectPeek(TokenType tokenType)
    {
      if (PeekTokenIs(tokenType))
      {
        NextToken();
        return true;
      }

      PeekError(tokenType);

      return false;
    }

    private void RegisterPrefix(TokenType tokenType, Func<IExpression> prefixParseFn)
    {
      PrefixParseFns[tokenType] = prefixParseFn;
    }

    private void RegisterInfix(TokenType tokenType, Func<IExpression, IExpression> infixParseFn)
    {
      InfixParseFns[tokenType] = infixParseFn;
    }

    private bool PeekTokenAtSameLine()
    {
      return CurToken.Line == PeekToken.Line;
    }

    private void NoPrefixParseFnError(TokenType tokenType)
    {
      Errors.Add($"no prefix function for {tokenType}. Line: {CurToken.Line}");
    }
  }
}
