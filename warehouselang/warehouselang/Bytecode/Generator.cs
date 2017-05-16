using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using warehouselang.Ast;

namespace warehouselang.Bytecode
{
  internal class LocalTable
  {
    public Dictionary<string, int> Store { get; set; }
    public int Count { get; set; }
    public int Depth { get; set; }
    public LocalTable Upper { get; set; }

    public Tuple<int, bool> Get(string v)
    {
      var ok = Store.ContainsKey(v);
      int value = 0;
      if (ok)
      {
        value = Store[v];
      }

      return new Tuple<int, bool>(value, ok);
    }

    public int Set(string key)
    {
      var ok = Store.ContainsKey(key);
      int value = 0;
      if (!ok)
      {
        value = Count;
        Store.Add(key, value);
        Count++;
        return value;
      }

      return Store[key];
    }

    public Tuple<int,  int> SetLCL(string v, int d)
    {
      var res = GetLCL(v, d);

      if (!res.Item3)
      {
        var index = Set(v);
        var depth = Depth;

        return new Tuple<int, int>(index, depth);
      }

      return new Tuple<int, int>(res.Item1, res.Item2);
    }

    public Tuple<int, int, bool> GetLCL(string v, int d)
    {
      var indexTuple = Get(v);
      if (indexTuple.Item2)
      {
        return new Tuple<int, int, bool>(indexTuple.Item1, d - Depth, indexTuple.Item2);
      }

      if (Upper != null)
      {
        var upperTuple = Upper.GetLCL(v, d);
        return upperTuple;
      }

      return new Tuple<int, int, bool>(-1, 0, false);
    }
  }

  internal class Scope
  {
    public IStatement Self { get; set; }
    public AstProgram Program { get; set; }
    public Scope Out { get; set; }
    public LocalTable LocalTable { get; set; }
    public int Line { get; set; }
  }

  internal class Generator
  {
    public AstProgram Program { get; set; }
    public List<InstructionSet> InstructionSets { get; set; }
    public int BlockCounter { get; set; }

    public Generator(AstProgram program)
    {
      Program = program;
    }

    public string GenerateByteCode(AstProgram program)
    {
      var scope = new Scope { Program = program, LocalTable = new LocalTable() };
      CompileStatements(program.Statements, scope, scope.LocalTable);

      var output = new StringBuilder();
      foreach (var instructionSet in InstructionSets)
      {
        output.Append(instructionSet.Compile());
      }

      return RemoveEmptyLine(output.ToString()).Trim();
    }

    private void CompileStatements(List<IStatement> statements, Scope scope, LocalTable localTable)
    {
      var instructionSet = new InstructionSet { Label = new Label { Name = InstructionType.Program } };

      foreach (var statement in statements)
      {
        CompileStatement(instructionSet, statement, scope, localTable);
      }

      EndInstructions(instructionSet);
      InstructionSets.Add(instructionSet);
    }

    private void CompileStatement(InstructionSet instructionSet, IStatement statement, Scope scope, LocalTable localTable)
    {
      scope.Line++;
      if (statement is ExpressionStatement)
      {
        CompileExpression(instructionSet, ((ExpressionStatement)statement).Expression, scope, localTable);
      }
      else if (statement is DefStatement)
      {
        instructionSet.Define(InstructionType.PutSelf);
        instructionSet.Define(InstructionType.PutString, $"\"{((DefStatement)statement).Name.Value}\"");
        if (((DefStatement)statement).Receiver is SelfExpression)
        {
          instructionSet.Define(InstructionType.DefSingletonMethod, ((DefStatement)statement).Parameters);
        }
        else if (((DefStatement)statement).Receiver == null)
        {
          instructionSet.Define(InstructionType.DefMethod, ((DefStatement)statement).Parameters);
        }
        CompileDefStmt(statement as DefStatement, scope);
      }
      else if (statement is AssignStatement)
      {
        var stmt = statement as AssignStatement;
        CompileAssignStmt(instructionSet, stmt, scope, localTable);
      }
      else if (statement is ClassStatement)
      {
        var stmt = statement as ClassStatement;
        instructionSet.Define(InstructionType.PutSelf);
        if (stmt.SuperClass != null)
        {
          instructionSet.Define(InstructionType.DefClass, stmt.Name.Value, stmt.SuperClass.Value);
        }
        else
        {
          instructionSet.Define(InstructionType.DefClass, stmt.Name.Value);
        }

        instructionSet.Define(InstructionType.Pop);
        CompileClassStmt(stmt, scope);
      }
      else if (statement is ReturnStatement)
      {
        var stmt = statement as ReturnStatement;
        CompileExpression(instructionSet, stmt.ReturnValue, scope, localTable);
        EndInstructions(instructionSet);
      }
      else if (statement is RequireRelativeStatement)
      {
        var stmt = statement as RequireRelativeStatement;
        instructionSet.Define(InstructionType.RequireRelative, stmt.FilePath);
      }
    }

