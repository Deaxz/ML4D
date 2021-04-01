#nullable enable
using System;
using System.Collections.Generic;

namespace ML4D.Compiler
{
    public abstract class Node
    {
    }
    
    // Program/Block node
    public class LinesNode : Node
    {
        public List<Node> lines = new List<Node>();
    }
    
    // Base expression nodes 
    public abstract class ExpressionNode : Node
    {
    }
    
    // Infix operator node
    public abstract class InfixExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
    }
    
    // Unary operator node
    public class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Inner { get; set; }
    }
    
    
    
    
    
}


