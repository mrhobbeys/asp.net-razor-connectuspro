
Imports System
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports SiteBlue.Data.EightHundred

Module EncryptMe

    ' <summary>
    ' This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and 
    ' decrypt data. As long as encryption and decryption routines use the same 
    ' parameters to generate the keys, the keys are guaranteed to be the same.
    ' The class uses static functions with duplicate code to make it easier to 
    ' demonstrate encryption and decryption logic. In a real-life application, 
    ' this may not be the most efficient way of handling encryption, so - as 
    ' soon as you feel comfortable with it - you may want to redesign this class.
    ' </summary>
    Public Class RijndaelSimple

        ' <summary>
        ' Encrypts specified plaintext using Rijndael symmetric key algorithm
        ' and returns a base64-encoded result.
        ' </summary>
        ' <param name="plainText">
        ' Plaintext value to be encrypted.
        ' </param>
        ' <param name="passPhrase">
        ' Passphrase from which a pseudo-random password will be derived. The 
        ' derived password will be used to generate the encryption key. 
        ' Passphrase can be any string. In this example we assume that this 
        ' passphrase is an ASCII string.
        ' </param>
        ' <param name="saltValue">
        ' Salt value used along with passphrase to generate password. Salt can 
        ' be any string. In this example we assume that salt is an ASCII string.
        ' </param>
        ' <param name="hashAlgorithm">
        ' Hash algorithm used to generate password. Allowed values are: "MD5" and
        ' "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        ' </param>
        ' <param name="passwordIterations">
        ' Number of iterations used to generate password. One or two iterations
        ' should be enough.
        ' </param>
        ' <param name="initVector">
        ' Initialization vector (or IV). This value is required to encrypt the 
        ' first block of plaintext data. For RijndaelManaged class IV must be 
        ' exactly 16 ASCII characters long.
        ' </param>
        ' <param name="keySize">
        ' Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        ' Longer keys are more secure than shorter keys.
        ' </param>
        ' <returns>
        ' Encrypted value formatted as a base64-encoded string.
        ' </returns>
        Public Shared Function Encrypt(ByVal plainText As String, _
                                       ByVal passPhrase As String, _
                                       ByVal saltValue As String, _
                                       ByVal hashAlgorithm As String, _
                                       ByVal passwordIterations As Integer, _
                                       ByVal initVector As String, _
                                       ByVal keySize As Integer) _
                               As String

            ' Convert strings into byte arrays.
            ' Let us assume that strings only contain ASCII codes.
            ' If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            ' encoding.
            Dim initVectorBytes As Byte()
            initVectorBytes = Encoding.ASCII.GetBytes(initVector)

            Dim saltValueBytes As Byte()
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue)

            ' Convert our plaintext into a byte array.
            ' Let us assume that plaintext contains UTF8-encoded characters.
            Dim plainTextBytes As Byte()
            plainTextBytes = Encoding.UTF8.GetBytes(plainText)

            ' First, we must create a password, from which the key will be derived.
            ' This password will be generated from the specified passphrase and 
            ' salt value. The password will be created using the specified hash 
            ' algorithm. Password creation can be done in several iterations.
            Dim password = New Rfc2898DeriveBytes(passPhrase, _
                                               saltValueBytes, _
                                               passwordIterations)

            ' Use the password to generate pseudo-random bytes for the encryption
            ' key. Specify the size of the key in bytes (instead of bits).
            Dim keyBytes As Byte()
            keyBytes = password.GetBytes(Convert.ToInt32(keySize / 8))

            ' Create uninitialized Rijndael encryption object.
            Dim symmetricKey As RijndaelManaged
            symmetricKey = New RijndaelManaged()

            ' It is reasonable to set encryption mode to Cipher Block Chaining
            ' (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC

            ' Generate encryptor from the existing key bytes and initialization 
            ' vector. Key size will be defined based on the number of the key 
            ' bytes.
            Dim encryptor As ICryptoTransform
            encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)

            ' Define memory stream which will be used to hold encrypted data.
            Dim memoryStream As MemoryStream
            memoryStream = New MemoryStream()

            ' Define cryptographic stream (always use Write mode for encryption).
            Dim cryptoStream As CryptoStream
            cryptoStream = New CryptoStream(memoryStream, _
                                            encryptor, _
                                            CryptoStreamMode.Write)
            ' Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)

            ' Finish encrypting.
            cryptoStream.FlushFinalBlock()

            ' Convert our encrypted data from a memory stream into a byte array.
            Dim cipherTextBytes As Byte()
            cipherTextBytes = memoryStream.ToArray()

            ' Close both streams.
            memoryStream.Close()
            cryptoStream.Close()

            ' Convert encrypted data into a base64-encoded string.
            Dim cipherText As String
            cipherText = Convert.ToBase64String(cipherTextBytes)

            ' Return encrypted string.
            Encrypt = cipherText

        End Function    'Encrypt

        ' <summary>
        ' Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        ' </summary>
        ' <param name="cipherText">
        ' Base64-formatted ciphertext value.
        ' </param>
        ' <param name="passPhrase">
        ' Passphrase from which a pseudo-random password will be derived. The 
        ' derived password will be used to generate the encryption key. 
        ' Passphrase can be any string. In this example we assume that this 
        ' passphrase is an ASCII string.
        ' </param>
        ' <param name="saltValue">
        ' Salt value used along with passphrase to generate password. Salt can 
        ' be any string. In this example we assume that salt is an ASCII string.
        ' </param>
        ' <param name="hashAlgorithm">
        ' Hash algorithm used to generate password. Allowed values are: "MD5" and
        ' "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        ' </param>
        ' <param name="passwordIterations">
        ' Number of iterations used to generate password. One or two iterations
        ' should be enough.
        ' </param>
        ' <param name="initVector">
        ' Initialization vector (or IV). This value is required to encrypt the 
        ' first block of plaintext data. For RijndaelManaged class IV must be 
        ' exactly 16 ASCII characters long.
        ' </param>
        ' <param name="keySize">
        ' Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        ' Longer keys are more secure than shorter keys.
        ' </param>
        ' <returns>
        ' Decrypted string value.
        ' </returns>
        ' <remarks>
        ' Most of the logic in this function is similar to the Encrypt 
        ' logic. In order for decryption to work, all parameters of this function
        ' - except cipherText value - must match the corresponding parameters of 
        ' the Encrypt function which was called to generate the 
        ' ciphertext.
        ' </remarks>
        Public Shared Function Decrypt(ByVal cipherText As String, _
                                       ByVal passPhrase As String, _
                                       ByVal saltValue As String, _
                                       ByVal hashAlgorithm As String, _
                                       ByVal passwordIterations As Integer, _
                                       ByVal initVector As String, _
                                       ByVal keySize As Integer) _
                               As String

            ' Convert strings defining encryption key characteristics into byte
            ' arrays. Let us assume that strings only contain ASCII codes.
            ' If strings include Unicode characters, use Unicode, UTF7, or UTF8
            ' encoding.
            Dim initVectorBytes As Byte()
            initVectorBytes = Encoding.ASCII.GetBytes(initVector)

            Dim saltValueBytes As Byte()
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue)

            ' Convert our ciphertext into a byte array.
            Dim cipherTextBytes As Byte()
            cipherTextBytes = Convert.FromBase64String(cipherText)

            ' First, we must create a password, from which the key will be 
            ' derived. This password will be generated from the specified 
            ' passphrase and salt value. The password will be created using
            ' the specified hash algorithm. Password creation can be done in
            ' several iterations.
            Dim password = New Rfc2898DeriveBytes(passPhrase, _
                                               saltValueBytes, _
                                               passwordIterations)

            ' Use the password to generate pseudo-random bytes for the encryption
            ' key. Specify the size of the key in bytes (instead of bits).
            Dim keyBytes As Byte()
            keyBytes = password.GetBytes(Convert.ToInt32(keySize / 8))

            ' Create uninitialized Rijndael encryption object.
            Dim symmetricKey As RijndaelManaged
            symmetricKey = New RijndaelManaged()

            ' It is reasonable to set encryption mode to Cipher Block Chaining
            ' (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC

            ' Generate decryptor from the existing key bytes and initialization 
            ' vector. Key size will be defined based on the number of the key 
            ' bytes.
            Dim decryptor As ICryptoTransform
            decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

            ' Define memory stream which will be used to hold encrypted data.
            Dim memoryStream As MemoryStream
            memoryStream = New MemoryStream(cipherTextBytes)

            ' Define memory stream which will be used to hold encrypted data.
            Dim cryptoStream As CryptoStream
            cryptoStream = New CryptoStream(memoryStream, _
                                            decryptor, _
                                            CryptoStreamMode.Read)

            ' Since at this point we don't know what the size of decrypted data
            ' will be, allocate the buffer long enough to hold ciphertext;
            ' plaintext is never longer than ciphertext.
            Dim plainTextBytes As Byte()
            ReDim plainTextBytes(cipherTextBytes.Length)

            ' Start decrypting.
            Dim decryptedByteCount As Integer
            decryptedByteCount = cryptoStream.Read(plainTextBytes, _
                                                   0, _
                                                   plainTextBytes.Length)

            ' Close both streams.
            memoryStream.Close()
            cryptoStream.Close()

            ' Convert decrypted data into a string. 
            ' Let us assume that the original plaintext string was UTF8-encoded.
            Dim plainText As String
            plainText = Encoding.UTF8.GetString(plainTextBytes, _
                                                0, _
                                                decryptedByteCount)

            ' Return decrypted string.
            Decrypt = plainText

        End Function    'Decrypt


    End Class    'RijndaelSimple

    ', do NOT these values alter after saving data, you will NOT be able to decrypt it!!!!
    Public passPhrase As String = "*1Lor!*"            ' can be any string
    Public saltValue As String = "s@1tP3pp3r"          ' can be any string
    Public hashAlgorithm As String = "SHA1"            ' can be "MD5"
    Public passwordIterations As Integer = 2           ' can be any number
    Public initVector As String = "@1B2c3D4e5F6g7H8"   ' must be 16 bytes
    Public keySize As Integer = 256                    ' can be 192 or 128
    Public CryptName As String = "Toms Encryptor Message"

    Enum CryptTables
        tblACH = 1
        tblEmployee = 2
        tblUser = 3
        tbl_Payments = 4
        tbl_Payroll = 5
    End Enum

    Enum CryptFields
        AchTransitNumber = 11
        AchDFIAcct = 12
        AchReceivername = 13
        EmpDriverLicNumber = 21
        EmpSSNumber = 22
        EmpBirthDate = 23
        UserPassword = 31
        PaymentDLNumber = 41
        PaymentCheckNumber = 42
        PayrollTotCommission = 51
        PayrollGrossPay = 52
    End Enum

    '4 functions
    ' Public Function CryptMyString(ByVal TextValue As String) As String
    '   pass it any text string and it returns an excrypted string

    'Public Function DeCryptMyString(ByVal CipherText As String) As String
    '   pass it any encrytped string and it returns a plain text string

    'Public Function CryptSaveMyData(ByVal TableID As Integer, ByVal FieldId As Integer, ByVal KeyID As Integer, ByVal TextValue As String, ByVal tmpDebug As Boolean) As String
    '    pass it a tableid, fieldid, keyid, plain text, it encrypts the data and stores it in a tbl_Encryptions
    '    see table and field Enums above, add any table or field enum as needed

    ' Public Function DeCryptMySavedData(ByVal TableID As Integer, ByVal FieldId As Integer, ByVal KeyID As Integer, ByVal tmpDebug As Boolean) As String
    '    pass it a tableid, fieldid, keyid and it reads the tbl_Enxcryptions and decrypts the value and returns a plain text string
    '    see table and field Enums above, add any table or field enum as needed


    Public Function CryptMyString(ByVal TextValue As String) As String
        Dim cipherText As String
        Try
            cipherText = RijndaelSimple.Encrypt(TextValue, _
                                                passPhrase, _
                                                saltValue, _
                                                hashAlgorithm, _
                                                passwordIterations, _
                                                initVector, _
                                                keySize)
            CryptMyString = cipherText
        Catch ex As Exception
            CryptMyString = ""
        End Try

    End Function    'CryptMyString

    Public Function CryptSaveMyData(ByVal TableID As Integer, ByVal FieldId As Integer, ByVal KeyID As Integer, ByVal TextValue As String, ByVal tmpDebug As Boolean) As String
        If TextValue = "" Then
            CryptSaveMyData = ""
            Exit Function
        End If

        Dim cipherText As String
        Dim AllMessages = CryptName & Environment.NewLine & TextValue

        Try
            cipherText = CryptMyString(TextValue)
            CryptSaveMyData = cipherText
            AllMessages = AllMessages & Environment.NewLine & cipherText & Environment.NewLine & cipherText.Length.ToString
            Try
                Using db = CommonFunctions.Create_SQL_Connection()
                    Dim Oldrec = From O In db.tbl_Encryption Where O.TableID = TableID AndAlso O.FieldID = FieldId AndAlso O.KeyID = KeyID _
                                  Select O

                    If Oldrec.Count = 0 Then
                        Dim Newrec As New tbl_Encryption
                        Newrec.KeyID = KeyID
                        Newrec.FieldID = FieldId
                        Newrec.TableID = TableID
                        Newrec.FieldValue = cipherText
                        db.tbl_Encryption.AddObject(Newrec)
                        db.SaveChanges()

                    ElseIf Oldrec.Count = 1 Then
                        Oldrec.First.FieldValue = cipherText
                        db.SaveChanges()
                    Else
                        AllMessages = AllMessages & Environment.NewLine & "Multiple records for same table, key, is invalid data"
                    End If
                End Using
            Catch ex As Exception
                AllMessages = AllMessages & Environment.NewLine & ex.Message
            End Try

        Catch ex As Exception
            CryptSaveMyData = ""
            AllMessages = AllMessages & Environment.NewLine & ex.Message
        End Try

        If tmpDebug = True Then
            MessageBox.Show(AllMessages)
        End If

    End Function    'CryptSaveMyData

    Public Function DeCryptMyString(ByVal CipherText As String) As String
        Dim PlainText As String
        Try
            PlainText = RijndaelSimple.Decrypt(cipherText, _
                                    passPhrase, _
                                    saltValue, _
                                    hashAlgorithm, _
                                    passwordIterations, _
                                    initVector, _
                                    keySize)
            DeCryptMyString = PlainText
        Catch ex As Exception
            DeCryptMyString = ""
        End Try

    End Function    'DeCryptMyString

    Public Function DeCryptMySavedData(ByVal TableID As Integer, ByVal FieldId As Integer, ByVal KeyID As Integer, ByVal tmpDebug As Boolean) As String
        Dim CipherText As String
        Dim PlainText As String
        Dim AllMessages = CryptName

        Try
            Using db = CommonFunctions.Create_SQL_Connection()
                Dim Oldrec = From O In db.tbl_Encryption Where O.TableID = TableID AndAlso O.FieldID = FieldId AndAlso O.KeyID = KeyID _
                              Select O

                If Oldrec.Count = 0 Then
                    DeCryptMySavedData = ""
                ElseIf Oldrec.Count = 1 Then
                    CipherText = Oldrec.First.FieldValue
                    PlainText = DeCryptMyString(CipherText)
                    DeCryptMySavedData = PlainText
                    AllMessages = AllMessages & Environment.NewLine & PlainText & Environment.NewLine & PlainText.Length.ToString
                Else
                    DeCryptMySavedData = ""
                    AllMessages = AllMessages & Environment.NewLine & "Multiple records for same table, key, is invalid data"
                End If
            End Using
        Catch ex As Exception
            DeCryptMySavedData = ""
            AllMessages = AllMessages & Environment.NewLine & ex.Message
        End Try

        If tmpDebug = True Then
            MessageBox.Show(AllMessages)
        End If

    End Function    'DeCryptMySavedData


End Module

