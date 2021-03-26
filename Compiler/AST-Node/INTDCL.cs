internal class IntDCLNode : DCLNode
{
    public string id { get; set; }
    
    //if init is null then it is declared without initialisation
    public ExprNode init;
}