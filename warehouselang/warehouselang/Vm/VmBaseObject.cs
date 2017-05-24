using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Vm
{
  interface IVmBaseObject : IVmObject
  {
    IVmClass ReturnClass();
  }

  class RVmObject : IVmBaseObject
  {
    public RVmClass VmClass { get; set; }
    public VmEnvironment InstanceVariables { get; set; }
    public VmMethod InitializeMethod { get; set; }

    public string ObjectType()
    {
      return ObjectTypes.BaseObject;
    }

    public string Inspect()
    {
      return $"<Instance of: {VmClass.Name}>";
    }

    public IVmClass ReturnClass()
    {
      if (VmClass == null)
      {
        throw new Exception($"Object {Inspect()} doesn't have class.");
      }

      return VmClass;
    }
  }
}
