using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Vm
{
  class VmEnvironment
  {
    public Dictionary<string, IVmObject> Store { get; set; }
    public VmEnvironment Outer { get; set; }

    public VmEnvironment()
    {
      Store = new Dictionary<string, IVmObject>();
    }

    public (object, bool) GetCurrent(string name)
    {
      var ok = Store.TryGetValue(name, out IVmObject obj);
      return (obj, ok);
    }

    public (VmEnvironment, bool) GetValueLocation(string name)
    {
      var env = this;
      var ok = Store.ContainsKey(name);
      if (!ok && Outer != null)
      {
        (env, ok) = Outer.GetValueLocation(name);
      }

      return (env, ok);
    }

    public (IVmObject, bool) Get(string name)
    {
      var ok = Store.TryGetValue(name, out IVmObject obj);
      if (!ok && Outer != null)
      {
        (obj, ok) = Outer.Get(name);
      }

      return (obj, ok);
    }

    public IVmObject Set(string name, IVmObject val)
    {
      Store[name] = val;

      return val;
    }
  }
}
