using ChavLang.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChavLang.Nodes
{
    public abstract class NodeBase
    {
        private readonly List<NodeBase> _children = new List<NodeBase>();
        private NodeBase _parent;

        public NodeBase(NodeBase parent)
        {
            _parent = parent;
        }

        public NodeBase Parent
        {
            get => _parent;
            protected internal set { _parent = value; }
        }

        public IEnumerable<NodeBase> Children
        {
            get => _children;
        }

        /// <summary>
        /// Adds a child node to this node, and sets its parent property to this node.
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(NodeBase node)
        {
            node.Parent = this;
            _children.Add(node);
        }

        public override string ToString()
        {
            return $"[{this.GetType().Name}]";
        }
    }
}
