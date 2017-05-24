using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Vm
{
  interface IVmClass : IVmBaseObject
  {
    IVmObject LookupClassMethod(string name);
    IVmObject LookupInstanceMethod(string name);
    Pointer LookupConstant(string name, bool flag);
    string ReturnName();
    IVmClass ReturnSuperClass();
  }

  class RVmClass : VmBaseClass
  {
    
  }

  class VmBaseClass : IVmClass
  {
    public string Name { get; set; }
    public VmEnvironment Methods { get; set; }
    public VmEnvironment ClassMethods { get; set; }
    public RVmClass PseudoSuperClass { get; set; }
    public RVmClass SuperClass { get; set; }
    public RVmClass VmClass { get; set; }
    public bool Singleton { get; set; }
    public bool IsModule { get; set; }
    public Dictionary<string, Pointer> Constants { get; set; }
    public IVmClass Scope { get; set; }


    public string ObjectType()
    {
      throw new NotImplementedException();
    }

    public string Inspect()
    {
      throw new NotImplementedException();
    }

    public IVmClass ReturnClass()
    {
      throw new NotImplementedException();
    }

    public IVmObject LookupClassMethod(string name)
    {
      throw new NotImplementedException();
    }

    public IVmObject LookupInstanceMethod(string name)
    {
      throw new NotImplementedException();
    }

    public Pointer LookupConstant(string name, bool flag)
    {
      throw new NotImplementedException();
    }

    public string ReturnName()
    {
      throw new NotImplementedException();
    }

    public IVmClass ReturnSuperClass()
    {
      throw new NotImplementedException();
    }
  }
}
