using ChavLang.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class IntNode : NodeBase
    {
        public int Value
        {
            get;
            protected set;
        }

        public IntNode(NodeBase parent, int value) : base(parent)
        {
            Value = value;
        }
    }
}
