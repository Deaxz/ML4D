using System.Collections.Generic;
using System.Linq;

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
            return new List<Node>() { Right };
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
            List<Node> children = new List<Node>() { Predicate };
            return children.Concat(Body.lines).ToList();
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
                return new List<Node>() { Inner };
            return new List<Node>();
        }
    }
    
    public class IfElseChainNode : Node 
    {
        public List<IfNode> IfNodes = new List<IfNode>();
        public LinesNode? ElseBody = new LinesNode();
        
        public override List<Node> GetChildren()
        {
            if (ElseBody is null)
            {
                List<Node> children = new List<Node>();
                return children.Concat(IfNodes).ToList();  
            }
            return IfNodes.Concat(ElseBody.lines).ToList();
        }
    }
    
    public class IfNode : Node 
    {
        public ExpressionNode Predicate { get; set; }
        public LinesNode Body { get; set; }
        
        public IfNode(ExpressionNode predicate, LinesNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>() { Predicate };
            return children.Concat(Body.lines).ToList();
        }
    }

    public class ForNode : Node 
    {
        public VariableDCLNode Init { get; set; }
        public ExpressionNode Predicate { get; set; }
        public AssignNode Final { get; set; }
        public LinesNode Body { get; set; }

        public ForNode(VariableDCLNode init, ExpressionNode predicate, AssignNode final, LinesNode body)
        {
            Init = init;
            Predicate = predicate;
            Final = final;
            Body = body;
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>()
            {
                Init, Predicate, Final
            };
            return children.Concat(Body.lines).ToList();
        }
    }

    public class GradientsNode : Node 
    {
        public string tensorID { get; set; }
        public List<string> GradVariables = new List<string>();
        public List<string> GradTensors = new List<string>();
        public  LinesNode Body { get; set; }
        
        public GradientsNode(string tensorId, LinesNode body)
        {
            tensorID = tensorId;
            Body = body;
        }

        public override List<Node> GetChildren() { return new List<Node>(Body.lines); }
    }
}