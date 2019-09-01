using ChavLang.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class UIntNode : NodeBase
    {
        public uint Value
        {
            get;
            protected set;
        }

        public UIntNode(NodeBase parent, uint value) : base(parent)
        {
            Value = value;
        }
    }
}
