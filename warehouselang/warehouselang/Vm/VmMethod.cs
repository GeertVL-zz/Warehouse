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
      
    }

    public string Inspect()
    {
      throw new NotImplementedException();
    }
  }
}
