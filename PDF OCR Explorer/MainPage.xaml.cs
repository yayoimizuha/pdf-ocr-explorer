using System.Diagnostics;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using MauiIcons.Fluent;

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
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(50) },
                    new ColumnDefinition { Width = new GridLength(50) }
                },
                Margin = new Thickness(10)
            };

            var fileNameLabel = new Label {
                Text = Path.GetFileName(poeFile.DispName) ?? string.Empty,
                VerticalOptions = LayoutOptions.End,
                FontSize = 20
            };
            grid.SetRow(fileNameLabel, 0);
            grid.SetColumn(fileNameLabel, 0);
            grid.Add(fileNameLabel);
            var image = new Image { Source = poeFile.ThumbImage() };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, args) =>
            {
                LogViewer.Text += Environment.NewLine + "Opened: " + poeFile.OrigFile;
                FileViewer.Source = poeFile.FilePath;
                foreach (var view in ThumbnailStack.Children){
                    var stackChild = (Grid)view;
                    stackChild.BackgroundColor = Colors.Transparent;
                }

                grid.BackgroundColor = Colors.DarkOliveGreen;
            };
            //image.GestureRecognizers.Add(tapGestureRecognizer);
            grid.GestureRecognizers.Add(tapGestureRecognizer);
            var nameEditButton = new ImageButton {
                Source = (ImageSource)new MauiIcon {
                    Icon = FluentIcons.Edit20,
                    IconColor = Colors.WhiteSmoke
                }
            };
            nameEditButton.Clicked += async (s, e) =>
            {
                var changedTitle = await DisplayPromptAsync("ファイル名を入力してください。", "", initialValue: poeFile.DispName) ??
                                   poeFile.DispName;

                fileNameLabel.Text = changedTitle;
                foreach (var file1 in _manager.Files.FindAll(file => file.OrigFile.Equals(poeFile.OrigFile))){
                    file1.DispName = changedTitle;
                }

                poeFile.DispName = changedTitle;
                _manager.Write();
            };
            grid.SetColumn(nameEditButton, 1);
            grid.SetRow(nameEditButton, 0);
            grid.Add(nameEditButton);


            var removeButton = new ImageButton {
                Source = (ImageSource)new MauiIcon {
                    Icon = FluentIcons.Delete20,
                    IconColor = Colors.WhiteSmoke
                }
            };
            removeButton.Clicked += (sender, args) =>
            {
                _manager.Remove(poeFile.OrigFile);
                foreach (var thumbnailStackChild in ThumbnailStack.ToArray()){
                    var stackedGrid = (Grid)thumbnailStackChild;
                    if (!((Label)stackedGrid.Children[2]).Text.Equals(poeFile.FileHash)) continue;
                    ThumbnailStack.Remove(thumbnailStackChild);
                    break;
                }


                //grid.Clear();
                _manager.Write();
            };
            var recLabel = new Label {
                Text = poeFile.FileHash,
                IsVisible = false,
            };
            grid.Add(recLabel, 0, 0);
            grid.SetColumn(removeButton, 2);
            grid.SetRow(removeButton, 0);
            grid.Add(removeButton);
            //grid.Add(image, 0, 1);
            grid.SetRow(image, 1);
            grid.SetColumnSpan(image, 3);
            grid.Add(image);
            return grid;
        }


        public MainPage() {
            var azureKeyCredential = new AzureKeyCredential(Key1);
            var documentAnalysisClient =
                new DocumentAnalysisClient(new Uri(Endpoint), azureKeyCredential);
            InitializeComponent();


            _manager = new Manager.FileList(documentAnalysisClient);

            Debug.Print(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            // DirectoryReader = new DirectoryReader();
            //LogViewer.Text = string.Join(Environment.NewLine, DirectoryReader.Files);
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
            var tasks = new List<Task>();
            foreach (var fileResult in pickerRes){
                LogViewer.Text += Environment.NewLine + "Opened: " + fileResult.FullPath;
                addFiles += fileResult.FileName + Environment.NewLine;
                var addFile = _manager.Add(file: fileResult.FullPath);
                ThumbnailStack.Children.Add(AddThumb(addFile));
                tasks.Add(addFile.Ocr());
            }

            if (addFiles.Length != 0){
                await DisplayAlert("追加されたファイル", addFiles, "OK");
            }

            foreach (var task in tasks){
                await task;
            }
        }

        private void DocumentSearchButtonOnClicked(object sender, EventArgs e) {
            //throw new NotImplementedException();
            var window = new Window(new SearchPage {
                //Window = { Title = "検索ウィンドウ" }
            });
            Application.Current!.OpenWindow(window);
        }
    }
}