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
    
    // Base expression node
    public abstract class ExpressionNode : Node
    {
        public string Type { get; set; }
    }
    
    // Infix operator node
    public class InfixExpressionNode : ExpressionNode // TODO rename to BinaryExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public override List<Node> GetChildren()
        {
            return new List<Node>() {Left, Right};
        }
    }
    
    // Unary operator node
    public class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Inner { get; set; }
        public override List<Node> GetChildren()
        {
            return new List<Node>() {Inner};
        }
    }
}


