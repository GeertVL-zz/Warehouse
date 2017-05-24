using System;
using System.Collections.Generic;
using System.Text;
using warehouselang.Bytecode;

namespace warehouselang.Vm
{
  using StackTrace = Int32;
  using IsTable = Dictionary<string, List<InstructionSet>>;
  using FileName = String;
  using ErrorMessage = String;
  using StandardLibraryInitMethod = Func<Vm>;

  internal class Vm
  {
    public CallFrameStack CallFrameStack { get; set; }
    public int Cfp { get; set; }
    public Stack Stack { get; set; }
    public int Sp { get; set; }
    public Dictionary<string, Pointer> Constants { get; set; }
    public Dictionary<LabelType, IsTable> IsTables { get; set; }
    public Dictionary<FileName, IsIndexTable> MethodIsIndexTables { get; set; }
    public Dictionary<FileName, IsIndexTable> ClassIsIndexTables { get; set; }
    public Dictionary<FileName, Dictionary<string, InstructionSet>> BlockTables { get; set; }
    public string FileDir { get; set; }
    public List<string> Args { get; set; }

    public Vm(string fileDir, params string[] args)
    {
      var s = new Stack();

    }
  }

  internal class IsIndexTable
  {
    public Dictionary<string, int> Data { get; set; }

  }

  internal class Stack
  {
    public List<Pointer> Data { get; set; }
    public Vm Vm { get; set; }
  }
}
