using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class FunctionNode : NodeBase
    {
        public string Name
        {
            get;
            protected set;
        }

        public IEnumerable<FunctionParameter> Parameters
        {
            get;
            protected set;
        }

        public FunctionNode(ProgramNode parent, string name, IEnumerable<FunctionParameter> parameters) : base(parent)
        {
            Name = name;
            Parameters = parameters;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(11 + Name.Length);
            sb.Append("FUNCTION ");
            sb.Append(Name);
            sb.Append("(");
            bool first = true;
            foreach (FunctionParameter fp in Parameters)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                sb.Append(fp.Type);
                sb.Append(" ");
                sb.Append(fp.Name);

                first = false;
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}
