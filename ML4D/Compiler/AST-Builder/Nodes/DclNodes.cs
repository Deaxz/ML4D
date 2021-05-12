using System.Collections.Generic;
using System.Linq;

namespace ML4D.Compiler.Nodes
{
    public abstract class DCLNode : Node
    {
        public string Type { get; set; }
        public string ID { get; set; }

        protected DCLNode(string type, string id)
        {
            Type = type;
            ID = id;
        }
    }
    
    public class VariableDCLNode : DCLNode
    {
        public ExpressionNode Init { get; set; }

        public VariableDCLNode(string type, string id, ExpressionNode init) : base(type, id)
        {
            Init = init;
        }

        public override List<Node> GetChildren()
        {
            return new List<Node>() {Init};
        }
    }

    public class FunctionDCLNode : DCLNode
    {
        public List<FunctionArgumentNode> Arguments = new List<FunctionArgumentNode>();        
        public LinesNode Body { get; set; }

        public FunctionDCLNode(string type, string id) : base(type, id) {}

        public override List<Node> GetChildren()
        { 
            return Arguments.Concat(Body.lines).ToList();
        }
    }
    
    public class FunctionArgumentNode : DCLNode
    {
        public FunctionArgumentNode(string type, string id) : base(type, id) {}

        public override List<Node> GetChildren() { return new List<Node>(); }
    }
}