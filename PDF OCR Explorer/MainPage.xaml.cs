using System.Diagnostics;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;

namespace PDF_OCR_Explorer{
    public partial class MainPage : ContentPage{
        private readonly IFilePicker _filePicker;
        int _count = 0;

        internal const string Endpoint = "https://tomokazu-katayama-1.cognitiveservices.azure.com/";
        internal const string Key1 = "90536c6541af426eab1c6e8e4a8cdafe";
        internal const string Key2 = "dd1eeaea8843491dadb258f8642b22ec";
        private Manager.FileList _manager;


        private Grid AddThumb(Manager.PoeFile poeFile) {
            var grid = new Grid {
                HeightRequest = 400,
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(75) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions = {
                    new ColumnDefinition()
                },
                Margin = new Thickness(10)
            };

            grid.Add(
                new Label {
                    Text = Path.GetFileName(poeFile.OrigFile) ?? string.Empty,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 30
                }
                , 0, 0
            );
            var image = new Image { Source = poeFile.ThumbImage() };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, args) =>
            {
                Label.Text += Environment.NewLine + poeFile.OrigFile;
                //FileViewer.Children.Clear();
                //if (Path.GetExtension(poeFile.FilePath)!.ToLower().Equals(".pdf")){
                //FileViewer.Children.Add(new WebView { Source = poeFile.FilePath });
                FileViewer.Source = poeFile.FilePath;
                //}
                //else{
                //    FileViewer.Children.Add(new Image { Source = poeFile.FilePath });
                //}
                foreach (var view in ThumbnailStack.Children){
                    var stackChild = (Grid)view;
                    stackChild.BackgroundColor = Colors.Transparent;
                }

                grid.BackgroundColor = Colors.DarkOliveGreen;
            };
            image.GestureRecognizers.Add(tapGestureRecognizer);
            grid.Add(image, 0, 1);
            return grid;
        }


        public MainPage() {
            var azureKeyCredential = new AzureKeyCredential(Key1);
            var documentAnalysisClient =
                new DocumentAnalysisClient(new Uri(Endpoint), azureKeyCredential);
            InitializeComponent();

            _manager = new Manager.FileList();

            Debug.Print(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            // DirectoryReader = new DirectoryReader();
            //Label.Text = string.Join(Environment.NewLine, DirectoryReader.Files);
            //foreach (var filePath in DirectoryReader.Files){
            foreach (var file in _manager.Files){
                Debug.Print(file.OrigFile);
                ThumbnailStack.Children.Add(AddThumb(file));
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
            foreach (var fileResult in pickerRes){
                Label.Text += Environment.NewLine + fileResult.FullPath;
                addFiles += fileResult.FileName + Environment.NewLine;
                var addFile = _manager.Add(file: fileResult.FullPath);
                ThumbnailStack.Children.Add(AddThumb(addFile));
            }

            if (addFiles.Length != 0){
                await DisplayAlert("追加されたファイル", addFiles, "OK");
            }
        }
    }
}