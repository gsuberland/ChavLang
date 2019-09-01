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

        /// <summary>
        /// Throws an exception if the parent-child relationship of any node in the program has become corrupted.
        /// </summary>
        public void ValidateTree()
        {
            var remainingNodes = new Stack<NodeBase>();
            remainingNodes.Push(this);
            while (remainingNodes.Count > 0)
            {
                var currentNode = remainingNodes.Pop();
                foreach (var childNode in currentNode.Children)
                {
                    if (childNode.Parent != currentNode)
                    {
                        throw new Exception("[BUG] Node parent structure corrupted.");
                    }
                    remainingNodes.Push(childNode);
                }
            }
        }

        /// <summary>
        /// Get a human-readable tree representation of the current program.
        /// </summary>
        /// <returns></returns>
        public string GetProgramTreeString()
        {
            ValidateTree();

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
