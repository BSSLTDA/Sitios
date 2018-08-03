Public Class Generator
    'token()
    Public Function Token() As String
        Token = RandomString() & RandomString() & RandomString() & RandomString() & RandomString() & RandomString()
    End Function

    'RandomString
    Function RandomString()
        Dim numbers = {({7, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"}), ({1, "0123456789"})}
        Randomize()

        Dim Count_
        Dim Chars
        Dim Index_
        Dim temp As String = ""

        For i = 0 To UBound(numbers)
            Count_ = numbers(i)(0)
            Chars = numbers(i)(1)
            For j = 1 To Count_
                Index_ = Int(Rnd() * Len(Chars)) + 1
                temp = temp & Mid(Chars, Index_, 1)
            Next
        Next

        Dim TempCopy As String = ""

        Do Until Len(temp) = 0
            Index_ = Int(Rnd() * Len(temp)) + 1
            TempCopy = TempCopy & Mid(temp, Index_, 1)
            temp = Mid(temp, 1, Index_ - 1) & Mid(temp, Index_ + 1)
        Loop
        RandomString = TempCopy
    End Function
End Class
