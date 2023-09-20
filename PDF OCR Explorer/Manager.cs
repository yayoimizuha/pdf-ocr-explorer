using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PDF_OCR_Explorer;

public class Manager{
    private static readonly string ApplicationDataRoot =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "pdf_ocr_explorer");

    private class FileList{
        private static readonly List<File> Files = new();

        private class File{
            private string _origFile;
            private readonly string _fileHash;
            private DateTime _addDate;

            public File(string origFile) {
                _origFile = origFile;
                _fileHash = StringHasher(Path.GetFullPath(origFile));
                Directory.CreateDirectory(Path.Combine(ApplicationDataRoot, _fileHash));
                System.IO.File.Copy(_origFile,
                    Path.Combine(ApplicationDataRoot, _fileHash, Path.GetFileName(_origFile)));
                _addDate = DateTime.Now;
                Files.Add(this);
            }

            public void Delete() {
                Directory.Delete(Path.Combine(ApplicationDataRoot, _fileHash));
            }
        }

        private static string StringHasher(string input) {
            var inputByte = Encoding.Default.GetBytes(input);
            var hashBytes = MD5.HashData(inputByte);
            return BitConverter.ToString(hashBytes);
        }

        public FileList() {
            using var reader = new StreamReader(Path.Combine(ApplicationDataRoot, "data.json"));
            JsonSerializer.Deserialize<List<File>>(reader.Read());
        }

        ~FileList() {
            using var writer = new StreamWriter(Path.Combine(ApplicationDataRoot, "data.json"));
            writer.Write(JsonSerializer.Serialize(Files));
        }


        private void Add(File file) {
            Files.Add(file);
            using var writer = new StreamWriter(Path.Combine(ApplicationDataRoot, "data.json"));
            writer.Write(JsonSerializer.Serialize(Files));
        }
    }
}