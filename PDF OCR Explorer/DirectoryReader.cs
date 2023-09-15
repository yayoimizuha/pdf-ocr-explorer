using System.IO;

namespace PDF_OCR_Explorer{
    public class DirectoryReader{
        internal string SaveDir;

        public DirectoryReader() {
            SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "pdf_ocr_explorer");
            if (Directory.Exists(SaveDir)){
            }
            else{
                Console.WriteLine("Save directory not found:" + SaveDir);
                Directory.CreateDirectory(SaveDir);
            }
        }
    }
}