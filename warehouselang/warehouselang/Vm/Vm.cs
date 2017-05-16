using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Vm
{
  internal class Vm
  {
    public CallFrameStack CallFrameStack { get; set; }
    public int Cfp { get; set; }
    public Stack Stack { get; set; }
    public int Sp { get; set; }
    public Dictionary<string, Pointer> Constants { get; set; }
    public Dictionary<LabelType, IsTable> IsTables { get; set; }
  }
}
