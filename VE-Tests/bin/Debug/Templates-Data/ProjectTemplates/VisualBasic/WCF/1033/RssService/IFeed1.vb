' NOTE: You can use the "Rename" command on the context menu to change the interface name "IFeed1" in both code and config file together.
<ServiceContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
<ServiceKnownType(GetType(Atom10FeedFormatter))>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
<ServiceKnownType(GetType(Rss20FeedFormatter))>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
Public Interface IFeed1

    <OperationContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
    <WebGet(UriTemplate:="*", BodyStyle:=WebMessageBodyStyle.Bare)>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
    Function CreateFeed() As SyndicationFeedFormatter

    ' TODO: Add your service operations here

End Interface