    private void CompileClassStmt(ClassStatement stmt, Scope scope)
    {
      scope = new Scope { Out = scope, LocalTable = new LocalTable(), Self = stmt, Line = 0 };
      var instructionSet = new InstructionSet();
      instructionSet.SetLabel($"{InstructionType.LabelDefClass}:{stmt.Name.Value}");

      CompileBlockStatement(instructionSet, stmt.Body, scope, scope.LocalTable);
      instructionSet.Define(InstructionType.Leave);
      InstructionSets.Add(instructionSet);
    }

    private void CompileAssignStmt(InstructionSet instructionSet, AssignStatement statement, Scope scope, LocalTable localTable)
    {
      CompileExpression(instructionSet, statement.Value, scope, localTable);

      if (statement.Name is Identifier)
      {
        var idtfr = statement.Name as Identifier;
        var indexDepth = localTable.SetLCL(idtfr.Value, localTable.Depth);
        instructionSet.Define(InstructionType.SetLocal, indexDepth.Item1, indexDepth.Item2);
      }
      else if (statement.Name is InstanceVariable)
      {
        var iv = statement.Name as InstanceVariable;
        instructionSet.Define(InstructionType.SetInstanceVariable, iv.Value);
      }
      else if (statement.Name is Constant)
      {
        instructionSet.Define(InstructionType.SetConstant, statement.Name as Constant);
      }
    }

    private void CompileDefStmt(DefStatement stmt, Scope scope)
    {
      scope = new Scope { Out = scope, LocalTable = new LocalTable(), Self = stmt, Line = 0 };
      var instructionSet = new InstructionSet();
      instructionSet.SetLabel($"{InstructionType.LabelDef}:{stmt.Name.Value}");

      for (int i = 0; i < stmt.Parameters.Count; i++)
      {
        scope.LocalTable.SetLCL(stmt.Parameters[i].Value, scope.LocalTable.Depth);
      }

      CompileBlockStatement(instructionSet, stmt.BlockStatement, scope, scope.LocalTable);
      EndInstructions(instructionSet);
      InstructionSets.Add(instructionSet);
    }

