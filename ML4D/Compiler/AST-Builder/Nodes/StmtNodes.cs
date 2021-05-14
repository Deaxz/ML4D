using System.Collections.Generic;

namespace ML4D.Compiler.Nodes
{
    public class AssignNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public ExpressionNode Right { get; set; }
        
        public AssignNode(string id, ExpressionNode right)
        {
            ID = id;
            Right = right;
        }

        public override List<Node> GetChildren()
        {
            return new List<Node>() {Right};
        }
    }
    
    public class WhileNode : Node
    {
        public ExpressionNode Predicate { get; set; }
        public LinesNode Body { get; set; }
        
        public WhileNode(ExpressionNode predicate, LinesNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>();
            children.Add(Predicate);
            foreach (Node node in Body.lines)
                children.Add(node);
            return children;
        }
    }

    public class FunctionStmtNode : FunctionNode
    {
        public FunctionStmtNode(string id) : base(id) {}
    }
    
    public class ReturnNode : Node
    {
        public string Type { get; set; }
        public ExpressionNode? Inner { get; set; }
        public ReturnNode() {}
        public ReturnNode(ExpressionNode inner)
        {
            Inner = inner;
        }
        
        public override List<Node> GetChildren()
        {
            if (Inner is not null)
                return new List<Node>() {Inner};
            return new List<Node>();
        }
    }
}