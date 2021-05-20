#nullable enable
using System;
using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler
{
	public class ASTBuilder : ML4DBaseVisitor<Node>
	{
		public override LinesNode VisitLines(ML4DParser.LinesContext context)
		{
			LinesNode linesNode = new LinesNode();

			foreach (IParseTree child in context.children)
			{
				Node? node = Visit(child);
				if (node is not null)
					linesNode.lines.Add(node);
			}
			return linesNode;
		}

		// Declarations
		public override Node VisitVarDecl(ML4DParser.VarDeclContext context)
		{
			VariableDCLNode varDeclNode;

			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token.
			{
				case ML4DLexer.INT:
					varDeclNode = new VariableDCLNode("int", context.id.Text, (ExpressionNode) Visit(context.init));
					break;
				case ML4DLexer.DOUBLE:
					varDeclNode = new VariableDCLNode("double", context.id.Text, (ExpressionNode) Visit(context.init));
					break;
				case ML4DLexer.BOOL:
					varDeclNode = new VariableDCLNode("bool", context.id.Text, (ExpressionNode) Visit(context.init));
					break;
				default:
					throw new NotSupportedException(
						$"The variable {context.id.Text}, was declared with an illegal type.");
			}
			return varDeclNode;
		}

		public override Node VisitTensorDecl(ML4DParser.TensorDeclContext context)
		{
			TensorDCLNode tensorDclNode;
			
			if (context.assignInit.IsEmpty)
				tensorDclNode = new TensorDCLNode(context.type.Text, context.id.Text, int.Parse(context.rows.Text), int.Parse(context.coloumns.Text), (TensorInitNode) Visit(context.init));
			else 
				tensorDclNode = new TensorDCLNode(context.type.Text, context.id.Text, int.Parse(context.rows.Text), int.Parse(context.coloumns.Text), (TensorInitNode) Visit(context.assignInit));

			return tensorDclNode;
		}

		public override Node VisitTensor_init(ML4DParser.Tensor_initContext context)
		{
			TensorInitNode tensorInitNode = new TensorInitNode();

			foreach (ML4DParser.ExprContext expr in context._firstRow)
				tensorInitNode.FirstRowElements.Add((ExpressionNode) Visit(expr));
			foreach (ML4DParser.ExprContext expr in context._elements)
				tensorInitNode.Elements.Add((ExpressionNode) Visit(expr));
			return tensorInitNode;
		}

		public override Node VisitFuncDecl(ML4DParser.FuncDeclContext context)
		{
			FunctionDCLNode functionDclNode;

			switch (context.type.type.Type) // 1. dcl type = types, 2. types type = INT..., 3. type.Type for token.
			{
				case ML4DLexer.INT:
					functionDclNode = new FunctionDCLNode("int", context.id.Text);
					break;
				case ML4DLexer.DOUBLE:
					functionDclNode = new FunctionDCLNode("double", context.id.Text);
					break;
				case ML4DLexer.BOOL:
					functionDclNode = new FunctionDCLNode("bool", context.id.Text);
					break;
				case ML4DLexer.VOID:
					functionDclNode = new FunctionDCLNode("void", context.id.Text);
					break;
				default:
					throw new NotSupportedException(
						$"The function {context.id.Text}, was declared with an illegal type.");
			}

			for (int i = 0; i < context._argid.Count; i++)
				functionDclNode.Arguments.Add(new FunctionArgumentNode(
					context._argtype[i].type.Text, context._argid[i].Text));

			functionDclNode.Body = VisitLines(context.body);
			return functionDclNode;
		}

		// Statements
		public override Node VisitIfStmt(ML4DParser.IfStmtContext context)
		{
			IfElseChainNode ifElseChainNode = new IfElseChainNode();
			int predicates = context._pred.Count;
			int bodies = context._body.Count;
			
			// If
			ifElseChainNode.IfNodes.Add(new IfNode((ExpressionNode) Visit(context._pred[0]),
														(LinesNode) Visit(context._body[0])));
			// Else
			if (predicates == 1 && predicates < bodies)
				ifElseChainNode.ElseBody = (LinesNode) Visit(context._body[bodies-1]);

			// Else if/Else if Else
			if (predicates > 1 && predicates == bodies)
			{
				for (int i = 1; i < bodies; i++)
					ifElseChainNode.IfNodes.Add(new IfNode((ExpressionNode) Visit(context._pred[i]),
																(LinesNode) Visit(context._body[i])));
			}
			else if (predicates > 1 && predicates < bodies)
            {
                for (int i = 1; i < predicates; i++)
                    ifElseChainNode.IfNodes.Add(new IfNode((ExpressionNode) Visit(context._pred[i]),
																(LinesNode) Visit(context._body[i])));
                ifElseChainNode.ElseBody = (LinesNode) Visit(context._body[bodies-1]);
            }
			return ifElseChainNode;
		}
		
		public override Node VisitForStmt(ML4DParser.ForStmtContext context)
		{
			Node initNode = Visit(context.init);
			if (initNode is not VariableDCLNode)
				throw new Exception("Init is not of type VariableDCLNode");
			ForNode forNode = new ForNode(
				(VariableDCLNode) initNode,
				(ExpressionNode) Visit(context.pred), 
				(AssignNode) Visit(context.final),
				(LinesNode) Visit(context.body));
			return forNode;
		}

		public override Node VisitWhileStmt(ML4DParser.WhileStmtContext context)
		{
			WhileNode whileNode = new WhileNode(
				(ExpressionNode) Visit(context.pred), (LinesNode) Visit(context.body));
			return whileNode;
		}

		public override Node VisitReturnStmt(ML4DParser.ReturnStmtContext context)
		{
			ReturnNode returnNode;
			if (context.inner is not null)
				returnNode = new ReturnNode((ExpressionNode) Visit(context.inner));
			else
				returnNode = new ReturnNode();
			return returnNode;
		}

		public override Node VisitFuncStmt(ML4DParser.FuncStmtContext context)
		{
			FunctionStmtNode functionStmtNode = new(context.id.Text);

			foreach (ML4DParser.Bool_exprContext argument in context._argexpr)
				functionStmtNode.Arguments.Add(Visit(argument));
			return functionStmtNode;
		}

		public override Node VisitGradientsStmt(ML4DParser.GradientsStmtContext context)
		{
			GradientsNode gradientsNode = new GradientsNode(context.tensor.Text, (LinesNode) Visit(context.body));
			
			for (int i = 0; i < context._gradvar.Count; i++)
			{
				gradientsNode.GradVariables.Add(new FunctionArgumentNode("double", context._gradvar[i].Text));
				gradientsNode.GradTensors.Add(context._gradtensor[i].Text);
			}
			return gradientsNode;
		}

		public override Node VisitAssignStmt(ML4DParser.AssignStmtContext context)
		{
			return Visit(context.assign_expr());
		}

		public override Node VisitAssign_expr(ML4DParser.Assign_exprContext context)
		{
			return new AssignNode(context.id.Text, (ExpressionNode) Visit(context.right));
		}

		// Expressions
		public override Node VisitInfixRelationalExpr(ML4DParser.InfixRelationalExprContext context)
		{
			InfixExpressionNode node;

			switch (context.op.Type)
			{
				case ML4DLexer.LTHAN:
					node = new LessThanNode(context.op.Text);
					break;
				case ML4DLexer.GTHAN:
					node = new GreaterThanNode(context.op.Text);
					break;
				case ML4DLexer.LETHAN:
					node = new LessEqualThanNode(context.op.Text);
					break;
				case ML4DLexer.GETHAN:
					node = new GreaterEqualThanNode(context.op.Text);
					break;
				case ML4DLexer.EQUALS:
					node = new EqualNode(context.op.Text);
					break;
				case ML4DLexer.NOTEQUALS:
					node = new NotEqualNode(context.op.Text);
					break;
				default:
					throw new NotSupportedException(
						$"The operator {context.op.Text}, is not a valid relational operator.");
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitInfixBoolExpr(ML4DParser.InfixBoolExprContext context)
		{
			InfixExpressionNode node;

			switch (context.op.Type)
			{
				case ML4DLexer.AND:
					node = new AndNode("and");
					break;
				case ML4DLexer.OR:
					node = new OrNode("or");
					break;
				default:
					throw new NotSupportedException(
						$"The operator {context.op.Text}, is not a valid bool operator.");
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitParensExpr(ML4DParser.ParensExprContext context)
		{
			ExpressionNode node = (ExpressionNode) Visit(context.inner);
			node.Parenthesized = true;
			return node;
		}

		public override Node VisitInfixValueExpr(ML4DParser.InfixValueExprContext context)
		{
			InfixExpressionNode node;

			switch (context.op.Type)
			{
				case ML4DLexer.PLUS:
					node = new AdditionNode("+");
					break;
				case ML4DLexer.MINUS:
					node = new SubtractionNode("-");
					break;
				case ML4DLexer.MUL:
					node = new MultiplicationNode("*");
					break;
				case ML4DLexer.DIV:
					node = new DivisionNode("/");
					break;
				case ML4DLexer.POW:
					node = new PowerNode("**");
					break;
				default:
					throw new NotSupportedException(
						$"The operator {context.op.Text}, is not a valid arithmetic operator.");
			}
			node.Left = (ExpressionNode) Visit(context.left);
			node.Right = (ExpressionNode) Visit(context.right);
			return node;
		}

		public override Node VisitUnaryExpr(ML4DParser.UnaryExprContext context)
		{
			UnaryExpressionNode node;

			switch (context.op.Type)
			{
				case ML4DLexer.MINUS:
					node = new UnaryMinusNode("-");
					node.Inner = (ExpressionNode) Visit(context.right);
					break;
				case ML4DLexer.NOT:
					node = new NotNode("not");
					node.Inner = (ExpressionNode) Visit(context.inner);
					break;
				default:
					throw new NotSupportedException(
						$"The operator {context.op.Text}, is not a valid unary operator.");
			}
			return node;
		}

		public override Node VisitFuncExpr(ML4DParser.FuncExprContext context)
		{
			FunctionExprNode functionExprNode = new FunctionExprNode(context.id.Text);

			foreach (ML4DParser.Bool_exprContext argument in context._argexpr)
				functionExprNode.Arguments.Add(Visit(argument));
			return functionExprNode;
		}

		// Types
		public override Node VisitTypeExpr(ML4DParser.TypeExprContext context)
		{
			ExpressionNode node;

			switch (context.value.Type)
			{
				case ML4DLexer.INUM:
					node = new IntNode(int.Parse(context.value.Text));
					break;
				case ML4DLexer.FNUM:
					node = new DoubleNode(double.Parse(context.value.Text, CultureInfo.InvariantCulture));
					break;
				case ML4DLexer.BOOLVAL:
					node = new BoolNode(bool.Parse(context.value.Text));
					break;
				case ML4DLexer.ID:
					node = new IDNode(context.value.Text);
					break;
				default:
					throw new NotSupportedException($"The value {context.value.Text}, is not allowed.");
			}
			return node;
		}
	}
}
