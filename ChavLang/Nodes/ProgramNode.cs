using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Nodes
{
    public class ProgramNode : NodeBase
    {
        public ProgramNode() : base(null)
        {

        }

        public string GetProgramTreeString()
        {
            var output = new StringBuilder();

            var nodesToPrint = new Stack<Tuple<int, NodeBase>>();
            nodesToPrint.Push(new Tuple<int, NodeBase>(0, this));

            while (nodesToPrint.Count > 0)
            {
                (int depth, NodeBase currentNode) = nodesToPrint.Pop();

                // append the right number of tabs
                output.Append(new StringBuilder("\t".Length * depth).Insert(0, "\t", depth).ToString());
                // append the current node as a string
                output.AppendLine(currentNode.ToString());

                // now manage any child nodes
                var childNodesReversed = new List<NodeBase>(currentNode.Children);
                childNodesReversed.Reverse();
                foreach (NodeBase childNode in childNodesReversed)
                {
                    nodesToPrint.Push(new Tuple<int, NodeBase>(depth + 1, childNode));
                }
            }

            return output.ToString();
        }

        public override string ToString()
        {
            return "PROGRAM";
        }
    }
}
