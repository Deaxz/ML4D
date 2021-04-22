using System;
using ML4D.Compiler.Exceptions;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler.ASTVisitors
{
    public class TypeCheckSymbolTableVisitor : ASTVisitor
    {
        private SymbolTable SymTable { get; }

        public TypeCheckSymbolTableVisitor(SymbolTable symTable)
        {
            SymTable = symTable;
        }

        // Declaration
        public override void Visit(VariableDCLNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                SymTable.Insert(node.ID, node.Type);
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
            if (SymTable.Retrieve(node.ID) is null)
                SymTable.Insert(node.ID, node.Type);
            else
                throw new FunctionAlreadyDeclaredException(node, 
                    $"The function \"{node.ID}\" has already been declared in the current or parent scope.");
            
            SymTable.OpenScope();
            base.Visit(node);
            // Umildbart håndteret af gcc.
            SymTable.CloseScope();
        }

        public override void Visit(FunctionArgumentNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                SymTable.Insert(node.ID, node.Type);
            else
                throw new VariableAlreadyDeclaredException(
                    $"The variable \"{node.ID}\" has already been declared. And would hide the variable in the parent scope if declared inside the function.");
        }
        
        // Statement      
        public override void Visit(AssignNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                throw new VariableNotDeclaredException(
                    $"The variable \"{node.ID}\" cannot be assigned, as it has not been declared.");
            
            base.Visit(node);
            node.Type = SymTable.Retrieve(node.ID).Type;
            
            if (node.Type == node.Right.Type || node.Type == "double" && node.Right.Type == "int")
                return;
            throw new VariableAssignmentException(node, 
                "Failed to assign variable. The expression has an incorrect type.");
        }
        
        public override void Visit(WhileNode node)
        {
            SymTable.OpenScope();
            base.Visit(node);
            SymTable.CloseScope();
        }

        public override void Visit(FunctionExprNode node)
        {
            if (SymTable.Retrieve(node.ID) is null)
                throw new FunctionNotDeclaredException(node, 
                    $"The function \"{node.ID}\" cannot be called, as it is not declared");

            base.Visit(node);
            // Umildbart håndteret af gcc.
        }
        
        // Expression
        public override void Visit(IDNode node)
        {
            Symbol variableDCL = SymTable.Retrieve(node.ID);
            if (variableDCL is null)
                throw new VariableNotDeclaredException(node, 
                    $"The variable \"{node.ID}\" cannot be used, as it has not been declared.");
            node.Type = variableDCL.Type;
        }
        
        public override void Visit(InfixExpressionNode node)
        {
            switch (node)
            {
                // Arithmetic
                case AdditionNode:
                case SubtractionNode:
                case MultiplicationNode:
                case DivisionNode:
                case PowerNode:
                    VisitChildren(node);
                    
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
                    VisitChildren(node);
                    
                    if (node.Left.Type == "bool" || node.Right.Type == "bool")
                        throw new InvalidOperandsException(node,
                            "The operands of a relational operator can only be int and double. It does not allow bool.");
                    else
                        node.Type = "bool";
                    break;
                    
                // Boolean
                case AndNode:
                case OrNode:
                    VisitChildren(node);
                   
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