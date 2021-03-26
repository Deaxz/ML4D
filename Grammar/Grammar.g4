grammar firstGrammar;

prog: lines EOF;

lines:
    (dcl SEMICOLON lines |
     stmt SEMICOLON lines)?;

dcl:
    type ID (ASSIGN expr_stmt)? |
    (type | VOID) ID LPAREN (type ID (COMMA type ID)*)? RPAREN LBRACE lines RBRACE;

stmt:
    ID ASSIGN expr_stmt |
    WHILE LPAREN expr_stmt RPAREN LBRACE lines RBRACE |
    ID BACKWARDS |
    RETURN expr_stmt |
    expr_stmt;

expr_stmt:
    logical_OR_expr;

logical_OR_expr:
    logical_OR_expr OR logical_AND_expr |
    logical_AND_expr;

logical_AND_expr:
    logical_AND_expr AND equals_expr |
    equals_expr |
    notequals_expr;

equals_expr:
    (addition_expr | subtraction_expr) EQUALS (addition_expr | subtraction_expr) |
    relational_gthan_expr |
    relational_lthan_expr |
    relational_gethan_expr |
    relational_lethan_expr;

notequals_expr:
    (addition_expr | subtraction_expr) NOTEQUALS (addition_expr | subtraction_expr) |
    relational_gthan_expr |
    relational_lthan_expr |
    relational_gethan_expr |
    relational_lethan_expr;

relational_gthan_expr:
    (addition_expr | subtraction_expr) GTHAN (addition_expr | subtraction_expr) |
    addition_expr |
    subtraction_expr;

relational_lthan_expr:
    (addition_expr | subtraction_expr) LTHAN (addition_expr | subtraction_expr) |
    addition_expr |
    subtraction_expr;

relational_gethan_expr:
    (addition_expr | subtraction_expr) LETHAN (addition_expr | subtraction_expr) |
    addition_expr |
    subtraction_expr;

relational_lethan_expr:
    (addition_expr | subtraction_expr) LETHAN (addition_expr | subtraction_expr) |
    addition_expr |
    subtraction_expr;

addition_expr:
    (multiplication_expr | division_expr) PLUS (addition_expr | subtraction_expr) |
    multiplication_expr |
    division_expr;

subtraction_expr:
    (multiplication_expr | division_expr) MINUS (addition_expr | subtraction_expr) |
    multiplication_expr |
    division_expr;

multiplication_expr:
    power_expr MUL (multiplication_expr | division_expr) |
    power_expr;

division_expr:
    power_expr DIV (multiplication_expr | division_expr) |
    power_expr;

power_expr:
    factor POWER power_expr |
    factor;

factor:
    ID |
    ID DOT GRAD |
    INUM |
    FNUM |
    BOOLVAL |
    not_expr |
    LPAREN expr_stmt RPAREN |
    method_invocation;

not_expr:
    NOT ID;

method_invocation:
    ID LPAREN argument_list RPAREN;

argument_list:
    (expr_stmt (COMMA expr_stmt)*)?;

type:
    INT |
    BOOL |
    DOUBLE;

GRAD:'grad';
NOT: 'not';
AND: 'and';
OR: 'or';  
VOID: 'void';
DOT: '.';
WHILE: 'while';
DOUBLE: 'double';
INT: 'int';
BOOL: 'bool';
BOOLVAL: ('true'|'false');
RETURN: 'return';
BACKWARDS: '<-';
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
INUM: [-]?[0-9]+; // Unary?
FNUM: [-]?[0-9]+ [.][0-9]+; // Unary?
ID: [A-z]([0-9A-z])*;