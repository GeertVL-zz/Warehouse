using System;
using System.Collections.Generic;
using warehouselang.Ast;
using warehouselang.Lexer;

namespace warehouselang.Parser
{
  internal partial class Parser
  {
    private Lexer.Lexer _lexer;
    public List<string> Errors { get; set; }

    public Token CurToken { get; set; }
    public Token PeekToken { get; set; }

    public Dictionary<TokenType, Func<IExpression>> PrefixParseFns { get; set; }
    public Dictionary<TokenType, Func<IExpression, IExpression>> InfixParseFns { get; set; }


    public static AstProgram BuildAST(string fileContent)
    {
      var lexer = new Lexer.Lexer(fileContent);
      var p = new Parser(lexer);

      var program = p.ParseProgram();
      p.CheckErrors();

      return program;
    }

    public Parser(Lexer.Lexer lexer)
    {
      _lexer = lexer;
      Errors = new List<string>();
      PrefixParseFns = new Dictionary<TokenType, Func<IExpression>>();
      InfixParseFns = new Dictionary<TokenType, Func<IExpression, IExpression>>();

      NextToken();
      NextToken();

      RegisterPrefix(TokenType.Ident, ParseIdentifier);
      RegisterPrefix(TokenType.Constant, ParseConstant);
    }

    public AstProgram ParseProgram()
    {
      var program = new AstProgram();

      while (!CurTokenIs(TokenType.Eof))
      {
        var statement = ParseStatement();
        if (statement != null)
        {
          program.Statements.Add(statement);
        }
        NextToken();
      }

      return program;
    }

    private IStatement ParseStatement()
    {
      switch (CurToken.Type)
      {
        case TokenType.InstanceVariable:
        case TokenType.Ident:
        case TokenType.Constant:
          if (CurToken.Literal == "class")
          {
            CurToken.Type = TokenType.Class;
            return ParseStatement();
          }
          if (PeekTokenIs(TokenType.Assign))
          {
            ParseAssignStatement();
          }

          return ParseExpressionStatement();
        case TokenType.Return:
          return ParseReturnStatement();
        case TokenType.Def:
          return ParseDefMethodStatement();
        case TokenType.Class:
          return ParseClassStatement();
        case TokenType.Comment:
          return null;
        case TokenType.While:
          return ParseWhileStatement();
        case TokenType.RequireRelative:
          return ParseRequireRelativeStatement();
        default:
          return ParseExpressionStatement();
      }
    }

    private RequireRelativeStatement ParseRequireRelativeStatement()
    {
      var stmt = new RequireRelativeStatement { Token = CurToken };
      NextToken();

      var filePath = CurToken.Literal;
      stmt.FilePath = filePath;

      return stmt;
    }

    private DefStatement ParseDefMethodStatement()
    {
      var stmt = new DefStatement { Token = CurToken };
      NextToken();

      switch (CurToken.Type)
      {
        case TokenType.Ident:
          if (PeekTokenIs(TokenType.Dot))
          {
            stmt.Receiver = new Identifier { Token = CurToken, Value = CurToken.Literal };
            NextToken();
            if (!ExpectPeek(TokenType.Ident))
            {
              return null;
            }
            stmt.Name = new Identifier { Token = CurToken, Value = CurToken.Literal };
          }
          else
          {
            stmt.Name = new Identifier { Token = CurToken, Value = CurToken.Literal };
          }
          break;
        case TokenType.Self:
          stmt.Receiver = new SelfExpression { Token = CurToken };
          NextToken();
          if (ExpectPeek(TokenType.Ident))
          {
            return null;
          }
          stmt.Name = new Identifier { Token = CurToken, Value = CurToken.Literal };
          break;
        default:
          return null;
      }

      if (PeekTokenIs(TokenType.Assign))
      {
        stmt.Name.Value = stmt.Name.Value + "=";
        NextToken();
      }

      if (PeekTokenAtSameLine())
      {
        if (!ExpectPeek(TokenType.LParen))
        {
          return null;
        }

        stmt.Parameters = ParseParameters();
      }
      else
      {
        stmt.Parameters = new List<Identifier>();
      }

      stmt.BlockStatement = ParseBlockStatement();

      return stmt;
    }

    private ClassStatement ParseClassStatement()
    {
      var stmt = new ClassStatement { Token = CurToken };
      if (!ExpectPeek(TokenType.Constant))
      {
        return null;
      }

      stmt.Name = new Constant { Token = CurToken, Value = CurToken.Literal };

      if (PeekTokenIs(TokenType.Lt))
      {
        NextToken();
        NextToken();
        stmt.SuperClass = new Constant { Token = CurToken, Value = CurToken.Literal };
      }

      stmt.Body = ParseBlockStatement();

      return stmt;
    }

    private List<Identifier> ParseParameters()
    {
      var identifiers = new List<Identifier>();

      if (PeekTokenIs(TokenType.RParen))
      {
        NextToken();
        return identifiers;
      }

      NextToken();

      var ident = new Identifier { Token = CurToken, Value = CurToken.Literal };
      identifiers.Add(ident);

      while (PeekTokenIs(TokenType.Comma))
      {
        NextToken();
        NextToken();
        identifiers.Add(new Identifier { Token = CurToken, Value = CurToken.Literal });
      }

      if (!ExpectPeek(TokenType.RParen))
      {
        return null;
      }

      return identifiers;
    }

    private AssignStatement ParseAssignStatement()
    {
      var stmt = new AssignStatement { Token = CurToken };

      switch (CurToken.Type)
      {
        case TokenType.Ident:
          stmt.Name = new Identifier { Token = CurToken, Value = CurToken.Literal };
          break;
        case TokenType.Constant:
          stmt.Name = new Constant { Token = CurToken, Value = CurToken.Literal };
          break;
        case TokenType.InstanceVariable:
          stmt.Name = new InstanceVariable { Token = CurToken, Value = CurToken.Literal };
          break;
      }

      if (!ExpectPeek(TokenType.Assign))
      {
        return null;
      }

      NextToken();

      stmt.Value = ParseExpression(LOWEST);

      if (PeekTokenIs(TokenType.Semicolon))
      {
        NextToken();
      }

      return stmt;
    }

    private ReturnStatement ParseReturnStatement()
    {
      var stmt = new ReturnStatement { Token = CurToken };

      NextToken();

      stmt.ReturnValue = ParseExpression(LOWEST);

      if (PeekTokenIs(TokenType.Semicolon))
      {
        NextToken();
      }

      return stmt;
    }

    private ExpressionStatement ParseExpressionStatement()
    {
      var stmt = new ExpressionStatement { Token = CurToken };

      stmt.Expression = ParseExpression(LOWEST);
      if (PeekTokenIs(TokenType.Semicolon))
      {
        NextToken();
      }

      return stmt;
    }

    private BlockStatement ParseBlockStatement()
    {
      var bs = new BlockStatement { Token = CurToken };

      NextToken();

      while (!CurTokenIs(TokenType.End) && !CurTokenIs(TokenType.Else))
      {
        var stmt = ParseStatement();
        if (stmt != null)
        {
          bs.Statements.Add(stmt);
        }
        NextToken();
      }

      return bs;
    }

    private WhileStatement ParseWhileStatement()
    {
      var ws = new WhileStatement { Token = CurToken };
      NextToken();
      ws.Condition = ParseExpression(LOWEST);
      ws.Body = ParseBlockStatement();

      return ws;
    }

    
  }
}
