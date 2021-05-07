using System.Collections.Generic;

namespace ML4D.Compiler.Nodes
{
    public abstract class Node
    {
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
        public bool Parenthesized { get; set; }
        public string Type { get; set; }
        public string Symbol { get; set; }
    }
    
    // Infix operator node
    public abstract class InfixExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public InfixExpressionNode(string symbol)
        {
            Symbol = symbol;
        }
        public override List<Node> GetChildren()
        {
            return new List<Node>() {Left, Right};
        }
    }
    
    // Unary operator node
    public abstract class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Inner { get; set; }
        public UnaryExpressionNode(string symbol)
        {
            Symbol = symbol;
        }
        public override List<Node> GetChildren()
        {
            return new List<Node>() {Inner};
        }
    }
    
    // Base function node
    public abstract class FunctionNode : ExpressionNode
    {
        public string ID { get; set; }
        public List<Node> Arguments = new List<Node>();
        public FunctionNode(string id)
        {
            ID = id;
        }
        
        public override List<Node> GetChildren() 
        {
            return Arguments;
        }
    }
}