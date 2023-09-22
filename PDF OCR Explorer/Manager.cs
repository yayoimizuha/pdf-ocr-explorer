using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

#if WINDOWS
using Windows.Data.Pdf;
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
        public string FilePath { get; set; }
        public string FileHash { get; set; }
        public DateTime AddDate { get; set; }


        public PoeFile(string origFile, DateTime addDate) {
            OrigFile = origFile;
            FileHash = StringHasher(origFile);
            FilePath = Path.Combine(ApplicationDataRoot, FileHash, "file" + Path.GetExtension(origFile)!);
            AddDate = addDate;
        }

        public async Task<ImageSource> ThumbImage() {
            ImageSource returnImageSource;

            if (Path.GetExtension(OrigFile)!.ToLower().Equals(".pdf")) {
#if WINDOWS
                var renderOptions = new PdfPageRenderOptions();
                var pdfFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(OrigFile);
                var pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
                using var pdfFirstPage = pdfDocument.GetPage(0);
                renderOptions.DestinationHeight = (uint)pdfFirstPage.Size.Height * 2;
                renderOptions.DestinationWidth = (uint)pdfFirstPage.Size.Width * 2;
                var memStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                pdfFirstPage.RenderToStreamAsync(memStream, renderOptions);
                returnImageSource = ImageSource.FromStream(() => memStream.AsStream());
#else
                throw new NotImplementedException();
#endif
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
            using var reader = new StreamReader(DataJsonPath);
            JsonSerializer.Deserialize<List<PoeFile>>(reader.Read());
        }

        ~FileList() {
            using var writer = new StreamWriter(DataJsonPath);
            writer.Write(JsonSerializer.Serialize(Files));
            writer.Flush();
            writer.Close();
        }


        public void Add(string file) {
            var addFile = new PoeFile(origFile: file, addDate: File.GetCreationTimeUtc(file));
            if (!Directory.Exists(Path.Combine(ApplicationDataRoot, addFile.FileHash))) return;
            Files.Add(addFile);
            Directory.CreateDirectory(Path.Combine(ApplicationDataRoot, addFile.FileHash));
            File.Copy(sourceFileName: file, destFileName: addFile.FilePath);
            using var writer = new StreamWriter(DataJsonPath);
            writer.Write(JsonSerializer.Serialize(Files));
        }

        public void Remove(string file) {
            foreach (var remFile in Files.FindAll(value => value.OrigFile.Equals(file))) {
                Directory.Delete(Path.Combine(ApplicationDataRoot, remFile.FileHash));
            }

            Files.RemoveAll(value => value.OrigFile.Equals(file));
        }
    }
}