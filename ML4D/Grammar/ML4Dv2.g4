grammar ML4Dv2;

lines
   :   ((dcl | stmt))+
   |   EOF
   ;

dcl
    :   type=types id=ID op='=' right=bool_expr ';'                                                 # varDecl
    |   type=types id=ID ('[' expr ']' '[' expr ']' ) op='=' right=tensorInit ';'                   # tensorDecl
    |   type=types id=ID '(' (argtype+=types argid+=ID (',' types ID)*)? ')' '{' body=lines '}'     # funcDecl
    ;

stmt
    :   IF '(' bool_expr ')' '{' body=lines '}' (ELSE IF '(' bool_expr ')' '{' body=lines '}' )* (ELSE '{' body=lines '}')? # ifStmt
    |   FOR '(' initialization=dcl ';' condition=bool_expr ';' finalExpression=expression_stmt ')' '{' body=lines '}'       # forStmt
    |   WHILE '(' predicate=bool_expr ')' '{' body=lines '}'                                                                # whileStmt
    |   RETURN inner=bool_expr? ';'                                                                                         # returnStmt
    |   id=ID '(' (argexpr+=bool_expr (',' bool_expr)*)? ')'                                                                # funcStmt
    |   GRADIENTS '(' ID ',' '(' ID '<<' ID (',' ID '<<' ID) ')' ',' '{' lines '}' ')' ';'                                  # gradientsStmt
    |   expression_stmt ';'                                                                                                 # exprStmt
    ;
    
expression_stmt
    : id=ID op='=' right=bool_expr                              # assignExpr
    | bool_expr                                                 # exprSemi  
    ;

bool_expr
    :   left=expr op=('<'|'<='|'>'|'>=') right=expr             # infixRelationalExpr
    |   left=expr op=('=='|'!=') right=expr                     # infixRelationalExpr
    |   left=bool_expr op='and' right=bool_expr                 # infixBoolExpr
    |   left=bool_expr op='or'  right=bool_expr                 # infixBoolExpr
    |   expr                                                    # exprExpr
    ;

tensorInit
    :   '{' ('[' expr (',' expr)*']') (',' '[' expr (',' expr)*']')* '}'
    ;

expr
    :   '(' bool_expr ')'                                       # parensExpr
    |   op='-' left=expr                                        # unaryExpr  // TODO unary minus (virker umiddelbart, men skal måske udskydes?) (Check træet for -1**-2**-3**-4, event unary minus under power)
    |   op='not' inner=bool_expr                                # unaryExpr  // TODO Man kan skrive "b = not not not not a;", men den kan ikke flyttes pga precendence, men tror heller ikke det er et problem.
    |   <assoc=right> left=expr op='**' right=expr              # infixExpr
    |   left=expr op=('*'|'/') right=expr                       # infixExpr
    |   left=expr op=('+'|'-') right=expr                       # infixExpr
    |   id=ID '(' (argexpr+=bool_expr (',' bool_expr)*)? ')'    # funcExpr
    |   value=(INUM|FNUM|BOOLVAL|ID)                            # typeExpr
    ;

types
    :   type=INT
    |   type=BOOL
    |   type=DOUBLE
    |   type=TENSOR
    |   type=VOID
    ;

// Types
VOID: 'void';
DOUBLE: 'double';
INT: 'int';
BOOL: 'bool';
TENSOR: 'tensor';

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
IF: 'if';
ELSE: 'else';
WHILE: 'while';
FOR: 'for';
RETURN: 'return';
GRADIENTS: 'gradients';
WS: [ \t\r\n]+ -> skip;

// Values
BOOLVAL: ('true'|'false');
INUM: [0-9]+;
FNUM: [0-9]+ [.][0-9]+;
ID: [A-Za-z]([0-9_A-Za-z])*;

// Comments
BLOCKCOMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT: '//' ~[\r\n]* -> skip;