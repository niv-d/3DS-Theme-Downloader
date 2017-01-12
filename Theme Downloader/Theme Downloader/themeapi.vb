Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq

Public Class themeapi
    Private pageNumber As Integer = 1
    Private id As String = "", query As String = ""

    Public Sub setPage(ByVal _pageNumber)
        pageNumber = _pageNumber
        If pageNumber <= 0 Then pageNumber = 1
    End Sub

    Public Function getPage()
        Return pageNumber
    End Function

    Public Function getPopular()
        Dim themeJson = fetchURL("https://3dsthem.es/getThemes.php?popular=&p=" + pageNumber.ToString)

        Dim result As JObject = JObject.Parse(themeJson)
        Dim themes = result("themes")

        Return themes
    End Function

    Public Sub setSearch(ByVal q)
        query = q
    End Sub

    Public Function getSearch()
        Dim themeJson = fetchURL("https://3dsthem.es/getThemes.php?q=" + query + "&p=" + pageNumber.ToString)

        Dim result As JObject = JObject.Parse(themeJson)
        Dim themes = result("themes")

        Return themes
    End Function

    Private Function fetchURL(ByVal url)
        Dim request As WebRequest = WebRequest.Create(url)
        Dim response As WebResponse = request.GetResponse()
        Dim dataStream As Stream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()

        Return responseFromServer
    End Function


    Public Function setID(ByVal _id As String)
        id = _id
    End Function

    Public Function getThemeJson()
        Dim themeJson = fetchURL("https://3dsthem.es/getTheme.php?i=" + id)

        Dim theme As JObject = JObject.Parse(themeJson)

        Return theme
    End Function

    Public Function getImage() As Image
        Using wc As New WebClient()
            Return Image.FromStream(wc.OpenRead("https://3dsthem.es/_/themes/" + id + "/Preview.png"))
        End Using
    End Function

    Public Function getDwnldURL() As String
        Return "https://3dsthem.es/downloadTheme.php?i=" + id
    End Function
End Class
