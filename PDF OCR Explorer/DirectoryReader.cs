﻿using System.IO;

namespace PDF_OCR_Explorer {
    public class DirectoryReader {
        internal readonly string SaveDir;
        internal readonly string DataDir;
        internal List<string> Files;

        public DirectoryReader() {
            SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "pdf_ocr_explorer");
            if (Directory.Exists(SaveDir)) {
            }
            else {
                Console.WriteLine("Save directory not found: " + SaveDir);
                Directory.CreateDirectory(SaveDir);
            }

            DataDir = Path.Combine(SaveDir, "files");
            if (Directory.Exists(DataDir)) {
            }
            else {
                Console.WriteLine("Save directory not found: " + DataDir);
                Directory.CreateDirectory(DataDir);
            }

            Console.WriteLine(Directory.GetDirectories(DataDir));
            Files = new List<string>(Directory.GetDirectories(DataDir));
        }

        internal short DocumentAdd(string filePath) {
            File.Copy(filePath, Files.Count + Path.GetExtension(Path.GetFileName(filePath)));
            Files.Add(filePath);
            return 0;
        }
    }
}