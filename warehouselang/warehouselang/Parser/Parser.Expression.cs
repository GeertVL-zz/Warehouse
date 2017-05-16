using System;
using System.Collections.Generic;
using warehouselang.Ast;
using warehouselang.Lexer;

namespace warehouselang.Parser
{
  internal partial class Parser
  {
    private IExpression ParseExpression(int precedence)
    {
      var prefix = PrefixParseFns[CurToken.Type];
      if (prefix == null)
      {
        NoPrefixParseFnError(CurToken.Type);
        return null;
      }
      var leftExp = prefix();

      while (!PeekTokenIs(TokenType.Semicolon) && precedence < PeekPrecedence() && PeekTokenAtSameLine())
      {
        var infix = InfixParseFns[PeekToken.Type];
        if (infix == null)
        {
          return leftExp;
        }
        NextToken();
        leftExp = infix(leftExp);
      }

      return leftExp;
    }

    private IExpression ParseSelfExpression()
    {
      return new SelfExpression { Token = CurToken };
    }

    private IExpression ParseIdentifier()
    {
      return new Identifier { Token = CurToken, Value = CurToken.Literal };
    }

    private IExpression ParseConstant()
    {
      return new Constant { Token = CurToken, Value = CurToken.Literal };
    }

    private IExpression ParseInstanceVariable()
    {
      return new InstanceVariable { Token = CurToken, Value = CurToken.Literal };
    }

    private IExpression ParseIntegerLiteral()
    {
      var lit = new IntegerLiteral { Token = CurToken };

      long value;
      if (!Int64.TryParse(lit.TokenLiteral(), out value))
      {
        var msg = $"could not parse {lit.TokenLiteral()} as integer";
        Errors.Add(msg);
        return null;
      }

      lit.Value = (int)value;

      return lit;
    }

    private IExpression ParseStringLiteral()
    {
      var lit = new StringLiteral { Token = CurToken };
      lit.Value = CurToken.Literal;

      return lit;
    }

    private IExpression ParseBooleanLiteral()
    {
      var lit = new Ast.BooleanExpression { Token = CurToken };

      bool value;
      if (!bool.TryParse(lit.TokenLiteral(), out value))
      {
        var msg = $"could not parse {lit.TokenLiteral()} as boolean";
        Errors.Add(msg);
        return null;
      }

      lit.Value = value;

      return lit;
    }

    private IExpression ParsePostFixExpression(IExpression receiver)
    {
      var arguments = new List<IExpression>();
      return new CallExpression { Token = CurToken, Receiver = receiver, Method = CurToken.Literal, Arguments = arguments };
    }

    private IExpression ParseHashExpression()
    {
      var hash = new HashExpression { Token = CurToken };
      hash.Data = ParseHashPairs();

      return hash;
    }

    private Dictionary<string, IExpression> ParseHashPairs()
    {
      var pairs = new Dictionary<string, IExpression>();

      if (PeekTokenIs(TokenType.RBrace))
      {
        NextToken();
        return pairs;
      }

      ParseHashPair(pairs);

      while (PeekTokenIs(TokenType.Comma))
      {
        NextToken();
        ParseHashPair(pairs);
      }

      if (!ExpectPeek(TokenType.RBrace))
      {
        return null;
      }

      return pairs;
    }

    private void ParseHashPair(Dictionary<string, IExpression> pairs)
    {
      if (!ExpectPeek(TokenType.Ident))
      {
        return;
      }

      var key = ((Identifier)ParseIdentifier()).Value;
      if (ExpectPeek(TokenType.Colon))
      {
        return;
      }

      NextToken();
      var value = ParseExpression(LOWEST);
      pairs[key] = value;
    }

    private IExpression ParseArrayExpression()
    {
      var arr = new ArrayExpression { Token = CurToken };
      arr.Elements = ParseArrayElements();
      return arr;
    }

    private IExpression ParseArrayIndexExpression(IExpression left)
    {
      var callExpression = new CallExpression { Receiver = left, Method = "[]", Token = CurToken };
      if (PeekTokenIs(TokenType.RBracket))
      {
        return null;
      }

      NextToken();

      callExpression.Arguments = new List<IExpression> { ParseExpression(LOWEST) };
      if (ExpectPeek(TokenType.RBracket))
      {
        return null;
      }

      if (PeekTokenIs(TokenType.Assign))
      {
        NextToken();
        NextToken();
        var assignValue = ParseExpression(LOWEST);
        callExpression.Method = "[]=";
        callExpression.Arguments.Add(assignValue);
      }

      return callExpression;
    }

