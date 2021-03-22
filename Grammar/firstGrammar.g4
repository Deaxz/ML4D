grammar firstGrammar;

prog: lines EOF;

lines:
    (dcl SEMICOLON lines |
    stmt SEMICOLON lines )?;

dcl:
    type ID (ASSIGN expr_stmt)? |
    FUNDCL type? ID LPAREN (type ID (COMMA type ID)*)? RPAREN LBRACE lines RBRACE |;

stmt:
    WHILE LPAREN expr_stmt RPAREN LBRACE lines RBRACE |
    ID LPAREN ID RPAREN |  //method call
    ID BACKWARDS |
    RETURN expr_stmt |
    expr_stmt;

expr_stmt:
    ID ASSIGN expr_stmt |
    equality_expr;

equality_expr:
    relational_expr (EQUALS | NOTEQUALS) equality_expr |
    relational_expr |;

relational_expr:
    plinus_expr (GTHAN | LTHAN | GETHAN | LETHAN) relational_expr |
    plinus_expr;

plinus_expr:
    muldiv_expr (PLUS | MINUS) plinus_expr |
    muldiv_expr;

muldiv_expr:
    power_expr (MUL | DIV) muldiv_expr |
    power_expr;

power_expr:
    factor POWER power_expr |
    factor;

factor:
    ID |
    ID DOT ID |
    INUM |
    FNUM |
    LPAREN expr_stmt RPAREN |
    method_invocation;

method_invocation:
    ID LPAREN argument_list RPAREN;

argument_list:
    (relational_expr (COMMA relational_expr)*)?;

type:
    FLOAT |
    DOUBLE |
    INT;

DOT: '.';
FOR: 'for';
WHILE: 'while';
FLOAT: 'float';
DOUBLE: 'double';
INT: 'int';
//TENSOR: 'tensor';
RETURN: 'return ';
BACKWARDS: '<-';
FUNDCL: 'fun';
//PRINT: 'p';
ASSIGN: '=';
POWER: '**';
MUL: '*';
DIV: '/';
PLUS: '+';
MINUS: '-';
COMMA: ',';
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
LBRACK: '[';
RBRACK: ']';
EQUALS: '==';
NOTEQUALS: '!=';
GTHAN: '>';
LTHAN: '<';
GETHAN: '>=';
LETHAN: '<=';
SEMICOLON: ';';
WS: [ \t\r\n]+ -> skip;
INUM: [-]?[0-9]+;
FNUM: [-]?[0-9]+ [.][0-9]+;
ID: [A-z]([0-9A-z])*;