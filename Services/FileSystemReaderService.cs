using System.IO;

namespace PptxExtractor.Services {
    public interface IFileSystemReaderService {
        string GetStoragePathForSave (string fileName);
        string GetStoragePathTemp (string fileName);
        bool TryGetFile(string fileName, out string path);
    }

    public class FileSystemReaderService : IFileSystemReaderService
    {
        private string GetStoragePath(string folder, string fileName){
            var folderName = Path.Combine("wwwroot", "Resources", folder);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!System.IO.Directory.Exists(pathToSave))
            {
                System.IO.Directory.CreateDirectory(pathToSave);
            }
            var fullPath = Path.Combine(pathToSave, fileName);
            return fullPath;
        }

        public string GetStoragePathForSave(string fileName)
        {
            return GetStoragePath("Files", fileName);
        }

        public string GetStoragePathTemp(string fileName)
        {
            return GetStoragePath("Temp", fileName);
        }

        public bool TryGetFile(string fileName, out string path)
        {
            var folderName = Path.Combine("wwwroot", "Resources", "Files");
            var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            path = Path.Combine(pathToRead, fileName);
            return System.IO.File.Exists(path);
        }
    }
}