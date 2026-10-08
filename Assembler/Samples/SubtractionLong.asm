// Test subtraction with constants
@7
D=A
@diff1
M=D

// Test subtraction with variables
@15
D=A
@a
M=D
@5
D=A
@b
M=D
@a
D=M
@b
D=D-M
@result
M=D

// Test edge cases
@0
D=A
@zero
M=D
@65532
D=A
@negative
M=D

// Test with immediate values
@a
D=M
@3
D=D-A
@immediate
M=D
@10
D=A
@b
D=D-M
@immediate2
M=D

(END)
@END
0;JMP