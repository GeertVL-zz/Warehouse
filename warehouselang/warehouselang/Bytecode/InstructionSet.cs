using System.Collections.Generic;
using System.Text;

namespace warehouselang.Bytecode
{
  internal class InstructionSet
  {
    public Label Label { get; set; }
    public List<Instruction> Instructions { get; set; }
    public int Count { get; set; }

    public InstructionSet()
    {
      Instructions = new List<Instruction>();
    }

    public void SetLabel(string name)
    {
      var l = new Label { Name = name };
      Label = l;
    }

    public void Define(string action, params object[] parameters)
    {
      var ps = new List<string>();
      var i = new Instruction { Action = action, Parameters = ps, Line = Count };
      foreach (var param in parameters)
      {
        if (param is string)
        {
          ps.Add(param.ToString());
        }
        else if (param is Anchor)
        {
          i.Anchor = (Anchor)param;
        }
        else if (param is int)
        {
          ps.Add(param.ToString());
        }
      }

      if (ps.Count > 0)
        i.Parameters = ps;

      Instructions.Add(i);
      Count++;
    }

    public string Compile()
    {
      var output = new StringBuilder();
      output.Append(Label.Compile());
      foreach (var instruction in Instructions)
      {
        output.Append(instruction.Compile());
      }

      return output.ToString();
    }
  }
}
