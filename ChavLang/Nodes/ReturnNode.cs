using ChavLang.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class ReturnNode : NodeBase
    {
        /// <summary>
        /// A return node that does not return a value.
        /// </summary>
        /// <param name="parent"></param>
        public ReturnNode(NodeBase parent) : base(parent)
        {
        }

        /// <summary>
        /// A return node that returns a specified value.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="value"></param>
        public ReturnNode(NodeBase parent, NodeBase value) : this(parent)
        {
            AddChild(value);
        }

        public override string ToString()
        {
            return @"RETURN";
        }
    }
}
