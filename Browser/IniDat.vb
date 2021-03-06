Public Module IniDat
    Public DAT_ErrorMsg As String = "DAT file decryption failed. Please check the password in the INI file."

    'Load DAT file
    Public Function LoadDat() As String
        LoadDat = String.Empty
        Dim fs As System.IO.FileStream = New System.IO.FileStream(DatFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(fs, System.Text.Encoding.Unicode)

        Try
            LoadDat = sr.ReadToEnd
            sr.Close()
            fs.Close()

        Catch ex As Exception
            g_Main.txtMsg.Text = "File loading error:" & ex.ToString
            sr.Close()
            fs.Close()
        End Try
    End Function

    'Save DAT file
    Public Sub SaveDat(ByRef cipherText As String)
        Dim fs As System.IO.FileStream = New System.IO.FileStream(DatFilePath, System.IO.FileMode.Create, IO.FileAccess.Write)
        Dim sw As System.IO.StreamWriter = New System.IO.StreamWriter(fs, System.Text.Encoding.Unicode)

        Try
            sw.Write(cipherText)
            sw.Flush()
            sw.Close()
            fs.Close()

        Catch ex As Exception
            g_Main.txtMsg.Text = "File saving error:" & ex.ToString
            sw.Close()
            fs.Close()
        End Try
    End Sub

    'AES (Advanced Encryption Standard) / Rijndael encryption
    'Code example: http://www.obviex.com/samples/encryption.aspx
    Public Function Encrypt(plainText As String) As String
        Dim passPhrase As String = Prog_Name & CStr("☼¥£¢¡ῧᾏ♫ﮋ﴾ƀ-‼₪Ω")
        Dim byteValue() As Byte = System.Text.Encoding.UTF8.GetBytes("95F75666F23D18F40809635CDA76AFD1")
        Dim saltValue As String = INI.GetSection(INI_Settings).GetKey(INI_PW).GetValue() & CStr("Ƀ")
        Dim initVector As String = "№βλ¢฿+∞☺" 'Must be 16 bytes
        Dim passwordIterations As Integer = 2
        Dim keySize As Integer = 256

        Dim initVectorBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(initVector)
        Dim saltValueBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(saltValue)
        Dim plainTextBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(plainText)

        Dim password As New Security.Cryptography.Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)
        Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

        Dim symmetricKey As New Security.Cryptography.RijndaelManaged()
        symmetricKey.Mode = Security.Cryptography.CipherMode.CBC

        Dim encryptor As Security.Cryptography.ICryptoTransform = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
        Dim memoryStream As New IO.MemoryStream()
        Dim cryptoStream As New Security.Cryptography.CryptoStream(memoryStream, encryptor, Security.Cryptography.CryptoStreamMode.Write)

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)
        cryptoStream.FlushFinalBlock()
        Dim cipherTextBytes As Byte() = memoryStream.ToArray()
        memoryStream.Close()
        cryptoStream.Close()
        'Encrypted Text
        Encrypt = Convert.ToBase64String(cipherTextBytes)
    End Function

    Public Function Decrypt(ByRef cipherText As String) As String
        Dim passPhrase As String = Prog_Name & CStr("☼¥£¢¡ῧᾏ♫ﮋ﴾ƀ-‼₪Ω")
        Dim byteValue() As Byte = System.Text.Encoding.UTF8.GetBytes("95F75666F23D18F40809635CDA76AFD1")
        Dim saltValue As String = INI.GetSection(INI_Settings).GetKey(INI_PW).GetValue() & CStr("Ƀ")
        Dim initVector As String = "№βλ¢฿+∞☺" 'Must be 16 bytes
        Dim passwordIterations As Integer = 2
        Dim keySize As Integer = 256

        Dim initVectorBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(initVector)
        Dim saltValueBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(saltValue)
        Dim cipherTextBytes As Byte() = Convert.FromBase64String(cipherText)

        Dim password As New Security.Cryptography.Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)
        Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

        Dim symmetricKey As New Security.Cryptography.RijndaelManaged()
        symmetricKey.Mode = Security.Cryptography.CipherMode.CBC  'Cipher Block Chaining (CBC)

        Dim decryptor As Security.Cryptography.ICryptoTransform = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)
        Dim memoryStream As New IO.MemoryStream(cipherTextBytes)

        Dim cryptoStream As New Security.Cryptography.CryptoStream(memoryStream, decryptor, Security.Cryptography.CryptoStreamMode.Read)
        Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}

        Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)
        memoryStream.Close()
        cryptoStream.Close()
        'Plain Text
        Decrypt = System.Text.Encoding.Unicode.GetString(plainTextBytes, 0, decryptedByteCount)
    End Function
End Module
