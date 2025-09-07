' Gambas module file

Public Sub Main()
  Dim priority_a As String = ""
  Dim priority_b As String = ""
  Dim priority_c As String = ""
  
  Print "-----------------------------" & "\n"
  Print "  PRIORITIES CHECK V.2.0.1   " & "\n"
  Print "-----------------------------" & "\n"
  
  Print "HELLO WORLD" & "\n"
  Print "WHAT'S THE PRIORITY A TODAY:" & "\n"
  Line Input priority_a
  priority_a = UCase(priority_a)
  
  Print "WHAT'S THE PRIORITY B TODAY:" & "\n"
  Line Input priority_b
  priority_b = UCase(priority_b)
  
  Print "WHAT'S THE PRIORITY C TODAY:" & "\n"
  Line Input priority_c
  priority_c = UCase(priority_c)
  
  Dim todaySign As String = Format(Now, "ddmmyyyy") & ".TXT"
  Print "OK: " & priority_a & "\n"
  Print "OK: " & priority_b & "\n"
  Print "OK: " & priority_c & "\n"
  
  'TODO: Dump it to txt file in ddMMyyyy format
  Dim outPath As String = User.Home & "/OUT_" & todaySign 
  Dim outLecture As Stream
  outLecture = Open outPath For Create
  Print #outLecture, todaySign & " " & "PRIORITY A IS: " & priority_a & "\n"
  Print #outLecture, todaySign & " " & "PRIORITY B IS: " & priority_b & "\n"
  Print #outLecture, todaySign & " " & "PRIORITY C IS: " & priority_c & "\n"

  Dim anotherTask As String = "n"
  While True
    Print "ANOTHER TASK TODAY OR NOW (YES/NO):" & "\n" 
    Line Input anotherTask 
    anotherTask = UCase(anotherTask)
    
    If anotherTask = "Y" Or anotherTask = "YES" Then
      Dim which As String = ""
      Write "WHICH:" & "\n"
      Line Input which 
      which = UCase(which)
      Write #outLecture, "ANOTHER TASK IS: " & which & "\n"
    Else If anotherTask = "N" Or anotherTask = "NO" Then
      Exit 
    Else
      Write "I DO NOT UNDERSTAND THAT" & "\n"
    End If
  Wend 
'End 
End
