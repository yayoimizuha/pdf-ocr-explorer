using System.Runtime.InteropServices.JavaScript;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;

namespace PDF_OCR_Explorer {
    public partial class MainPage : ContentPage {
        private readonly IFilePicker _filePicker;
        int _count = 0;

        internal const string Endpoint = "https://tomokazu-katayama-1.cognitiveservices.azure.com/";
        internal const string Key1 = "90536c6541af426eab1c6e8e4a8cdafe";
        internal const string Key2 = "dd1eeaea8843491dadb258f8642b22ec";
        internal DirectoryReader DirectoryReader;

        public class Thumbnail {
            public string FileName { get; set; }
            public string OrigFile { get; set; }
            public byte ThumbData { get; set; }
        }

        private static Grid AddThumb(string fp) {
            var grid = new Grid {
                HeightRequest = 400,
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(75) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions = {
                    new ColumnDefinition()
                }
            };
            grid.Add(
                new Label {
                    Text = Path.GetFileName(fp),
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 30
                }
                , 0, 0
            );
            grid.Add(
                new BoxView {
                    Color = Colors.Orange
                }, 0, 1
            );
            return grid;
        }


        public MainPage() {
            var azureKeyCredential = new AzureKeyCredential(Key1);
            var documentAnalysisClient =
                new DocumentAnalysisClient(new Uri(Endpoint), azureKeyCredential);
            InitializeComponent();

            var manager = new Manager.FileList();

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            // DirectoryReader = new DirectoryReader();
            //Label.Text = string.Join(Environment.NewLine, DirectoryReader.Files);
            //foreach (var filePath in DirectoryReader.Files){
            foreach (var file in manager.Files) {
                AddThumb(fp: file.FilePath);
            }
            
            //}
        }


        private void ThumbnailView_Loaded(object sender, EventArgs e) {
            ThumbnailColumnDefinition.Width = Math.Min(400, Window.Width * .2);
        }

        private void Window_SizeChanged(object sender, EventArgs e) {
            ThumbnailView_Loaded(sender, e);
        }


        private readonly PickOptions _pickOptions = new() {
            PickerTitle = "スキャンするファイルを選択してください。",
            FileTypes = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>> {
                    { DevicePlatform.WinUI, FilePickerFileType.Images.Value.Append(".pdf") },
                })
        };

        private async void FilePickerButton_OnClicked(object sender, EventArgs e) {
            var pickerRes = await FilePicker.PickMultipleAsync(_pickOptions);
            var addFiles = "";
            foreach (var fileResult in pickerRes) {
                Label.Text += Environment.NewLine + fileResult.FullPath;
                addFiles += fileResult.FileName + Environment.NewLine;
                DirectoryReader.DocumentAdd(fileResult.FullPath);
                ThumbnailStack.Children.Add(
                    AddThumb(fileResult.FullPath)
                );
            }

            if (addFiles.Length != 0) {
                await DisplayAlert("追加されたファイル", addFiles, "OK");
            }
        }
    }
}