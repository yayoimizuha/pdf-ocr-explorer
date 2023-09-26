﻿using System.Text.Json;

namespace PDF_OCR_Explorer;

public partial class SearchPage {
    private readonly Label _logViewer;
    private readonly Manager.FileList _fileList;

    public SearchPage(Label logViewer, Manager.FileList fileList) {
        InitializeComponent();
        _logViewer = logViewer;
        _fileList = fileList;
    }

    private IView AddQueryResult(string fileHash, uint page, string lineText, string queryText) {
        const int fontSize = 36;
        var grid = new Grid {
            HeightRequest = 120,
            ColumnDefinitions = {
                new ColumnDefinition { Width = new GridLength(120) }, //Thumbnail
                new ColumnDefinition { Width = new GridLength(540) }, //DispName
                new ColumnDefinition { Width = new GridLength(180) }, //page
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // line text
            },
            Margin = new Thickness(6),
        };
        _logViewer.Text += Environment.NewLine + fileHash;
        grid.Add(new Image {
            Source = _fileList.Files.Find(value => value.FileHash.Equals(fileHash)).ThumbImage(),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        }, 0);
        grid.Add(new Label {
            Text = _fileList.Files.Find(value => value.FileHash.Equals(fileHash)).DispName,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = fontSize
        }, 1);
        grid.Add(new Label {
            Text = $"{page + 1}ページ目",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = fontSize
        }, 2);
        var splitStr = $" {lineText} ".Split(queryText, StringSplitOptions.None);
        var formattedString = new FormattedString();
        formattedString.Spans.Add(new Span { Text = splitStr[0].Remove(0, 1) });
        formattedString.Spans.Add(new Span { Text = queryText, TextColor = Colors.Red });
        formattedString.Spans.Add(new Span { Text = splitStr[1].Remove(splitStr[1].Length - 1) });
        grid.Add(new Label {
            FormattedText = formattedString,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = fontSize
        }, 3);

        return grid;
    }

    private void SearchEntry_OnCompleted(object sender, EventArgs e) {
        _logViewer.Text += Environment.NewLine + "Search Queried: " + Manager.PackComma(SearchEntry.Text);
        if (SearchEntry.Text == "") {
            return;
        }
        SearchRes.Clear();

        foreach (var directory in Directory.GetDirectories(Manager.ApplicationDataRoot)) {
            var jsonPath = Path.Combine(Manager.ApplicationDataRoot, directory, "ocr.json");
            if (!File.Exists(jsonPath)) {
                continue;
            }

            if (new FileInfo(jsonPath).Length == 0) {
                continue;
            }

            var ocrData =
                JsonSerializer.Deserialize<OcrResultJson>(File.ReadAllText(jsonPath));
            for (var pageNum = 0; pageNum < ocrData.Pages.Length; pageNum++) {
                foreach (var documentLine in ocrData.Pages[pageNum].Lines) {
                    documentLine.Content = documentLine.Content.Replace(" ", "");
                    if (documentLine.Content.Contains(SearchEntry.Text, StringComparison.Ordinal)) {
                        SearchRes.Add(AddQueryResult(fileHash: Path.GetFileName(directory), page: (uint)pageNum,
                            lineText: documentLine.Content, queryText: SearchEntry.Text));
                    }
                }
            }
        }
    }
}