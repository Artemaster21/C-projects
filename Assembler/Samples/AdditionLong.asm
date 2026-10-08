// Test addition with constants
@8
D=A
@result1
M=D

// Test addition with variables
@10
D=A
@x
M=D
@4
D=A
@y
M=D
@x
D=M
@y
D=D+M
@sum
M=D

// Test addition with mixed operands
@x
D=M
@2
D=D+A
@mixed
M=D
@3
D=A
@y
D=D+M
@mixed2
M=D

// Test chained addition
@x
D=M
@temp
M=D
@temp
D=M
@y
D=D+M
@chained
M=D

(END)
@END
0;JMP