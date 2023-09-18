using System.Runtime.InteropServices.JavaScript;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;

namespace PDF_OCR_Explorer{
    public partial class MainPage : ContentPage{
        private readonly IFilePicker _filePicker;
        int _count = 0;

        internal const string Endpoint = "https://tomokazu-katayama-1.cognitiveservices.azure.com/";
        internal const string Key1 = "90536c6541af426eab1c6e8e4a8cdafe";
        internal const string Key2 = "dd1eeaea8843491dadb258f8642b22ec";
        internal DirectoryReader DirectoryReader;

        public class Thumbnail{
            public string FileName { get; set; }
            public string OrigFile { get; set; }
            public byte ThumbData { get; set; }
        }

        public MainPage() {
            var azureKeyCredential = new AzureKeyCredential(Key1);
            var documentAnalysisClient =
                new DocumentAnalysisClient(new Uri(Endpoint), azureKeyCredential);
            InitializeComponent();


            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            DirectoryReader = new DirectoryReader();
            Label.Text = string.Join('\n', DirectoryReader.Files);
            foreach (var filePath in DirectoryReader.Files){
                ThumbnailStack.Children.Add(new ImageButton {
                    MinimumHeightRequest = 400, Margin = new Thickness(20), Source = filePath
                });
            }
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
                    { DevicePlatform.WinUI, new[] { ".pdf", ".jpg", ".png" } },
                })
        };

        private async void FilePickerButton_OnClicked(object sender, EventArgs e) {
            var pickerRes = await FilePicker.PickMultipleAsync(_pickOptions);
            var addFiles = "";
            foreach (var fileResult in pickerRes){
                Label.Text += fileResult.FullPath + Environment.NewLine;
                addFiles += fileResult.FileName + Environment.NewLine;
                DirectoryReader.DocumentAdd(fileResult.FullPath);
            }

            await DisplayAlert("追加されたファイル", addFiles, "OK");
        }
    }
}