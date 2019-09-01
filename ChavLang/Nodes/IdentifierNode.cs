using ChavLang.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class IdentifierNode : NodeBase
    {
        public string Name
        {
            get;
            protected set;
        }

        public IdentifierNode(NodeBase parent, string name) : base(parent)
        {
            Name = name;
        }
    }
}
