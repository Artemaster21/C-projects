// Test division with constants
// Division loop for 12/3
@TEMP_0
M=0
@3
D=A
@DIV_END_1
D;JEQ
@12
D=A
@DIV_END_1
D;JEQ
@TEMP_1
M=D
(DIV_LOOP_1)
@TEMP_1
D=M
@3
D=D-A
@DIV_END_LOOP_1
D;JLT
@TEMP_1
M=D
@TEMP_0
M=M+1
@DIV_LOOP_1
0;JMP
(DIV_END_LOOP_1)
@TEMP_0
D=M
@quot1
M=D
(DIV_END_1)

// Division loop for 10/3
@TEMP_2
M=0
@3
D=A
@DIV_END_2
D;JEQ
@10
D=A
@DIV_END_2
D;JEQ
@TEMP_3
M=D
(DIV_LOOP_2)
@TEMP_3
D=M
@3
D=D-A
@DIV_END_LOOP_2
D;JLT
@TEMP_3
M=D
@TEMP_2
M=M+1
@DIV_LOOP_2
0;JMP
(DIV_END_LOOP_2)
@TEMP_2
D=M
@quot2
M=D
(DIV_END_2)

// 1/1 = 1
@1
D=A
@quot3
M=D

// Test division with variables
@20
D=A
@p
M=D
@4
D=A
@q
M=D

// Division loop for p/q
@TEMP_4
M=0
@q
D=M
@DIV_END_3
D;JEQ
@p
D=M
@DIV_END_3
D;JEQ
@TEMP_5
M=D
(DIV_LOOP_3)
@TEMP_5
D=M
@q
D=D-M
@DIV_END_LOOP_3
D;JLT
@TEMP_5
M=D
@TEMP_4
M=M+1
@DIV_LOOP_3
0;JMP
(DIV_END_LOOP_3)
@TEMP_4
D=M
@div_result
M=D
(DIV_END_3)

// Test edge cases
@1
D=A
@self
M=D
@20
D=A
@one
M=D

(END)
@END
0;JMP