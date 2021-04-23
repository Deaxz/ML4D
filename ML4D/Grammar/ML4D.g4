grammar ML4D;
  
lines
   :   ((dcl | stmt) ';')+   // TODO forestiller mig comments bliver et | her
  //:   (udensemi | (dcl | stmt) ';')+ // Fungere, men på både vores nuværende, og udensemi, så yeeter den resten af træet ved ";;" og ";" respectively.
   |   EOF
   ;

//udensemi 
//    : WHILE '(' predicate=bool_expr ')' '{' body=lines '}'                                    # whileStmt
//    | type=types id=ID '(' (argtype+=types argid+=ID (',' types ID)*)? ')' '{' body=lines '}'  # funcDecl
//    | For loop        
//    ;

dcl
    :   type=types id=ID op='=' right=bool_expr                                                                  # varDecl                        
    |   type=types id=ID '(' (argtype+=types argid+=ID (',' argtype+=types argid+=ID)*)? ')' '{' body=lines '}'  # funcDecl
    ;    

stmt
    :   id=ID op='=' right=bool_expr                                     # assignStmt
    |   WHILE '(' predicate=bool_expr ')' '{' body=lines '}'             # whileStmt
    |   id=ID op='<-'                                                    # backwardStmt // TODO slet
    |   RETURN inner=bool_expr?                                          # returnStmt
    |   id=ID '(' (argexpr+=bool_expr (',' argexpr+=bool_expr)*)? ')'    # funcStmt
    ;

bool_expr 
    :   left=expr op=('<'|'<='|'>'|'>=') right=expr             # infixRelationalExpr
    |   left=expr op=('=='|'!=') right=expr                     # infixRelationalExpr
    |   left=bool_expr op='and' right=bool_expr                 # infixBoolExpr
    |   left=bool_expr op='or'  right=bool_expr                 # infixBoolExpr
    |   expr                                                    # exprExpr
    ;
    
expr  // TODO introduce unary minus, can be done similarly to Math AST
    :   '(' bool_expr ')'                                               # parensExpr 
//    |   op='-' left=expr                                                # unaryExpr // TODO unary minus
    |   op='not' inner=bool_expr                                        # unaryExpr  // TODO Man kan skrive "b = not not not not a;", men den kan ikke flyttes pga precendence, men tror heller ikke det er et problem.
    |   <assoc=right> left=expr op='**' right=expr                      # infixValueExpr
    |   left=expr op=('*'|'/') right=expr                               # infixValueExpr
    |   left=expr op=('+'|'-') right=expr                               # infixValueExpr
    |   id=ID '(' (argexpr+=bool_expr (',' argexpr+=bool_expr)*)? ')'   # funcExpr  
    |   value=(INUM|FNUM|BOOLVAL|ID)                                    # typeExpr
    ;
        
types
    :   type=INT 
    |   type=BOOL 
    |   type=DOUBLE
    |   type=VOID
    ;

// Types
VOID: 'void';
DOUBLE: 'double'; 
INT: 'int';
BOOL: 'bool';

// Operators
NOT: 'not';
POW: '**';
MUL: '*';
DIV: '/';
PLUS: '+';
MINUS: '-';
EQUALS: '==';
NOTEQUALS: '!=';
GTHAN: '>';
LTHAN: '<';
GETHAN: '>=';
LETHAN: '<=';
AND: 'and';
OR: 'or'; 
BACKWARDS: '<-';
ASSIGN: '=';

// Delimiters
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
LBRACK: '[';
RBRACK: ']';
COMMA: ',';
SEMICOLON: ';';

// Control Structure
WHILE: 'while';
RETURN: 'return';
WS: [ \t\r\n]+ -> skip;

// Values
BOOLVAL: ('true'|'false');
INUM: [1-9][0-9]*; 
FNUM: [1-9][0-9]*[.][0-9]+; 
ID: [A-Za-z][0-9_A-Za-z]*;