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
            symbolTable.Insert("gradients", "void", false);
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
            if (SymbolTable.GetCurrentFunctionType() is null)
            {
                if (SymbolTable.Retrieve(node.ID) is null)
                    SymbolTable.Insert(node.ID, node.Type, true);
                else
                    throw new FunctionAlreadyDeclaredException(node,
                        $"The function \"{node.ID}\" has already been declared in the current or parent scope.");
            }
            else
                throw new Exception(
                    $"The function \"{node.ID}\" was declared in a function. Nested function are not allowed in ML4D.");
            
            SymbolTable.OpenScope(node.Type);
            base.Visit(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(FunctionArgumentNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
            {
                if (node.Type == "tensor")
                    throw new Exception("Functions cannot take tensors as parameters.");
                SymbolTable.Insert(node.ID, node.Type, false);
            }
            else
                throw new VariableAlreadyDeclaredException(
                    $"The variable \"{node.ID}\" has already been declared. And would hide the variable in the parent scope if declared inside the function.");
        }

        public override void Visit(TensorDCLNode node)
        {
            if (SymbolTable.Retrieve(node.ID) is null)
                SymbolTable.Insert(node.ID, node.Type, false, node.Rows, node.Columns);
            else
                throw new VariableAlreadyDeclaredException(
                    $"The variable \"{node.ID}\" could not be declared, as it has already been declared in the current or parent scope.");

            base.Visit(node);

            int initColumns = -1;
            int initRows = -1;

            if (node.Init is TensorInitNode init)
            {
                initColumns = init.FirstRowElements.Count;
                initRows = (init.Elements.Count / initColumns) + 1;
            }
            else if (node.Init.Rows is not null && node.Init.Columns is not null)
            {
                initColumns = (int) node.Init.Columns;
                initRows = (int) node.Init.Rows;
            }

            if (initColumns != node.Columns || initRows != node.Rows)
                throw new Exception(
                    $"Declared dimensions rows: {node.Rows} - {initRows}, columns: {node.Columns} - {initColumns}");
        }

        public override void Visit(TensorInitNode node)
        {
            base.Visit(node);

            foreach (ExpressionNode element in node.FirstRowElements)
                if (element.Type is "bool" or "tensor")
                    throw new TensorInitialisationException(node,
                        $"Incorrect element type: {element.Type}, only double and int are allowed");
            foreach (ExpressionNode element in node.Elements)
                if (element.Type is "bool" or "tensor")
                    throw new TensorInitialisationException(node,
                        $"Incorrect element type: {element.Type}, only double and int are allowed");
        }

        // Statement      
        public override void Visit(AssignNode node)
        {
            Symbol variableDCL = SymbolTable.Retrieve(node.ID);

            if (SymbolTable.Retrieve(node.ID) is null)
                throw new VariableNotDeclaredException(
                    $"The variable \"{node.ID}\" cannot be assigned, as it has not been declared.");

            base.Visit(node);

            if (variableDCL is TensorSymbol tensorDcl)
            {
                if (tensorDcl.Rows != node.Right.Rows || tensorDcl.Columns != node.Right.Columns)
                    throw new VariableAssignmentException(node,
                        $"A tensor's dimensions cannot be changed during run-time. " +
                        $"Rows: {tensorDcl.Rows} - {node.Right.Rows}, Columns:{tensorDcl.Columns} - {node.Right.Columns}");
                node.Type = tensorDcl.Type;
            }
            else
                node.Type = variableDCL.Type;

            if (node.Type != node.Right.Type && (node.Type == "double" && node.Right.Type == "int"))
                throw new VariableAssignmentException(node,
                    $"Illegal assignment. Left side: \"{node.Type}\", right side: \"{node.Right.Type}\".");
        }

        public override void Visit(WhileNode node)
        {
            SymbolTable.OpenScope();
            base.Visit(node);

            if (node.Predicate.Type != "bool")
                throw new PredicateTypeException(node, "Predicate is not of type bool.");
            SymbolTable.CloseScope();
        }

        public override void Visit(ReturnNode node)
        {
            base.Visit(node);
            string funcType = SymbolTable.GetCurrentFunctionType();
            
            if (funcType is null)
                throw new ReturnOutsideFunctionException(node,
                    "Return was called outside function scope, which is not allowed");
            
            if (node.Inner is not null)
            {
                if (node.Inner.Type != funcType)
                    throw new InvalidOperationException(
                        $"Function has return type: {funcType} but you are returning a {node.Inner.Type}.\n");
                node.Type = node.Inner.Type;
            }
            else if(funcType is not "void")
                throw new InvalidOperationException(
                        $"Function has return type: {funcType} but you are returning a void.\n");
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
            foreach (IfNode ifNode in node.IfNodes)
                Visit(ifNode);

            if (node.ElseBody is not null)
            {
                SymbolTable.OpenScope();
                Visit(node.ElseBody);
                SymbolTable.CloseScope();
            }
        }

        public override void Visit(IfNode node)
        {
            SymbolTable.OpenScope();
            base.Visit(node);

            if (node.Predicate.Type != "bool")
                throw new PredicateTypeException(node, "Predicate is not of type bool.");
            SymbolTable.CloseScope();
        }


        public override void Visit(ForNode node)
        {
            SymbolTable.OpenScope();
            base.Visit(node);

            if (node.Predicate.Type != "bool")
                throw new PredicateTypeException(node, "Predicate is not of type bool.");
            SymbolTable.CloseScope();
        }

        public override void Visit(GradientsNode node)
        {
            SymbolTable.OpenScope();

            Symbol tensorDCL = SymbolTable.Retrieve(node.tensorID); 
            
            if (SymbolTable.Retrieve(node.tensorID) is null)
                throw new VariableNotDeclaredException(
                    $"The variable \"{tensorDCL.Name}\" cannot be assigned, as it has not been declared.");
            if (tensorDCL.Type != "tensor")
                throw new Exception($"Variable \"{tensorDCL.Name}\" is not a tensor. Grads can only be derived from tensors.");

            for (int i = 0; i < node.GradVariables.Count; i++)
            {
                string gradVar = node.GradVariables[i];
                string gradTensor = node.GradTensors[i];

                Symbol symbol = SymbolTable.Retrieve(gradTensor);

                if (symbol is TensorSymbol tensorSymbol) // Er null check nødvendigt? eller implicit i "is" operator?
                {
                    if (SymbolTable.Retrieve(gradVar) is null)
                        SymbolTable.Insert(gradVar, "tensor", false, tensorSymbol.Rows, tensorSymbol.Columns);
                    else
                        throw new VariableAlreadyDeclaredException(
                            $"The variable \"{gradVar}\" could not be declared, as it has already been declared in the current or parent scope.");
                }
                else if (symbol is null)
                    throw new VariableNotDeclaredException(
                        $"The variable \"{gradTensor}\" cannot be assigned, as it has not been declared.");
                else
                    throw new Exception(
                        $"Variable \"{symbol.Name}\" is not a tensor. Grads can only be derived from tensors.");
            }
            
            base.Visit(node);
            
            SymbolTable.CloseScope();
        }

        // Expression
        public override void Visit(IDNode node)
        {
            Symbol variableDCL = SymbolTable.Retrieve(node.ID);
            if (variableDCL is null)
                throw new VariableNotDeclaredException(node,
                    $"The variable \"{node.ID}\" cannot be used, as it has not been declared.");

            if (variableDCL is TensorSymbol tensorDcl)
            {
                node.Type = tensorDcl.Type;
                node.Rows = tensorDcl.Rows;
                node.Columns = tensorDcl.Columns;
            }
            else
                node.Type = variableDCL.Type;
        }

        private void TensorTypecheck(InfixExpressionNode node)
        {
            if (node.Left.Type == "tensor" && node.Right.Type == "tensor")
            {
                switch (node)
                {
                    case AdditionNode:
                    case SubtractionNode:
                        if (node.Left.Rows != node.Right.Rows || node.Left.Columns != node.Right.Columns)
                            throw new InvalidOperandsException(node,
                                $"The dimensions of the tensors are incorrect for the operation \"{node.Symbol}\".");
                        node.Rows = node.Left.Rows;
                        node.Columns = node.Left.Columns;
                        node.Type = "tensor";
                        break;

                    case MultiplicationNode:
                        if (node.Left.Columns != node.Right.Rows)
                            throw new InvalidOperandsException(node,
                                $"The dimensions of the tensors are incorrect for the operation \"{node.Symbol}\".");
                        node.Rows = node.Left.Rows;
                        node.Columns = node.Right.Columns;
                        node.Type = "tensor";
                        break;
                    default:
                        throw new InvalidOperandsException(node,
                            $"The operator \"{node.Symbol}\", does not allow right operands of type tensor.");
                }
                return;
            }

            if (node.Left.Type == "tensor")
            {
                switch (node)
                {
                    case AdditionNode:
                    case SubtractionNode:
                    case MultiplicationNode:
                    case DivisionNode:
                    case PowerNode:
                        node.Rows = node.Left.Rows;
                        node.Columns = node.Left.Columns;
                        node.Type = "tensor";
                        break;
                    default:
                        throw new InvalidOperandsException(node,
                            $"The operator \"{node.Symbol}\", does not allow operands of type tensor.");
                }
                return;
            }

            if (node.Right.Type == "tensor")
            {
                switch (node)
                {
                    case AdditionNode:
                    case SubtractionNode:
                    case MultiplicationNode:
                        node.Rows = node.Right.Rows;
                        node.Columns = node.Right.Columns;
                        node.Type = "tensor";
                        break;
                    default:
                        throw new InvalidOperandsException(node,
                            $"The operator \"{node.Symbol}\", does not allow right operands of type tensor.");
                }
            }
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
                    else if (node.Left.Type == "tensor" || node.Right.Type == "tensor")
                        TensorTypecheck(node);
                    else if (node.Left.Type == "double" || node.Right.Type == "double")
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
                    else if (node.Left.Type == "tensor" || node.Right.Type == "tensor")
                        throw new InvalidOperandsException(node,
                            "The operands of a relational operator can only be int and double. It does not allow tensor.");
                    else
                        node.Type = "bool";
                    break;

                // Boolean
                case AndNode:
                case OrNode:
                    if (node.Left.Type != "bool" || node.Right.Type != "bool")
                        throw new InvalidOperandsException(node,
                            "The operands of a bool operator can only be bool. It does not allow int, double or tensor.");
                    else
                        node.Type = "bool";
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Visit(UnaryExpressionNode node)
        {
            base.Visit(node);

            switch (node)
            {
                case NotNode:
                    if (node.Inner.Type != "bool")
                        throw new InvalidOperandsException(node,
                            "The operand of a bool operator can only be bool. It does not allow int, double or tensor.");
                    else
                        node.Type = "bool";
                    break;

                case UnaryMinusNode:
                    switch (node.Inner.Type)
                    {
                        case "int":
                            node.Type = "int";
                            break;
                        case "double":
                            node.Type = "double";
                            break;
                        case "tensor":
                            node.Rows = node.Inner.Rows;
                            node.Columns = node.Inner.Columns;
                            break;
                        default:
                            throw new InvalidOperandsException(node,
                                "The operand of a arithmetic operator can not be bool. It does only allow int, double or tensor.");
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}