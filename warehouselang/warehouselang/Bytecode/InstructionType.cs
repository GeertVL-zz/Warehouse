namespace warehouselang.Bytecode
{
  internal class InstructionType
  {
    // label types
    public const string LabelDef = "Def";
    public const string LabelDefClass = "DefClass";
    public const string Block = "Block";
    public const string Program = "ProgramStart";

    // instruction actions
    public const string GetLocal = "getlocal";
    public const string GetConstant = "getconstant";
    public const string GetInstanceVariable = "getinstancevariable";
    public const string SetLocal = "setlocal";
    public const string SetConstant = "setconstant";
    public const string SetInstanceVariable = "setinstancevariable";
    public const string PutString = "putstring";
    public const string PutSelf = "putself";
    public const string PutObject = "putobject";
    public const string PutNull = "putnull";
    public const string NewArray = "newarray";
    public const string NewHash = "newhash";
    public const string BranchUnless = "branchunless";
    public const string Jump = "jump";
    public const string DefMethod = "def_method";
    public const string DefSingletonMethod = "def_singleton_method";
    public const string DefClass = "def_class";
    public const string Send = "send";
    public const string InvokeBlock = "invokeblock";
    public const string Pop = "pop";
    public const string Leave = "leave";
    public const string RequireRelative = "require_relative";
  }
}
