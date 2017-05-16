using System;
using System.Collections.Generic;
using System.Text;
using warehouselang.Lexer;

namespace warehouselang.Ast
{
  class BlockStatement : IStatement
  {
    public Token Token { get; set; }
    public List<IStatement> Statements { get; set; }

    public BlockStatement()
    {
      Statements = new List<IStatement>();
    }

    public void StatementNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      foreach (var statement in Statements)
      {
        output.Append(statement.ToString());
      }

      return output.ToString();
    }
  }
}