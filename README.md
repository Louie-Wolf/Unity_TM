# Unity_TM
A universal turing machine simulated with Unity Version 2021.3.11f1

## Universal Turing Machine Code:
### Symbols - seperated with 1's (e.g. ...1010001001...)
Can be defined by the user in the TapeManager Inspector.  
x1 : any symbol (0)  
x2 : any symbol (00)  
x3 : any symbol (000)  

### States - seperated with 11's (e.g. ...110100101...)
q1 : start state (0)  
q2 : accepted state (00)  
q3... : normal state (000)  
qn : n normal states (0*n)  

### Input - seperated with 111 at the end of TM (e.g ...1110100101...)
Input (Symbols) to put on the tape.  

### Direction
L : Left (0)  
R : Right (00)  
  
![TM Example](Screenshot_TM.png)
