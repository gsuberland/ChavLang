using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Tokens
{
    public abstract class TokenBase
    {
        public TokenBase(string content)
        {
            Contents = content;
        }

        public string Contents
        {
            get;
            protected set;
        }

        public override string ToString()
        {
            return $"[{this.GetType().Name}] => {Contents}";
        }
    }
}
