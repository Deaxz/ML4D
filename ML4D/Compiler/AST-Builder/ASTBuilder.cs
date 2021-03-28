using System;
using Antlr4.Runtime.Misc;

namespace ML4D.Compiler
{
	public class ASTBuilder : dinoBaseVisitor<ExpressionNode>
	{
		public override ExpressionNode VisitProg(dinoParser.ProgContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitLines(dinoParser.LinesContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitDcl(dinoParser.DclContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitStmt(dinoParser.StmtContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitBool_expr(dinoParser.Bool_exprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitInfixExpr(dinoParser.InfixExprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitUnaryExpr(dinoParser.UnaryExprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitFuncExpr(dinoParser.FuncExprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitNumberExpr(dinoParser.NumberExprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitParensExpr(dinoParser.ParensExprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitIdExpr(dinoParser.IdExprContext context)
		{
			return VisitChildren(context);
		}

		public override ExpressionNode VisitTypes(dinoParser.TypesContext context)
		{
			return VisitChildren(context);
		}
	}
}


