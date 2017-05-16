using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Lexer
{
  enum TokenType
  {
    String,
    Eq,
    Assign,
    Decr,
    Minus,
    NotEq,
    Bang,
    Slash,
    Asterisk,
    Lt,
    Gt,
    Semicolon,
    LParen,
    RParen,
    Comma,
    Incr,
    Plus,
    LBrace,
    RBrace,
    LBracket,
    RBracket,
    Dot,
    Colon,
    Bar,
    Comment,
    Eof,
    Constant,
    InstanceVariable,
    Illegal,
    Int,
    Ident,
    Class,
    Def,
    RequireRelative,
    While,
    Self,
    Lte,
    Gte,
    Comp,
    Pow,
    End,
    Else,
    Do,
    Return
  }
}
