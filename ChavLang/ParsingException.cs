using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang
{
    class ParsingException : Exception
    {
        public ParsingException(string message) : base(message)
        {

        }
    }
}
