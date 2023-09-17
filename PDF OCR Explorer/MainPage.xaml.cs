using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using Microsoft.VisualBasic;


namespace PDF_OCR_Explorer {
    public partial class MainPage : ContentPage {
        private readonly IFilePicker _filePicker;
        int _count = 0;

        internal const string Endpoint = "https://tomokazu-katayama-1.cognitiveservices.azure.com/";
        internal const string Key1 = "90536c6541af426eab1c6e8e4a8cdafe";
        internal const string Key2 = "dd1eeaea8843491dadb258f8642b22ec";
        internal DirectoryReader DirectoryReader;


        public MainPage() {
            var azureKeyCredential = new AzureKeyCredential(Key1);
            var documentAnalysisClient =
                new DocumentAnalysisClient(new Uri(Endpoint), azureKeyCredential);
            InitializeComponent();


            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            DirectoryReader = new DirectoryReader();
            Label.Text = string.Join('_', DirectoryReader.Files);
            for (int i = 0; i < 20; i++) {
                Color color;
                if (i % 2 == 0) {
                    color = Colors.Red;
                }
                else {
                    color = Colors.Aqua;
                }

                ThumbnailStack.Children.Add(new BoxView { MinimumHeightRequest = 400, Color = color });
            }
        }


        private void ThumbnailView_Loaded(object sender, EventArgs e) {
            ThumbnailView.WidthRequest = Math.Min(400, Window.Width * .2);
        }

        private void Window_SizeChanged(object sender, EventArgs e) {
            ThumbnailView_Loaded(sender, e);
        }


        private async void FilePickerButton_OnClicked(object sender, EventArgs e) {
            var pickerRes = await FilePicker.PickMultipleAsync(PickOptions.Default);
            foreach (var fileResult in pickerRes) {
                Label.Text += fileResult.FullPath + Environment.NewLine;
                DirectoryReader.DocumentAdd(fileResult.FullPath);
            }
        }
    }
}