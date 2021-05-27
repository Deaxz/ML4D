//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/Dion/source/repos/p4/ML4D/Grammar\ML4D.g4 by ANTLR 4.9.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="ML4DParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.1")]
[System.CLSCompliant(false)]
public interface IML4DVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="ML4DParser.lines"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLines([NotNull] ML4DParser.LinesContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>varDecl</c>
	/// labeled alternative in <see cref="ML4DParser.dcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarDecl([NotNull] ML4DParser.VarDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>tensorDecl</c>
	/// labeled alternative in <see cref="ML4DParser.dcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTensorDecl([NotNull] ML4DParser.TensorDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>funcDecl</c>
	/// labeled alternative in <see cref="ML4DParser.dcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFuncDecl([NotNull] ML4DParser.FuncDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ML4DParser.tensor_init"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTensor_init([NotNull] ML4DParser.Tensor_initContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ifStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfStmt([NotNull] ML4DParser.IfStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>forStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForStmt([NotNull] ML4DParser.ForStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>whileStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhileStmt([NotNull] ML4DParser.WhileStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>returnStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnStmt([NotNull] ML4DParser.ReturnStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>funcStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFuncStmt([NotNull] ML4DParser.FuncStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>gradientsStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGradientsStmt([NotNull] ML4DParser.GradientsStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>assignStmt</c>
	/// labeled alternative in <see cref="ML4DParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignStmt([NotNull] ML4DParser.AssignStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ML4DParser.assign_expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssign_expr([NotNull] ML4DParser.Assign_exprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>exprExpr</c>
	/// labeled alternative in <see cref="ML4DParser.bool_expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExprExpr([NotNull] ML4DParser.ExprExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>infixRelationalExpr</c>
	/// labeled alternative in <see cref="ML4DParser.bool_expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInfixRelationalExpr([NotNull] ML4DParser.InfixRelationalExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>infixBoolExpr</c>
	/// labeled alternative in <see cref="ML4DParser.bool_expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInfixBoolExpr([NotNull] ML4DParser.InfixBoolExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>unaryExpr</c>
	/// labeled alternative in <see cref="ML4DParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnaryExpr([NotNull] ML4DParser.UnaryExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>funcExpr</c>
	/// labeled alternative in <see cref="ML4DParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFuncExpr([NotNull] ML4DParser.FuncExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>typeExpr</c>
	/// labeled alternative in <see cref="ML4DParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeExpr([NotNull] ML4DParser.TypeExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>infixValueExpr</c>
	/// labeled alternative in <see cref="ML4DParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInfixValueExpr([NotNull] ML4DParser.InfixValueExprContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>parensExpr</c>
	/// labeled alternative in <see cref="ML4DParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParensExpr([NotNull] ML4DParser.ParensExprContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ML4DParser.types"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypes([NotNull] ML4DParser.TypesContext context);
}
