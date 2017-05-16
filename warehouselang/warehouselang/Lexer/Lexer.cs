using System;
using System.Collections.Generic;
using System.Text;

namespace warehouselang.Lexer
{
  public class Lexer
  {
    private string _input;
    private int _position;
    private int _readPosition;
    private char _ch;
    private int _line;

    public Lexer(string input)
    {
      _input = input;
      ReadChar();
    }

    internal Token NextToken()
    {
      Token tok = null;

      SkipWhitespace();

      switch (_ch)
      {
        case '"':
        case '\'':
          tok = new Token { Type = TokenType.String, Literal = ReadString(_ch), Line = _line };
          break;
        case '=':
          if (PeekChar() == '=')
          {
            char currentByte = _ch;
            ReadChar();
            tok = new Token { Type = TokenType.Eq, Literal = currentByte.ToString(), Line = _line };
          }
          else
          {
            tok = new Token { Type = TokenType.Assign, Literal = _ch.ToString(), Line = _line };
          }
          break;
        case '-':
          if (PeekChar() == '-')
          {
            tok = new Token { Literal = "--", Line = _line, Type = TokenType.Decr };
            ReadChar();
            ReadChar();
          }
          else
          {
            tok = new Token { Type = TokenType.Minus, Literal = _ch.ToString(), Line = _line };
          }
          break;
        case '!':
          if (PeekChar() == '=')
          {
            var currentByte = _ch;
            ReadChar();
            tok = new Token { Type = TokenType.NotEq, Literal = currentByte.ToString() + _ch.ToString(), Line = _line };
          }
          else
          {
            tok = new Token { Type = TokenType.Bang, Literal = _ch.ToString(), Line = _line };
          }
          break;
        case '/':
          tok = new Token { Type = TokenType.Slash, Literal = _ch.ToString(), Line = _line };
          break;
        case '*':
          tok = new Token { Type = TokenType.Asterisk, Literal = _ch.ToString(), Line = _line };
          break;
        case '<':
          tok = new Token { Type = TokenType.Lt, Literal = _ch.ToString(), Line = _line };
          break;
        case '>':
          tok = new Token { Type = TokenType.Gt, Literal = _ch.ToString(), Line = _line };
          break;
        case ';':
          tok = new Token { Type = TokenType.Semicolon, Literal = _ch.ToString(), Line = _line };
          break;
        case '(':
          tok = new Token { Type = TokenType.LParen, Literal = _ch.ToString(), Line = _line };
          break;
        case ')':
          tok = new Token { Type = TokenType.RParen, Literal = _ch.ToString(), Line = _line };
          break;
        case ',':
          tok = new Token { Type = TokenType.Comma, Literal = _ch.ToString(), Line = _line };
          break;
        case '+':
          if (PeekChar() == '+')
          {
            tok = new Token { Type = TokenType.Incr, Literal = "++", Line = _line };
            ReadChar();
            ReadChar();
          }
          else
          {
            tok = new Token { Type = TokenType.Plus, Literal = _ch.ToString(), Line = _line };
          }
          break;
        case '{':
          tok = new Token { Type = TokenType.LBrace, Literal = _ch.ToString(), Line = _line };
          break;
        case '}':
          tok = new Token { Type = TokenType.RBrace, Literal = _ch.ToString(), Line = _line };
          break;
        case '[':
          tok = new Token { Type = TokenType.LBracket, Literal = _ch.ToString(), Line = _line };
          break;
        case ']':
          tok = new Token { Type = TokenType.RBracket, Literal = _ch.ToString(), Line = _line };
          break;
        case '.':
          tok = new Token { Type = TokenType.Dot, Literal = _ch.ToString(), Line = _line };
          break;
        case ':':
          tok = new Token { Type = TokenType.Colon, Literal = _ch.ToString(), Line = _line };
          break;
        case '|':
          tok = new Token { Type = TokenType.Bar, Literal = _ch.ToString(), Line = _line };
          break;
        case '#':
          tok = new Token();
          tok.Literal = AbsorbComment();
          tok.Type = TokenType.Comment;
          tok.Line = _line;
          break;
        case '\0':
          tok = new Token { Type = TokenType.Eof, Literal = string.Empty, Line = _line };
          break;
        default:
          if (IsLetter(_ch))
          {
            if ('A' <= _ch && _ch <= 'Z')
            {
              tok = new Token { Type = TokenType.Constant, Literal = ReadConstant(), Line = _line };
            }
            else
            {
              tok = new Token();
              tok.Literal = ReadIdentifier();
              tok.Type = Token.LookupIdent(tok.Literal);
              tok.Line = _line;
            }
          }
          else if (IsInstanceVariable(_ch))
          {
            if (IsLetter(PeekChar()))
            {
              tok = new Token { Line = _line };
              tok.Literal = ReadInstanceVariable();
              tok.Type = TokenType.InstanceVariable;
              tok.Line = _line;
            }
            else
            {
              tok = new Token { Type = TokenType.Illegal, Line = _line, Literal = _ch.ToString() };
            }
          }
          else if (IsDigit(_ch))
          {
            tok = new Token();
            tok.Literal = ReadNumber();
            tok.Type = TokenType.Int;
            tok.Line = _line;
          }
          else
          {
            tok = new Token { Type = TokenType.Illegal, Literal = _ch.ToString(), Line = _line };
          }
          break;
      }

      ReadChar();

      return tok;
    }

    private string ReadNumber()
    {
      var position = _position;
      while (IsDigit(_ch))
      {
        ReadChar();
      }

      return _input.Substring(position, _position - position);
    }

    private bool IsDigit(char ch)
    {
      return '0' <= _ch && _ch <= '9';
    }

    private string ReadInstanceVariable()
    {
      var position = _position;
      while (IsLetter(_ch) || IsInstanceVariable(_ch) || IsDigit(_ch))
      {
        ReadChar();
      }

      return _input.Substring(position, _position - position);
    }

    private bool IsInstanceVariable(char ch)
    {
      return _ch == '@';
    }

    private string ReadIdentifier()
    {
      var position = _position;
      while (IsLetter(_ch) || IsDigit(_ch))
      {
        ReadChar();
      }

      return _input.Substring(position, _position - position);
    }

    private string ReadConstant()
    {
      var position = _position;
      while (IsLetter(_ch) || IsDigit(_ch))
      {
        ReadChar();
      }

      return _input.Substring(position, _position - position);
    }

    private bool IsLetter(char ch)
    {
      return 'a' <= _ch && _ch <= 'z' || 'A' <= _ch && _ch <= 'Z' || _ch == '_';
    }

    private string AbsorbComment()
    {
      var position = _position;
      while (_ch != '\n' && _ch != 0)
      {
        ReadChar();
      }

      return _input.Substring(position, _position - position);
    }

    private char PeekChar()
    {
      if (_readPosition >= _input.Length)
      {
        return '\0';
      }

      return _input[_readPosition];
    }

    private string ReadString(char ch)
    {
      ReadChar();
      var position = _position;

      while (PeekChar() != _ch)
      {
        ReadChar();
      }

      ReadChar();
      var result = _input.Substring(position, _position - position);
      ReadChar();

      return result;
    }

    private void SkipWhitespace()
    {
      while (_ch == ' ' || _ch == '\t' || _ch == '\r' || _ch == '\n')
      {
        if (_ch == '\n')
        {
          _line++;
        }
        ReadChar();
      }
    }

    private void ReadChar()
    {
      if (_readPosition >= _input.Length)
      {
        _ch = '\0';
      }
      else
      {
        _ch = _input[_readPosition];
      }
      _position = _readPosition;
      _readPosition++;
    }
  }
}
