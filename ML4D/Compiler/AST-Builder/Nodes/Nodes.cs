#nullable enable
using System;
using System.Collections.Generic;

namespace ML4D.Compiler
{
    public abstract class Node
    {
        // get children return list of children
        public abstract List<Node> GetChildren();
    }
    
    // Program/Block node
    public class LinesNode : Node
    {
        public List<Node> lines = new List<Node>();
        public override List<Node> GetChildren()
        {
            return lines;
        }
    }
    
    // Base expression nodes 
    public abstract class ExpressionNode : Node
    {
    }
    
    // Infix operator node
    public class InfixExpressionNode : ExpressionNode // TODO rename to BinaryExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>() {Left, Right};
            return children;
        }
    }
    
    // Unary operator node
    public class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Inner { get; set; }
        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>() {Inner};
            return children;
        }
    }
}


