Imports MetadataExtractor

Class MainWindow
    Dim allMediaFiles As New List(Of String)
    Dim maximumPlayers = MySettings.Default.MaxMediaPlayers
    Dim playerHeight = MySettings.Default.PlayerHeight
    Dim playedIndices As New List(Of Integer)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim allFiles = System.IO.Directory.GetFiles(MySettings.Default.MediaRootFolder)
        For Each fileName In allFiles
            Dim information = New System.IO.FileInfo(fileName)
            If information.Name.EndsWith(".mp4") And Not information.Name.StartsWith(".") Then allMediaFiles.Add(information.FullName)
        Next
        If maximumPlayers < 1 Then maximumPlayers = 1
        mainPanel.Children.Clear()
        For counter As Integer = 1 To maximumPlayers
            Dim mediaPlayer As New MediaElement()
            Dim randomIndex = GetRandomIndex(allMediaFiles.Count - 1)
            mediaPlayer.LoadedBehavior = MediaState.Manual
            mediaPlayer.Width = Me.ActualWidth * 0.45
            mediaPlayer.Height = Me.ActualHeight * 0.45
            mediaPlayer.Volume = 0

            Dim filePath = allMediaFiles(randomIndex)
            LoadFile(filePath, mediaPlayer)

            AddHandler mediaPlayer.MediaEnded, AddressOf HandleMediaEnded
            AddHandler mediaPlayer.MediaFailed, AddressOf HandleMediaEnded
            mainPanel.Children.Add(mediaPlayer)
        Next
    End Sub

    Private Sub HandleMediaEnded(source As Object, e As RoutedEventArgs)
        Dim mediaPlayer As MediaElement = source
        Dim randomIndex = GetRandomIndex(allMediaFiles.Count - 1)
        'playedTill += 1
        'If playedTill >= allMediaFiles.Count Then playedTill = 0
        LoadFile(allMediaFiles(randomIndex), mediaPlayer)
    End Sub

    Private Sub LoadFile(filePath As String, ByRef mediaPlayer As MediaElement)
        Dim rotation = 0
        Dim metadata = ImageMetadataReader.ReadMetadata(filePath)
        For Each m In metadata
            For Each t In m.Tags
                If t.HasName And t.Name = "Rotation" Then
                    rotation = t.Description
                End If
            Next
        Next

        Dim transformGroup As New TransformGroup
        transformGroup.Children.Add(New RotateTransform(rotation))
        mediaPlayer.LayoutTransform = transformGroup
        mediaPlayer.Source = New Uri(filePath)
        mediaPlayer.Play()
    End Sub

    Private Function GetRandomIndex(maximum As Integer) As Integer
        Dim random As New Random
        Dim result As Integer = random.Next(0, maximum)
        If playedIndices.Count / allMediaFiles.Count < 0.8 Then
            While playedIndices.Contains(result)
                result = random.Next(0, maximum)
            End While
        End If

        playedIndices.Add(result)
        Return result
    End Function

End Class
