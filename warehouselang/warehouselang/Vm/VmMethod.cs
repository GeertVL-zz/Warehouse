using System;
using System.Collections.Generic;
using System.Text;
using warehouselang.Bytecode;

namespace warehouselang.Vm
{
  class VmMethod : IVmObject
  {
    public string Name { get; set; }
    public InstructionSet InstructionSet { get; set; }
    public int Argc { get; set; }

    public string ObjectType()
    {
      return ObjectTypes.MethodObject;
    }

    public string Inspect()
    {
      var output = new StringBuilder();
      output.Append("")
    }
  }

  class BuiltInMethod
  {
    public string Name { get; set; }
    public Func<IVmObject> BuiltInMethodBody { get; set; }
  }
}
