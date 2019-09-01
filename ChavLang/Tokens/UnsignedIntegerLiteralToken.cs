using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Tokens
{
    public class UnsignedIntegerLiteralToken : TokenBase
    {
        public uint Value
        {
            get;
        }

        public UnsignedIntegerLiteralToken(string contents) : base(contents)
        {
            if (!contents.EndsWith('u'))
            {
                throw new LexingException("[BUG] Tried to construct unsigned integer literal without 'u' suffix.");
            }
            Value = uint.Parse(contents.Substring(0, contents.Length - 1));
        }
    }
}
