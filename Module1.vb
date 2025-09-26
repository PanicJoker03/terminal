Imports System
Imports System.IO

Module Program

    Sub ModuleONE()
        Dim priority_a As String = ""
        Dim priority_b As String = ""
        Dim priority_c As String = ""

        Console.WriteLine("----------------------------")
        Console.WriteLine("  PRIORITIES CHECK V.1.0.3  ")
        Console.WriteLine("----------------------------")
        Console.WriteLine("HELLO WORLD")
        Console.WriteLine("WHAT'S THE PRIORITY A TODAY:")
        priority_a = Console.ReadLine().ToUpper()

        Console.WriteLine("WHAT'S THE PRIORITY B TODAY:")
        priority_b = Console.ReadLine().ToUpper()

        Console.WriteLine("WHAT'S THE PRIORITY C TODAY:")
        priority_c = Console.ReadLine().ToUpper()

        Dim todaySign = DateTime.Now.ToString("ddMMyyyy")
        Console.WriteLine("OK: " & priority_a)
        Console.WriteLine("OK: " & priority_b)
        Console.WriteLine("OK: " & priority_c)
        'TODO: Dump it to txt file in ddMMyyyy format
        Dim outPath As String = "OUT_" & todaySign & ".TXT"
        Using writer As New StreamWriter(outPath, append:=False)
            writer.WriteLine(todaySign & " " & "PRIORITY A IS: " & priority_a)
            writer.WriteLine(todaySign & " " & "PRIORITY B IS: " & priority_b)
            writer.WriteLine(todaySign & " " & "PRIORITY C IS: " & priority_c)

            Dim anotherTask As String = "n"
            While True

                Console.WriteLine("ANOTHER TASK TODAY OR NOW (YES/NO):")
                anotherTask = Console.ReadLine().ToUpper()

                If anotherTask = "Y" Or anotherTask = "YES" Then
                    Dim which = ""
                    Console.WriteLine("WHICH:")
                    which = Console.ReadLine().ToUpper()
                    writer.WriteLine("ANOTHER TASK IS: " & which)
                ElseIf anotherTask = "N" Or anotherTask = "NO" Then
                    Exit While
                Else
                    Console.WriteLine("I DO NOT UNDERSTAND THAT")
                End If
            End While
        End Using
        Console.WriteLine("PRESS ANY KEY TO FINALIZE PROGRAM...")
        Console.ReadKey()

    End Sub
    Sub ModuleTWO()
        'Count to ten
        Console.WriteLine("COUNT TO TEN: ")
        For counter = 1 To 10
            Threading.Thread.Sleep(1000)
            Console.WriteLine(++counter)
        Next
        Console.ReadKey()
        Console.WriteLine("PRESS ANY KEY TO EXIT PROGRAM...")
    End Sub
    Sub ModuleTHREE()
        Dim reality As Integer = New Random().Next(0, 2)

        Console.WriteLine("REALITY CHECK")
        Console.WriteLine(reality)
        Console.WriteLine("PRESS ANY KEY TO FINALIZE PROGRAM...")
        Console.ReadKey()
    End Sub

    Sub Main(args As String())
        While True
            Console.WriteLine("WHICH:")
            Dim optTxt As String = Console.ReadLine()
            Dim opt As Integer = 0
            Integer.TryParse(optTxt, opt)
            If opt = 1 Then
                ModuleONE()
                Exit Sub
            End If

            If opt = 2 Then
                ModuleTWO()
                Exit Sub
            End If

            If opt = 3 Then
                ModuleTHREE()
                Exit Sub
            End If

        End While
    End Sub
End Module
