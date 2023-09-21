using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

#if WINDOWS
using Windows.Data.Pdf;
#endif

namespace PDF_OCR_Explorer;

public class Manager{
    private static readonly string ApplicationDataRoot =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "pdf_ocr_explorer");

    private static readonly string DataJsonPath = Path.Combine(ApplicationDataRoot, "data.json");

    private static string StringHasher(string input) {
        var hashBytes = SHA256.HashData(Encoding.Default.GetBytes(input));
        return BitConverter.ToString(hashBytes);
    }

    private class PoeFile{
        public string OrigFile { get; set; }
        public string FileHash { get; set; }
        public DateTime AddDate { get; set; }


        public PoeFile(string origFile, DateTime addDate) {
            OrigFile = origFile;
            FileHash = StringHasher(origFile);
            AddDate = addDate;
        }

        public async Task<Image> ThumbImage() {
            Image returnImage;

            if (Path.GetExtension(OrigFile)!.ToLower().Equals(".pdf")){
#if WINDOWS
                var renderOptions = new PdfPageRenderOptions();
                var pdfFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(OrigFile);
                var pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
                using var pdfFirstPage = pdfDocument.GetPage(0);
                renderOptions.DestinationHeight = (uint)pdfFirstPage.Size.Height * 2;
                renderOptions.DestinationWidth = (uint)pdfFirstPage.Size.Width * 2;
                var memStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                pdfFirstPage.RenderToStreamAsync(memStream, renderOptions);
                returnImage = new Image { Source = ImageSource.FromStream(() => memStream.AsStream()) };
#else
                throw new NotImplementedException();
#endif
            }
            else{
                returnImage = new Image { Source = ImageSource.FromFile(OrigFile) };
            }

            return returnImage;
        }
    }

    private class FileList{
        private static readonly List<PoeFile> Files = new();


        public FileList() {
            using var reader = new StreamReader(DataJsonPath);
            JsonSerializer.Deserialize<List<PoeFile>>(reader.Read());
        }

        ~FileList() {
            using var writer = new StreamWriter(DataJsonPath);
            writer.Write(JsonSerializer.Serialize(Files));
            writer.Flush();
            writer.Close();
        }


        private void Add(string file) {
            Files.Add(new PoeFile(origFile: file, addDate: File.GetCreationTimeUtc(file)));
            using var writer = new StreamWriter(DataJsonPath);
            writer.Write(JsonSerializer.Serialize(Files));
        }
    }
}