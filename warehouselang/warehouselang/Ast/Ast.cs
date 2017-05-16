using System;
using System.Collections.Generic;
using System.Text;
using warehouselang.Lexer;

namespace warehouselang.Ast
{
  interface INode
  {
    string TokenLiteral();
  }

  interface IStatement : INode
  {
    void StatementNode();
  }

  interface IExpression : INode
  {
    void ExpressionNode();
  }

  interface IVariable : INode
  {
    void VariableNode();
    string ReturnValue();
  }

  class Identifier : IVariable, IExpression
  {
    public Token Token { get; set; }
    public string Value { get; set; }

    public string ReturnValue()
    {
      return Value;
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public void VariableNode()
    {
    }

    public override string ToString()
    {
      return Value;
    }

    public void ExpressionNode()
    {      
    }
  }

  class InstanceVariable : IVariable, IExpression
  {
    public Token Token { get; set; }
    public string Value { get; set; }

    public string ReturnValue()
    {
      return Value;
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public void VariableNode()
    {
    }

    public override string ToString()
    {
      return Value;
    }

    public void ExpressionNode()
    {
    }
  }

  class Constant : IVariable, IExpression
  {
    public Token Token { get; set; }
    public string Value { get; set; }

    public string ReturnValue()
    {
      return Value;
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public void VariableNode()
    {
    }

    public override string ToString()
    {
      return Value;
    }

    public void ExpressionNode()
    {
    }
  }

  class AstProgram
  {
    public List<IStatement> Statements { get; set; }

    public AstProgram()
    {
      Statements = new List<IStatement>();
    }

    public string TokenLiteral()
    {
      if (Statements.Count > 0)
      {
        return Statements[0].TokenLiteral();
      }

      return string.Empty;
    }

    public override string ToString()
    {
      var output = new StringBuilder();
      foreach (var stat in Statements)
      {
        output.Append(stat.ToString());
      }

      return output.ToString();
    }
  }

  class AssignStatement : IStatement
  {
    public Token Token { get; set; }
    public IVariable Name { get; set; }
    public IExpression Value { get; set; }

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
      output.Append(Name.ToString());
      output.Append(" = ");

      if (Value != null)
      {
        output.Append(Value.ToString());
      }

      return output.ToString();
    }
  }

  class DefStatement : IStatement
  {
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression Receiver { get; set; }
    public List<Identifier> Parameters { get; set; }

    public BlockStatement BlockStatement { get; set; }

    public DefStatement()
    {
      Parameters = new List<Identifier>();
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

      output.Append("def ");
      output.Append(Name.TokenLiteral());
      output.Append("(");

      foreach (var param in Parameters)
      {
        output.Append(param.ToString());
        if (Parameters[Parameters.Count - 1] != param)
        {
          output.Append(", ");
        }
      }

      output.Append(") ");
      output.Append("{\n");
      output.Append(BlockStatement.ToString());
      output.Append("\n}");

      return output.ToString();
    }
  }

  class ClassStatement : IStatement
  {
    public Token Token { get; set; }
    public Constant Name { get; set; }

    public BlockStatement Body { get; set; }

    public Constant SuperClass { get; set; }

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
      output.Append("class ");
      output.Append(Name.TokenLiteral());
      output.Append(" {\n");
      output.Append(Body.ToString());
      output.Append("\n}");

      return output.ToString();
    }
  }

  class ReturnStatement : IStatement
  {
    public Token Token { get; set; }

    public IExpression ReturnValue { get; set; }

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

      output.Append(TokenLiteral() + " ");

      if (ReturnValue != null)
      {
        output.Append(ReturnValue.ToString());
      }

      output.Append(";");

      return output.ToString();
    }
  }

  class ExpressionStatement : IStatement
  {
    public Token Token { get; set; }
    public IExpression Expression { get; set; }

    public void StatementNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      if (Expression != null)
      {
        return Expression.ToString();
      }

