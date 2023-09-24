using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#if WINDOWS
using Windows.Data.Pdf;
using Microsoft.UI.Xaml.Media.Imaging;
#endif

namespace PDF_OCR_Explorer;

public class Manager {
    public static string ApplicationDataRoot =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "pdf_ocr_explorer");

    public static string DataJsonPath = Path.Combine(ApplicationDataRoot, "data.json");

    private static string StringHasher(string input) {
        var hashBytes = MD5.HashData(Encoding.Default.GetBytes(input));
        return BitConverter.ToString(hashBytes);
    }

    public class PoeFile {
        public string OrigFile { get; set; }
        public string DispName { get; set; } = String.Empty;
        [JsonIgnore] public string FilePath { get; set; }
        [JsonIgnore] public string FileHash { get; set; }
        public DateTime AddDate { get; set; }


        public PoeFile(string origFile, DateTime addDate) {
            OrigFile = origFile;
            FileHash = StringHasher(origFile);
            FilePath = Path.Combine(ApplicationDataRoot, FileHash, "file" + Path.GetExtension(origFile)!);
            AddDate = addDate;
        }

        public ImageSource ThumbImage() {
            ImageSource returnImageSource;

            if (Path.GetExtension(OrigFile)!.ToLower().Equals(".pdf")) {
#if WINDOWS
                //var renderOptions = new PdfPageRenderOptions();
                //var pdfFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(OrigFile);
                //var pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
                //using var pdfFirstPage = pdfDocument.GetPage(0);
                //renderOptions.DestinationHeight = (uint)pdfFirstPage.Size.Height * 2;
                //renderOptions.DestinationWidth = (uint)pdfFirstPage.Size.Width * 2;
                //var memStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                //await pdfFirstPage.RenderToStreamAsync(memStream, renderOptions);
                //Microsoft.UI.Xaml.Media.Imaging.BitmapImage bitmapImage = new() {
                //    DecodePixelWidth = (int)renderOptions.DestinationWidth,
                //    DecodePixelHeight = (int)renderOptions.DestinationHeight
                //};
                //await bitmapImage.SetSourceAsync(memStream);
                ////var renderTargetBitmap = new Microsoft.UI.Xaml.Media.Imaging.RenderTargetBitmap();
                ////await renderTargetBitmap.RenderAsync(bitmapImage);
                //returnImage.Source = bitmapImage.UriSource;

#else
                throw new NotImplementedException();
#endif
                returnImageSource = ImageSource.FromUri(new Uri("https://mizuha-dev.com/files/Adobe_PDF.svg.png"));
            }
            else {
                returnImageSource = ImageSource.FromFile(OrigFile);
            }

            return returnImageSource;
        }
    }

    public class FileList {
        public List<PoeFile> Files { get; set; } = new List<PoeFile>();


        public FileList() {
            if (!File.Exists(DataJsonPath)) return;
            Files = JsonSerializer.Deserialize<List<PoeFile>>(File.ReadAllText(DataJsonPath));
        }

        public void Write() {
            File.WriteAllText(DataJsonPath, JsonSerializer.Serialize(Files));
        }

        ~FileList() {
            Write();
        }


        public PoeFile Add(string file) {
            var addFile = new PoeFile(origFile: file, addDate: File.GetCreationTimeUtc(file));
            addFile.DispName = Path.GetFileName(addFile.OrigFile);
            if (Directory.Exists(Path.Combine(ApplicationDataRoot, addFile.FileHash))) return addFile;
            Files.Add(addFile);
            Directory.CreateDirectory(Path.Combine(ApplicationDataRoot, addFile.FileHash));
            File.Copy(sourceFileName: file, destFileName: addFile.FilePath);
            Write();
            return addFile;
        }

        public void Remove(string file) {
            foreach (var remFile in Files.FindAll(value => value.OrigFile.Equals(file))) {
                Directory.Delete(Path.Combine(ApplicationDataRoot, remFile.FileHash), true);
            }

            Files.RemoveAll(value => value.OrigFile.Equals(file));
            Write();
        }
    }
}