Class MainWindow
    Dim playedTill As Integer
    Dim allMediaFiles As New List(Of String)
    Dim maximumPlayers = MySettings.Default.MaxMediaPlayers
    Dim playerWidth = MySettings.Default.PlayerWidth

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim allFiles = IO.Directory.GetFiles(MySettings.Default.MediaRootFolder)
        allMediaFiles = allFiles.Where(Function(fileName As String) fileName.EndsWith(".mp4")).ToList()
        If maximumPlayers < 1 Then maximumPlayers = 1
        mainWrapPanel.Children.Clear()
        For filePointer As Integer = 1 To maximumPlayers
            Dim mediaPlayer As New MediaElement()
            mediaPlayer.LoadedBehavior = MediaState.Manual
            mediaPlayer.Width = playerWidth
            mediaPlayer.Source = New Uri(allMediaFiles(filePointer))
            AddHandler mediaPlayer.MediaEnded, AddressOf HandleMediaEnded
            AddHandler mediaPlayer.MediaFailed, AddressOf HandleMediaEnded
            mainWrapPanel.Children.Add(mediaPlayer)
            playedTill = filePointer - 1
            mediaPlayer.Play()
        Next
    End Sub

    Private Sub HandleMediaEnded(source As Object, e As RoutedEventArgs)
        Dim mediaPlayer As MediaElement = source
        playedTill += 1
        If playedTill >= allMediaFiles.Count Then playedTill = 0
        mediaPlayer.Source = New Uri(allMediaFiles(playedTill))
        mediaPlayer.Play()
    End Sub

End Class
