using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Vm
{
  using ObjectType = String;

  class ObjectTypes
  {
    public const string IntegerObject = "INTEGER";
    public const string ArrayObject = "ARRAY";
    public const string HashObject = "HASH";
    public const string StringObject = "STRING";
    public const string BooleanObject = "BOOLEAN";
    public const string NullObject = "NULL";
    public const string ErrorObject = "ERROR";
    public const string MethodObject = "METHOD";
    public const string ClassObject = "CLASS";
    public const string BaseObject = "BASE_OBJECT";
    public const string BuildInMethodObj = "BUILD_IN_METHOD";
  }

  interface IVmObject
  {
    ObjectType ObjectType();
    string Inspect();
  }

  class Pointer
  {
    public IVmObject Target { get; set; }
  }
}
