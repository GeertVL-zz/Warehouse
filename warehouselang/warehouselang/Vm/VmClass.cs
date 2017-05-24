using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Vm
{
  class VmClassHelper
  {
    public static RVmClass CreateClass()
    {
      return new RVmClass
      {
        Name = "Class",
        Methods = new VmEnvironment(),
        ClassMethods = new VmEnvironment(),
        Constants = new Dictionary<string, Pointer>()
      };
    }

    public static RVmClass CreateObject()
    {
      return new RVmClass
      {
        Name = "Object",
        Class = CreateClass(),
        Methods = new VmEnvironment(),
        ClassMethods = new VmEnvironment(),
        Constants = new Dictionary<string, Pointer>()
      };
    }

    public static VmEnvironment CreateGlobalMethods()
    {
      
    }

    public static VmEnvironment CreateClassMethods()
    {
      
    }

    private void BuiltInGlobalMethods()
    {
      
    }
  }

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
    public RVmClass()
    {
      
    }

    public RVmClass(string name, bool isModule)
    {
      Name = name;
      Methods = new VmEnvironment();
      ClassMethods = new VmEnvironment();
      Class = VmClassHelper.CreateClass();
      PseudoSuperClass = VmClassHelper.CreateObject();
      SuperClass = VmClassHelper.CreateObject();
      Constants = new Dictionary<string, Pointer>();
      IsModule = isModule;
    }
  }

  class VmBaseClass : IVmClass
  {
    public string Name { get; set; }
    public VmEnvironment Methods { get; set; }
    public VmEnvironment ClassMethods { get; set; }
    public RVmClass PseudoSuperClass { get; set; }
    public RVmClass SuperClass { get; set; }
    public RVmClass Class { get; set; }
    public bool Singleton { get; set; }
    public bool IsModule { get; set; }
    public Dictionary<string, Pointer> Constants { get; set; }
    public IVmClass Scope { get; set; }


    public string ObjectType()
    {
      return ObjectTypes.ClassObject;
    }

    public string Inspect()
    {
      if (IsModule)
      {
        return $"<Module:" + Name + ">";
      }

      return $"<Class:" + Name + ">";
    }

    public IVmClass ReturnClass()
    {
      return Class;
    }

    public IVmObject LookupClassMethod(string name)
    {
      (var method, var ok) = ClassMethods.Get(name);
      if (!ok)
      {
        if (SuperClass != null)
        {
          return SuperClass.LookupClassMethod(name);
        }
        if (Class != null)
        {
          return Class.LookupClassMethod(name);
        }

        return null;
      }

      return method;
    }

    public IVmObject LookupInstanceMethod(string name)
    {
      (var method, var ok) = Methods.Get(name);
      if (!ok)
      {
        if (SuperClass != null)
        {
          return SuperClass.LookupInstanceMethod(name);
        }
        if (Class != null)
        {
          return Class.LookupInstanceMethod(name);
        }

        return null;
      }

      return method;
    }

    public Pointer LookupConstant(string name, bool findInScope)
    {
      var ok = Constants.TryGetValue(name, out var constant);
      if (!ok)
      {
        if (findInScope && Scope != null)
        {
          return Scope.LookupConstant(name, true);
        }
        if (SuperClass != null)
        {
          return SuperClass.LookupConstant(name, false);
        }

        return null;
      }

      return constant;
    }

    public string ReturnName()
    {
      return Name;
    }

    public IVmClass ReturnSuperClass()
    {
      return PseudoSuperClass;
    }

    public void SetSingletonMethod(string name, VmMethod method)
    {
      if (PseudoSuperClass.Singleton)
      {
        PseudoSuperClass.ClassMethods.Set(name, method);
      }

      var cls = new RVmClass(Name + "singleton", false);
      cls.Singleton = true;
      cls.ClassMethods.Set(name, method);
      cls.SuperClass = SuperClass;
      cls.Class = VmClassHelper.CreateClass();
      SuperClass = cls;
    }
  }
}
