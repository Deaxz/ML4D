using System;
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
            return new List<Node>() { Init };
        }
    }
    
    public class TensorDCLNode : DCLNode // TODO, make dis
    {
        public TensorInit Init { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        
        public TensorDCLNode(string type, string id, int rows, int columns, TensorInit init) : base(type, id)
        {
            Rows = rows;
            Columns = columns;
            Init = init;
        }

        public double GetElement(int row, int column)
        {
            return Init.Elements[(row - 1) * Columns + column - 1];
        }
        
        public override List<Node> GetChildren()
        {
            return new List<Node>() { Init };
        }
    }

    public class TensorInit : Node
    {
        public List<double> Elements = new List<double>();
        public override List<Node> GetChildren() { return new List<Node>(); }
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