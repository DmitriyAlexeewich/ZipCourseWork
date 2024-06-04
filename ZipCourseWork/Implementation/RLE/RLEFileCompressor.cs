using System.IO;
using System.Text;

namespace ZipCourseWork.Implementation.RLE
{
    public class RLEFileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public ResultFilePath Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---RLE Compress---");

            Console.WriteLine("Compress...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\RLE", $"{fileName}.comp");

            using (var stream = File.OpenRead(filePath))
            {
                var rleCompressor = new RLECompressor();

                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    while (true)
                    {
                        try
                        {
                            rleCompressor.AddToCompress(reader.ReadByte());
                        }
                        catch (EndOfStreamException e)
                        {
                            break;
                        }
                    }

                    File.WriteAllBytes(resultFilePath.FilePath, rleCompressor.Compress());
                }
            }

            Console.WriteLine("Completed!");

            return resultFilePath;
        }

        public ResultFilePath Uncompress(string fileName, string extensionName, string sourcePath = "")
        {
            Console.WriteLine();
            Console.WriteLine("---RLE Uncompress---");

            Console.WriteLine("Read files...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\RLE", $"{fileName}_decomp.{extensionName}");
            var sourceFilePath = $"{Path}\\Result\\RLE\\{fileName}.comp";

            if (sourcePath != "")
                sourceFilePath = sourcePath;

            using (var stream = File.OpenRead(sourceFilePath))
            {
                var rleCompressor = new RLECompressor();

                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    while (true)
                    {
                        try
                        {
                            rleCompressor.AddToUncompress(reader.ReadByte());
                        }
                        catch (EndOfStreamException e)
                        {
                            break;
                        }
                    }

                    File.WriteAllBytes(resultFilePath.FilePath, rleCompressor.UnCompress());
                }
            }

            Console.WriteLine("Completed!");

            return resultFilePath;
        }
    }
}
