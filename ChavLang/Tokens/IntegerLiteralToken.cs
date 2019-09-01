using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Tokens
{
    public class IntegerLiteralToken : TokenBase
    {
        public int Value
        {
            get;
        }

        public IntegerLiteralToken(string contents) : base(contents)
        {
            Value = int.Parse(contents);
        }
    }
}
