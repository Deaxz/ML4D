#nullable enable
using System;
using System.Collections.Generic;

namespace ML4D.Compiler
{
    public abstract class Node
    {
        public virtual Node GetChildren()
        {
            return this.GetChildren();
        }

        public virtual void AddChildren(Node child)
        {
            this.AddChildren(child);
        }
    }
 
    public class LinesNode : Node
    {
        public List<Node> lines = new List<Node>();
    }
    
    public class VariableDCLNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public ExpressionNode? Init { get; set; }

        public VariableDCLNode(string type, string id)
        {
            Type = type;
            ID = id;
        }
    }
    
    public class FunctionDCLNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public ExpressionNode predicate { get; set; }
        public List<Node> Body { get; set; }
    }

    
}


