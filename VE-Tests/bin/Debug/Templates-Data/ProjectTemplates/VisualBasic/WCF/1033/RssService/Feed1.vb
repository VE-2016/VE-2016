' NOTE: You can use the "Rename" command on the context menu to change the class name "Feed1" in both code and config file together.
Public Class Feed1
    Implements IFeed1

    Public Function CreateFeed() As SyndicationFeedFormatter Implements IFeed1.CreateFeed
        ' Create a new Syndication Feed.
        Dim feed As New SyndicationFeed("Feed Title", "A WCF Syndication Feed", Nothing)
        Dim items As New List(Of SyndicationItem)

        ' Create a new Syndication Item.
        Dim item As New SyndicationItem("An item", "Item content", Nothing)
        items.Add(item)
        feed.Items = items

        ' Return ATOM or RSS based on query string
        ' rss -> http://localhost:8733/Design_Time_Addresses/$safeprojectname$/Feed1/
        ' atom -> http://localhost:8733/Design_Time_Addresses/$safeprojectname$/Feed1/?format=atom
        Dim query As String = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.Get("format")
        Dim formatter As SyndicationFeedFormatter = Nothing
        If (query = "atom") Then
            formatter = New Atom10FeedFormatter(feed)
        Else
            formatter = New Rss20FeedFormatter(feed)
        End If

        Return formatter
    End Function

End Class
