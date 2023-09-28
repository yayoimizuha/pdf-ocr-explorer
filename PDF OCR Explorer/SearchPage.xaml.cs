using System.Text.Json;
using MauiIcons.Fluent;
using Image = Microsoft.Maui.Controls.Image;

namespace PDF_OCR_Explorer;

public partial class SearchPage{
    private readonly Label _logViewer;
    private readonly Manager.FileList _fileList;

    public SearchPage(Label logViewer, Manager.FileList fileList) {
        InitializeComponent();
        _logViewer = logViewer;
        _fileList = fileList;
    }

    private IView AddQueryResult(string fileHash, int page, Lines line, string queryText, double pageAngle,
        double pageHeight, double pageWidth) {
        const int fontSize = 36;
        var findRes = _fileList.Files.Find(value => value.FileHash.Equals(fileHash));
        var grid = new Grid {
            HeightRequest = 120,
            ColumnDefinitions = {
                new ColumnDefinition { Width = new GridLength(120) }, //Thumbnail
                new ColumnDefinition { Width = new GridLength(540) }, //DispName
                new ColumnDefinition { Width = new GridLength(180) }, //page
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // line text
                new ColumnDefinition { Width = new GridLength(120) } // line text
            },
            Margin = new Thickness(6),
        };
        _logViewer.Text += Environment.NewLine + fileHash;
        grid.Add(new Image {
            Source = findRes.ThumbImage(),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        }, 0);
        grid.Add(new Label {
            Text = findRes.DispName,
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
        var splitStr = $" {line.Content} ".Split(queryText, StringSplitOptions.None);
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

        // var gestureRecognizer = new TapGestureRecognizer();
        // gestureRecognizer.Tapped += (_, _) =>
        // {
        // };
        // grid.GestureRecognizers.Add(gestureRecognizer);
        var button = new ImageButton {
            Source = (ImageSource)new MauiIcon {
                Icon = FluentIcons.ContentView32
            }
        };
        button.Clicked += (_, _) =>
        {
            var window = new Window(new SearchView(lines: line, pageNum: page, poeFile: findRes, angle: pageAngle,
                width: pageWidth, height: pageHeight));
            if (Application.Current != null) Application.Current.OpenWindow(window);
        };
        grid.Add(button, 4);

        //grid.GestureRecognizers
        return grid;
    }

    private void SearchEntry_OnCompleted(object sender, EventArgs e) {
        _logViewer.Text += Environment.NewLine + "Search Queried: " + Manager.PackComma(SearchEntry.Text);
        if (SearchEntry.Text == ""){
            return;
        }

        SearchRes.Clear();

        foreach (var directory in Directory.GetDirectories(Manager.ApplicationDataRoot)){
            var jsonPath = Path.Combine(Manager.ApplicationDataRoot, directory, "ocr.json");
            if (!File.Exists(jsonPath)){
                continue;
            }

            if (new FileInfo(jsonPath).Length == 0){
                continue;
            }

            var ocrData =
                JsonSerializer.Deserialize<OcrResultJson>(File.ReadAllText(jsonPath));
            for (var pageNum = 0; pageNum < ocrData.Pages.Length; pageNum++){
                var angle = ocrData.Pages[pageNum].Angle;
                var height = ocrData.Pages[pageNum].Height;
                var width = ocrData.Pages[pageNum].Width;
                foreach (var documentLine in ocrData.Pages[pageNum].Lines){
                    documentLine.Content = documentLine.Content.Replace(" ", "");
                    if (documentLine.Content.Contains(SearchEntry.Text, StringComparison.Ordinal)){
                        SearchRes.Add(AddQueryResult(fileHash: Path.GetFileName(directory), page: pageNum,
                            line: documentLine, queryText: SearchEntry.Text, pageAngle: angle, pageWidth: width,
                            pageHeight: height));
                    }
                }
            }
        }
    }
}