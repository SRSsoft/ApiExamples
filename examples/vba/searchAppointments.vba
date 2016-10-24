Option Explicit

Const m_baseUri As String = "https://[yourServerName]/SRSAPI/Generic/"

Function login(userName As String, password As String) As String

    Dim objHTTP As New MSXML2.XMLHTTP60
    Dim URL As String
    
    Set objHTTP = CreateObject("MSXML2.ServerXMLHTTP")
    URL = m_baseUri & "Token"
    objHTTP.Open "POST", URL, False
    objHTTP.setRequestHeader "Content-Type", "application/json"
    objHTTP.send ("{""userName"":""" & userName & """,""password"":""" & password & """,""dataSourceId"":0}")
    login = objHTTP.responseText

End Function

Sub searchAppointments()

    Dim personId As String
    personId = "9783"

    Dim token As String
    token = login("yourUserName", "yourPassword")
    
    Dim objHTTP As New MSXML2.XMLHTTP60
    Dim URL As String
    
    Set objHTTP = CreateObject("MSXML2.ServerXMLHTTP")
    URL = m_baseUri & "Appointment?PersonId=" & personId
    objHTTP.Open "GET", URL, False
    objHTTP.setRequestHeader "Authorization", token
    objHTTP.send
    Debug.Print objHTTP.responseText

End Sub
