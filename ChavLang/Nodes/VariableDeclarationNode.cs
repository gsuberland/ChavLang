using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class VariableDeclarationNode : NodeBase
    {
        public string Type
        {
            get;
            protected set;
        }

        public string Name
        {
            get;
            protected set;
        }

        public string Value
        {
            get;
            protected set;
        }

        public VariableDeclarationNode(NodeBase parent, string type, string name, string value) : base(parent)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(11 + Name.Length);
            sb.Append("VAR ");
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append(" = ");
            sb.Append(Value ?? "<DEFAULT>");

            return sb.ToString();
        }
    }
}
