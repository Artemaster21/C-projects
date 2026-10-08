// Test multiplication with constants
// Multiplication loop for 3*4
@TEMP_0
M=0
@3
D=A
@TEMP_1
M=D
@MULT_END_1
D;JEQ
(MULT_LOOP_1)
@4
D=A
@TEMP_0
M=D+M
@TEMP_1
M=M-1
D=M
@MULT_LOOP_1
D;JGT
(MULT_END_1)
@TEMP_0
D=M
@prod1
M=D

// 0*5 = 0
@0
D=A
@prod2
M=D

// 1*10 = 10
@10
D=A
@prod3
M=D

// Test multiplication with variables
@5
D=A
@m
M=D
@6
D=A
@n
M=D

// Multiplication loop for m*n
@TEMP_2
M=0
@m
D=M
@TEMP_3
M=D
@MULT_END_2
D;JEQ
(MULT_LOOP_2)
@n
D=M
@TEMP_2
M=D+M
@TEMP_3
M=M-1
D=M
@MULT_LOOP_2
D;JGT
(MULT_END_2)
@TEMP_2
D=M
@result
M=D

// Test multiplication edge cases
@0
D=A
@zero1
M=D
@0
D=A
@zero2
M=D

// m*m = 5*5
@TEMP_4
M=0
@m
D=M
@TEMP_5
M=D
@MULT_END_3
D;JEQ
(MULT_LOOP_3)
@m
D=M
@TEMP_4
M=D+M
@TEMP_5
M=M-1
D=M
@MULT_LOOP_3
D;JGT
(MULT_END_3)
@TEMP_4
D=M
@same
M=D

(END)
@END
0;JMP