    private void CompileExpression(InstructionSet instructionSet, IExpression exp, Scope scope, LocalTable localTable)
    {
      if (exp is Identifier)
      {
        var idtfr = exp as Identifier;
        var indexDepth = localTable.GetLCL(idtfr.Value, localTable.Depth);
        if (indexDepth.Item3)
        {
          instructionSet.Define(InstructionType.GetLocal, indexDepth.Item1, indexDepth.Item2);
          return;
        }
        instructionSet.Define(InstructionType.Send, idtfr.Value, 0);
      }
      else if (exp is Constant)
      {
        instructionSet.Define(InstructionType.GetConstant, ((Constant)exp).Value);
      }
      else if (exp is InstanceVariable)
      {
        instructionSet.Define(InstructionType.GetInstanceVariable, ((InstanceVariable)exp).Value);
      }
      else if (exp is IntegerLiteral)
      {
        instructionSet.Define(InstructionType.PutObject, ((IntegerLiteral)exp).Value.ToString());
      }
      else if (exp is StringLiteral)
      {
        instructionSet.Define(InstructionType.PutString, $"\"{((StringLiteral)exp).Value}\"");
      }
      else if (exp is BooleanExpression)
      {
        instructionSet.Define(InstructionType.PutObject, ((BooleanExpression)exp).Value.ToString());
      }
      else if (exp is NilExpression)
      {
        instructionSet.Define(InstructionType.PutNull);
      }
      else if (exp is ArrayExpression)
      {
        foreach (var elem in ((ArrayExpression)exp).Elements)
        {
          CompileExpression(instructionSet, elem, scope, localTable);
        }
        instructionSet.Define(InstructionType.NewArray, ((ArrayExpression)exp).Elements.Count);
      }
      else if (exp is HashExpression)
      {
        foreach (var data in ((HashExpression)exp).Data)
        {
          instructionSet.Define(InstructionType.PutString, $"\"{data.Key}\"");
          CompileExpression(instructionSet, data.Value, scope, localTable);
        }
        instructionSet.Define(InstructionType.NewHash, ((HashExpression)exp).Data.Count * 2);
      }
      else if (exp is InfixExpression)
      {
        CompileInfixExpression(instructionSet, exp as InfixExpression, scope, localTable);
      }
      else if (exp is PrefixExpression)
      {
        var prefix = exp as PrefixExpression;
        switch (prefix.Operator)
        {
          case "!":
            CompileExpression(instructionSet, prefix.Right, scope, localTable);
            instructionSet.Define(InstructionType.Send, prefix.Operator, 0);
            break;
          case "-":
            instructionSet.Define(InstructionType.PutObject, 0);
            CompileExpression(instructionSet, prefix.Right, scope, localTable);
            instructionSet.Define(InstructionType.Send, prefix.Operator, 1);
            break;
        }
      }
      else if (exp is IfExpression)
      {
        CompileIfExpression(instructionSet, exp as IfExpression, scope, localTable);
      }
      else if (exp is SelfExpression)
      {
        instructionSet.Define(InstructionType.PutSelf);
      }
      else if (exp is YieldExpression)
      {
        instructionSet.Define(InstructionType.PutSelf);
        var yield = exp as YieldExpression;
        foreach (var arg in yield.Arguments)
        {
          CompileExpression(instructionSet, arg, scope, localTable);
        }
        instructionSet.Define(InstructionType.InvokeBlock, yield.Arguments.Count);
      }
      else if (exp is CallExpression)
      {
        var call = exp as CallExpression;
        CompileExpression(instructionSet, call.Receiver, scope, localTable);
        foreach (var arg in call.Arguments)
        {
          CompileExpression(instructionSet, arg, scope, localTable);
        }

        if (call.Block != null)
        {
          var newTable = new LocalTable { Store = new Dictionary<string, int>(), Depth = localTable.Depth + 1 };
          newTable.Upper = localTable;
          var blockIndex = BlockCounter;
          BlockCounter++;
          CompileBlockArgExpression(blockIndex, call, scope, newTable);
          instructionSet.Define("send", call.Method, call.Arguments.Count, $"block:{blockIndex}");
          return;
        }
        instructionSet.Define("send", call.Method, call.Arguments.Count);
      }
    }

    private void CompileBlockArgExpression(int index, CallExpression exp, Scope scope, LocalTable table)
    {
      var instructionSet = new InstructionSet();
      instructionSet.SetLabel($"{InstructionType.Block}:{index}");

      for (int i = 0; i < exp.Arguments.Count; i++)
      {
        table.Set(exp.BlockArguments[i].Value);
      }

      CompileBlockStatement(instructionSet, exp.Block, scope, table);
      EndInstructions(instructionSet);
      InstructionSets.Add(instructionSet);
    }

    private void CompileIfExpression(InstructionSet instructionSet, IfExpression exp, Scope scope, LocalTable table)
    {
      CompileExpression(instructionSet, exp.Condition, scope, table);
      var anchor1 = new Anchor();
      instructionSet.Define(InstructionType.BranchUnless, anchor1);

      CompileBlockStatement(instructionSet, exp.Consequence, scope, table);

      anchor1.Line = instructionSet.Count + 1;

      if (exp.Alternative == null)
      {
        anchor1.Line--;
        instructionSet.Define(InstructionType.PutNull);
        return;
      }

      var anchor2 = new Anchor();
      instructionSet.Define(InstructionType.Jump, anchor2);

      CompileBlockStatement(instructionSet, exp.Alternative, scope, table);

      anchor2.Line = instructionSet.Count;
    }

    private void CompileInfixExpression(InstructionSet instructionSet, InfixExpression node, Scope scope, LocalTable table)
    {
      CompileExpression(instructionSet, node.Left, scope, table);
      CompileExpression(instructionSet, node.Right, scope, table);
      instructionSet.Define(InstructionType.Send, node.Operator, "1");
    }

    private void CompileBlockStatement(InstructionSet instructionSet, BlockStatement stmt, Scope scope, LocalTable table)
    {
      foreach (var s in stmt.Statements)
      {
        CompileStatement(instructionSet, s, scope, table);
      }
    }

    private void EndInstructions(InstructionSet instructionSet)
    {
      instructionSet.Define(InstructionType.Leave);
    }

    private string RemoveEmptyLine(string s)
    {
      return Regex.Replace(s, "\n+", "\n");
    }
  }
}
