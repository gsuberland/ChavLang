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
        private readonly NodeBase _parent;

        public NodeBase(NodeBase parent)
        {
            _parent = parent;
        }

        public NodeBase Parent
        {
            get => _parent;
        }

        public IEnumerable<NodeBase> Children
        {
            get => _children;
        }

        public void AddChild(NodeBase node)
        {
            _children.Add(node);
        }

        public override string ToString()
        {
            return $"[{this.GetType().Name}]";
        }
    }
}
