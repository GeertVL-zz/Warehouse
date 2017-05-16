using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace tests.Lexer
{
  public class LexerTests
  {
    [Fact]
    public void TestNextToken()
    {
      var input = new StringBuilder();
      input.AppendLine("five = 5;");
      input.AppendLine("ten = 10;");
      input.AppendLine("");
      input.AppendLine("class Person");
      input.AppendLine("  def initialize(a)");
      input.AppendLine("    @a = a;");
      input.AppendLine("  end");
      input.AppendLine("");
      input.AppendLine("  def add(x, y)");
      input.AppendLine("    x + y;");
      input.AppendLine("  end");
      input.AppendLine("");
      input.AppendLine("  def ten()");
      input.AppendLine("    self.add(1, 9);");
      input.AppendLine("  end");
      input.AppendLine("");

    }
  }
}
