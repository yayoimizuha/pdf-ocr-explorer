using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;


namespace PDF_OCR_Explorer{
    public partial class MainPage : ContentPage{
        int count = 0;

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
            Label.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "pdf_ocr_explorer");
            DirectoryReader = new DirectoryReader();
        }


        private void ThumbnailView_Loaded(object sender, EventArgs e) {
            ThumbnailView.WidthRequest = Math.Min(400, Window.Width * .2);
        }

        private void Window_SizeChanged(object sender, EventArgs e) {
            ThumbnailView_Loaded(sender, e);
        }


        //        private void OnCounterClicked(object sender, EventArgs e) {
        //            count++;
        //
        //            if (count == 1)
        //                CounterBtn.Text = $"Clicked {count} time";
        //            else
        //                CounterBtn.Text = $"Clicked {count} times";
        //
        //            SemanticScreenReader.Announce(CounterBtn.Text);
        //        }
    }
}