using System;
using System.Collections.Generic;

namespace warehouselang.Bytecode
{

  internal class Instruction
  {
    public string Action { get; set; }
    public List<string> Parameters { get; set; }
    public int Line { get; set; }
    public Anchor Anchor { get; set; }

    public Instruction()
    {
      Parameters = new List<string>();
    }

    public string Compile()
    {
      if (Anchor != null)
      {
        return $"{Line} {Action} {Anchor.Line}\n";
      }
      if (Parameters.Count > 0)
      {
        return $"{Line} {Action} {string.Join(" ", Parameters)}\n";
      }

      return $"{Line} {Action}\n";
    }
  }
}
