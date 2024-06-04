using System.Text;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77FileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public ResultFilePath Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---LZ77 Compress---");

            Console.WriteLine("Compress...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\LZ77", $"{fileName}.comp");

            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var compressor = new LZ77Compressor();

                    while (true)
                    {
                        byte? fByte = null;
                        byte? sByte = null;
                        var isEnd = false;

                        try
                        {
                            fByte = reader.ReadByte();
                            sByte = reader.ReadByte();
                        }
                        catch (EndOfStreamException e)
                        {
                            if (fByte.HasValue && !sByte.HasValue)
                            {
                                sByte = 0;
                                compressor.SetHasEmptyByte();
                            }

                            isEnd = true;
                        }

                        if (fByte.HasValue && sByte.HasValue)
                        {
                            compressor.Add(fByte.Value, sByte.Value);
                        }

                        if (isEnd)
                            break;
                    }

                    File.WriteAllBytes(resultFilePath.FilePath, compressor.Compress());
                }
            }

            Console.WriteLine("Completed!");

            return resultFilePath;
        }

        public ResultFilePath Uncompress(string fileName, string extensionName, string sourcePath = "")
        {
            Console.WriteLine();
            Console.WriteLine("---LZ77 Uncompress---");

            Console.WriteLine("Read files...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\LZ77", $"{fileName}_decomp.{extensionName}");
            var sourceFilePath = $"{Path}\\Result\\LZ77\\{fileName}.comp";

            if (sourcePath != "")
                sourceFilePath = sourcePath;

            using (var stream = File.OpenRead(sourceFilePath))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var uncompressor = new LZ77Uncompressor(reader.ReadByte());

                    while (true)
                    {
                        try
                        {
                            uncompressor.Add(reader.ReadByte());
                        }
                        catch (EndOfStreamException e)
                        {
                            break;
                        }
                    }

                    File.WriteAllBytes(resultFilePath.FilePath, uncompressor.Uncompress());
                }
            }

            Console.WriteLine("Completed!");

            return resultFilePath;
        }
    }
}
