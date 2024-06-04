namespace ZipCourseWork.Implementation
{
    public class ResultFilePath
    {
        public string FolderPath { get; }
        public string FileName { get; }
        public string FilePath { get; }

        public ResultFilePath(string folderPath, string fileName)
        {
            FolderPath = folderPath;
            FileName = fileName;
            FilePath = $"{folderPath}\\{fileName}";
        }
    }
}
