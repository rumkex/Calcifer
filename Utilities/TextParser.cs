using System;
using System.Globalization;
using System.IO;
using OpenTK;

namespace Calcifer.Utilities
{
    public class ParserException : Exception
    {
        public ParserException(string message, int lineNumber)
            : base(message + " at line " + lineNumber)
        { }
    }
    public class TextParser
    {
        private TextReader reader;
        private string curLine;
        private int position;

        public bool DetectQuotes { get; set; }
        public bool AutoMultiline { get; set; }
        public int LineNumber { get; private set; }

        public TextParser(TextReader reader)
        {
            this.reader = reader;
        }

        public string NextLine()
        {
            LineNumber++;
            curLine = reader.ReadLine();
            position = 0;
            return curLine;
        }

        public string CurrentLine
        {
            get
        {
            return curLine;
        }
            
        }

        private string ReadToken()
        {
            if (curLine == null && NextLine() == null) throw new ParserException("Unexpected end of file", LineNumber);
            if (position >= curLine.Length)
                if (AutoMultiline) NextLine(); else throw new ParserException("Unexpected end of line", LineNumber);
            int tokenStart = position, tokenLength = 0;
            bool quotedToken = false;
            do
            {
                var c = curLine[position];
                if (char.IsWhiteSpace(c) && !quotedToken)
                {
                    if (tokenLength == 0)
                    {
                        tokenStart++;
                        continue;
                    }
                    break;
                }
                else if (c == '\"' && DetectQuotes)
                {
                    if (!quotedToken) tokenStart++;
                    quotedToken = !quotedToken;
                }
                else tokenLength++;
            } while (++position != curLine.Length);
            return tokenLength == 0 ? ReadToken() : curLine.Substring(tokenStart, tokenLength);
        }

        public string ReadLine()
        {
            if (curLine == null) NextLine();
            var line = (position == 0) ? curLine : NextLine();
            NextLine();
            return line;
        }

        public string ReadString()
        {
            return ReadToken();
        }

        public int ReadInt()
        {
            int result;
            var token = ReadToken();
            if (!int.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture,  out result)) throw new ParserException("Expected integer, got " + token, LineNumber);
            return result;
        }

        public float ReadFloat()
        {
            float result;
            var token = ReadToken();
            if (!float.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) throw new ParserException("Expected float, got " + token, LineNumber);
            return result;
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }
    }
}