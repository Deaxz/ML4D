using System;
using ML4D.Compiler.Exceptions;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler.ASTVisitors
{
    public class TypeCheckSymbolTableVisitor : ASTVisitor
    {
        private SymbolTable SymbolTable { get; }

        public TypeCheckSymbolTableVisitor(SymbolTable symbolTable)
        {
            SymbolTable = symbolTable;
        }

        // Declaration
        public override void Visit(VariableDCLNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
                SymbolTable.Insert(node.ID, node.Type, false);
            else
                throw new VariableAlreadyDeclaredException(node, 
                    $"The variable \"{node.ID}\" could not be declared, as it has already been declared in the current or parent scope.");
            
            base.Visit(node);
            
            if (node.Type == node.Init.Type || node.Type == "double" && node.Init.Type == "int")
                return;
            throw new VariableInitialisationException(node, 
                "Failed to initialise in declaration. The expression has an incorrect type.");                    
        }
        
        public override void Visit(FunctionDCLNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
                SymbolTable.Insert(node.ID, node.Type, true);
            else
                throw new FunctionAlreadyDeclaredException(node, 
                    $"The function \"{node.ID}\" has already been declared in the current or parent scope.");
            
            SymbolTable.OpenScope("function");
            base.Visit(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(FunctionArgumentNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
                SymbolTable.Insert(node.ID, node.Type, false);
            else
                throw new VariableAlreadyDeclaredException(
                    $"The variable \"{node.ID}\" has already been declared. And would hide the variable in the parent scope if declared inside the function.");
        }

        public override void Visit(TensorDCLNode node)
        {
            base.Visit(node);
        }

        public override void Visit(TensorInitNode node)
        {
            base.Visit(node);
        }

        // Statement      
        public override void Visit(AssignNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
                throw new VariableNotDeclaredException(
                    $"The variable \"{node.ID}\" cannot be assigned, as it has not been declared.");
            
            base.Visit(node);
            node.Type = SymbolTable.Retrieve(node.ID).Type;
            
            if (node.Type == node.Right.Type || node.Type == "double" && node.Right.Type == "int")
                return;
            throw new VariableAssignmentException(node, 
                "Failed to assign variable. The expression has an incorrect type.");
        }
        
        public override void Visit(WhileNode node)
        {
            SymbolTable.OpenScope("while");
            base.Visit(node);
            SymbolTable.CloseScope();
        }
        
        public override void Visit(ReturnNode node)
        {
            base.Visit(node);
            if (!SymbolTable.ScopeList().Contains("function"))
                throw new ReturnOutsideFunctionException(node, 
                    "Return was called outside function scope, which is not allowed");
            if (node.Inner is not null)
                node.Type = node.Inner.Type;
        }

        public override void Visit(FunctionNode node)
        {
            Symbol functionDCL = SymbolTable.Retrieve(node.ID);
            
            if (functionDCL is null)
                throw new FunctionNotDeclaredException(node, 
                    $"The function \"{node.ID}\" cannot be called, as it is not declared");
            if (!functionDCL.IsFunction)
                throw new InvalidCallToVariable(node, 
                    $"The identifier \"{node.ID}\" refers to a variable, not a function");
            
            base.Visit(node);
            node.Type = functionDCL.Type;
        }

        public override void Visit(IfElseChainNode node)
        {
            base.Visit(node);
        }

        public override void Visit(ForNode node)
        {
            base.Visit(node);
        }

        public override void Visit(GradientsNode node)
        {
            base.Visit(node);
        }

        // Expression
        public override void Visit(IDNode node)
        {
            Symbol variableDCL = SymbolTable.Retrieve(node.ID);
            if (variableDCL is null)
                throw new VariableNotDeclaredException(node, 
                    $"The variable \"{node.ID}\" cannot be used, as it has not been declared.");
            node.Type = variableDCL.Type;
        }
        
        public override void Visit(InfixExpressionNode node)
        {
            base.Visit(node);

            switch (node)
            {
                // Arithmetic
                case AdditionNode:
                case SubtractionNode:
                case MultiplicationNode:
                case DivisionNode:
                case PowerNode:
                    
                    if (node.Left.Type == "bool" || node.Right.Type == "bool")
                        throw new InvalidOperandsException(node, 
                            "The operands of a arithmetic operator can only be int and double. It does not allow bool.");
                    else if (node.Left.Type == "int" && node.Right.Type == "int")
                        node.Type = "int";
                    else
                        node.Type = "double";
                    break;
                    
                // Relational
                case LessThanNode:
                case LessEqualThanNode:
                case GreaterThanNode:
                case GreaterEqualThanNode:
                case EqualNode:
                case NotEqualNode:
                    
                    if (node.Left.Type == "bool" || node.Right.Type == "bool")
                        throw new InvalidOperandsException(node,
                            "The operands of a relational operator can only be int and double. It does not allow bool.");
                    else
                        node.Type = "bool";
                    break;
                    
                // Boolean
                case AndNode:
                case OrNode:
                   
                    if (node.Left.Type != "bool" || node.Right.Type != "bool")
                        throw new InvalidOperandsException(node, 
                            "The operands of a bool operator can only be bool. It does not allow int or double.");
                    else
                        node.Type = "bool";
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        
        public override void Visit(UnaryExpressionNode node)
        {
            switch (node)
            {
                case NotNode:
                    VisitChildren(node);
                    
                    if (node.Inner.Type != "bool")
                        throw new InvalidOperandsException(node,
                            "The operand of a bool operator can only be bool. It does not allow int or double.");
                    else
                        node.Type = "bool";
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}