    private List<IExpression> ParseArrayElements()
    {
      var elems = new List<IExpression>();
      if (PeekTokenIs(TokenType.RBracket))
      {
        NextToken();
        return elems;
      }

      NextToken();
      elems.Add(ParseExpression(LOWEST));

      while (PeekTokenIs(TokenType.Comma))
      {
        NextToken();
        NextToken();
        elems.Add(ParseExpression(LOWEST));
      }

      if (!ExpectPeek(TokenType.RBracket))
      {
        return null;
      }

      return elems;
    }

    private IExpression ParsePrefixExpression()
    {
      var pe = new PrefixExpression { Token = CurToken, Operator = CurToken.Literal };
      NextToken();
      pe.Right = ParseExpression(PREFIX);

      return pe;
    }

    private IExpression ParseInfixExpression(IExpression left)
    {
      var exp = new InfixExpression { Token = CurToken, Left = left, Operator = CurToken.Literal };
      var precedence = CurPrecedence();
      NextToken();
      exp.Right = ParseExpression(precedence);

      return exp;
    }

    private IExpression ParseGroupedExpression()
    {
      NextToken();
      var exp = ParseExpression(LOWEST);
      if (!ExpectPeek(TokenType.RParen))
      {
        return null;
      }

      return exp;
    }

    private IExpression ParseIfExpression()
    {
      var ie = new IfExpression { Token = CurToken };
      NextToken();
      ie.Condition = ParseExpression(LOWEST);
      ie.Consequence = ParseBlockStatement();

      if (CurTokenIs(TokenType.Else))
      {
        ie.Alternative = ParseBlockStatement();
      }

      return ie;
    }

    private IExpression ParseCallExpression(IExpression receiver)
    {
      CallExpression exp;

      if (CurTokenIs(TokenType.LParen))
      {
        var m = ((Identifier)receiver).Value;
        var selftok = new Token { Type = TokenType.Self, Literal = "self", Line = CurToken.Line };
        var self = new SelfExpression { Token = selftok };
        receiver = self;

        exp = new CallExpression { Token = CurToken, Receiver = receiver, Method = m };
        exp.Arguments = ParseCallArguments();
      }
      else
      {
        exp = new CallExpression { Token = CurToken, Receiver = receiver };
        if (!ExpectPeek(TokenType.Ident))
        {
          return null;
        }
        exp.Method = CurToken.Literal;
        if (PeekTokenIs(TokenType.LParen))
        {
          NextToken();
          exp.Arguments = ParseCallArguments();
        }
        else
        {
          exp.Arguments = new List<IExpression>();
        }
      }

      if (PeekTokenIs(TokenType.Assign))
      {
        exp.Method = exp.Method + "=";
        NextToken();
        NextToken();
        exp.Arguments.Add(ParseExpression(LOWEST));
      }

      if (PeekTokenIs(TokenType.Do))
      {
        ParseBlockParameters(exp);
      }

      return exp;
    }

    private void ParseBlockParameters(CallExpression exp)
    {
      NextToken();
      if (PeekTokenIs(TokenType.Bar))
      {
        var prms = new List<Identifier>();
        NextToken();
        NextToken();
        var param = new Identifier { Token = CurToken, Value = CurToken.Literal };
        prms.Add(param);

        while (PeekTokenIs(TokenType.Comma))
        {
          NextToken();
          NextToken();
          prms.Add(new Identifier { Token = CurToken, Value = CurToken.Literal });
        }

        if (ExpectPeek(TokenType.Bar))
        {
          return;
        }

        exp.BlockArguments = prms;
      }

      exp.Block = ParseBlockStatement();
    }

    private List<IExpression> ParseCallArguments()
    {
      var args = new List<IExpression>();
      if (PeekTokenIs(TokenType.RParen))
      {
        NextToken();
        return args;
      }

      NextToken();
      args.Add(ParseExpression(LOWEST));

      while (PeekTokenIs(TokenType.Comma))
      {
        NextToken();
        NextToken();
        args.Add(ParseExpression(LOWEST));
      }

      if (ExpectPeek(TokenType.RParen))
      {
        return null;
      }

      return args;
    }

    private IExpression ParseYieldExpression()
    {
      var ye = new YieldExpression { Token = CurToken };
      if (PeekTokenIs(TokenType.LParen))
      {
        NextToken();
        ye.Arguments = ParseCallArguments();
      }

      return ye;
    }
  }
}
