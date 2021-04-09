using System.Collections.Generic;

namespace ML4D.Compiler
{
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

        public override List<Node> GetChildren()
        {
            if (Init is not null)
                return new List<Node>() {Init};
            return new List<Node>();
        }
    }

    public class FunctionDCLNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public List<FunctionArgumentNode> Arguments = new List<FunctionArgumentNode>();        
        public LinesNode Body { get; set; }

        public FunctionDCLNode(string type, string id)
        {
            Type = type;
            ID = id;
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>();
            foreach (FunctionArgumentNode node in Arguments)
                children.Add(node);
            foreach (Node node in Body.lines)
                children.Add(node);
            
            return children;
        }
    }
    
    public class FunctionArgumentNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }

        public FunctionArgumentNode(string type, string id)
        {
            Type = type;
            ID = id;
        }

        public override List<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}