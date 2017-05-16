namespace warehouselang.Bytecode
{
  internal class Label
  {
    public string Name { get; set; }

    public string Compile()
    {
      return $"<{Name}>\n";
    }
  }
}