      return string.Empty;
    }
  }

  class IntegerLiteral : IExpression
  {
    public Token Token { get; set; }
    public int Value { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      return Token.Literal;
    }
  }

  class StringLiteral : IExpression
  {
    public Token Token { get; set; }
    public string Value { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      output.Append("\"");
      output.Append(Token.Literal);
      output.Append("\"");

      return output.ToString();
    }
  }

  class ArrayExpression : IExpression
  {
    public Token Token { get; set; }
    public List<IExpression> Elements { get; set; }

    public ArrayExpression()
    {
      Elements = new List<IExpression>();
    }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();
      output.Append("[");

      if (Elements.Count == 0)
      {
        output.Append("]");
        return output.ToString();
      }

      output.Append(Elements[0].ToString());

      foreach(var elem in Elements)
      {
        output.Append(", ");
        output.Append(elem.ToString());
      }

      output.Append("]");

      return output.ToString();
    }
  }

  class HashExpression : IExpression
  {
    public Token Token { get; set; }

    public Dictionary <string, IExpression> Data { get; set; }

    public HashExpression()
    {
      Data = new Dictionary<string, IExpression>();
    }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();
      var pairs = new List<string>();

      foreach (var item in Data)
      {
        pairs.Add($"{item.Key}: {item.Value}");
      }

      output.Append("{ ");
      output.Append(string.Join(", ", pairs));
      output.Append(" }");

      return output.ToString();
    }
  }

  class PrefixExpression : IExpression
  {
    public Token Token { get; set; }
    public string Operator { get; set; }
    public IExpression Right { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      output.Append("(");
      output.Append(Operator);
      output.Append(Right.ToString());
      output.Append(")");

      return output.ToString();
    }
  }

  class InfixExpression : IExpression
  {
    public Token Token { get; set; }
    public IExpression Left { get; set; }
    public string Operator { get; set; }
    public IExpression Right { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      output.Append("(");
      output.Append(Left.ToString());
      output.Append(" ");
      output.Append(Operator);
      output.Append(" ");
      output.Append(Right.ToString());
      output.Append(")");

      return output.ToString();
    }
  }

  class BooleanExpression : IExpression
  {
    public Token Token { get; set; }
    public bool Value { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      return Token.Literal;
    }
  }

  class NilExpression : IExpression
  {
    public Token Token { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      return "nil";
    }
  }

  class IfExpression : IExpression
  {
    public Token Token { get; set; }
    public IExpression Condition { get; set; }
    public BlockStatement Consequence { get; set; }
    public BlockStatement Alternative { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      output.Append("if");
      output.Append(" ");
      output.Append(Condition.ToString());
      output.Append("\n");
      output.Append(Consequence.ToString());

      if (Alternative != null)
      {
        output.Append("\n");
        output.Append("else\n");
        output.Append(Alternative.ToString());
      }

      output.Append("\nend");

      return output.ToString();
    }
  }

  class CallExpression : IExpression
  {
    public IExpression Receiver { get; set; }
    public Token Token { get; set; }
    public string Method { get; set; }
    public List<IExpression> Arguments { get; set; }
    public BlockStatement Block { get; set; }
    public List<Identifier> BlockArguments { get; set; }

    public CallExpression()
    {
      Arguments = new List<IExpression>();
      BlockArguments = new List<Identifier>();
    }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      output.Append(Receiver.ToString());
      output.Append(".");
      output.Append(Method);

      output.Append("(");
      output.Append(string.Join(",", Arguments));
      output.Append(")");

      if (Block != null)
      {
        output.Append(" do");

        if (BlockArguments.Count > 0)
        {
          output.Append(" |");
          output.Append(string.Join(", ", BlockArguments));
          output.Append("|");
        }

        output.Append("\n");
        output.Append(Block.ToString());
        output.Append("\nend");
      }

      return output.ToString();
    }
  }

  class SelfExpression : IExpression
  {
    public Token Token { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      return "self";
    }
  }

  class WhileStatement : IStatement
  {
    public Token Token { get; set; }
    public IExpression Condition { get; set; }
    public BlockStatement Body { get; set; }

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

      output.Append("while ");
      output.Append(Condition.ToString());
      output.Append(" do\n");
      output.Append(Body.ToString());
      output.Append("\nend");

      return output.ToString();
    }
  }

  class YieldExpression : IExpression
  {
    public Token Token { get; set; }
    public List<IExpression> Arguments { get; set; }

    public void ExpressionNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      var output = new StringBuilder();

      output.Append(TokenLiteral());
      output.Append("(");
      output.Append(string.Join(", ", Arguments));
      output.Append(")");

      return output.ToString();
    }
  }

  class RequireRelativeStatement : IStatement
  {
    public Token Token { get; set; }
    public string FilePath { get; set; }

    public void StatementNode()
    {
    }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public override string ToString()
    {
      return $"require_relative \"{FilePath}\"";
    }
  }
}
