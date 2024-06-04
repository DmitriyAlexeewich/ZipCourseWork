using System.Text;

namespace ZipCourseWork.Implementation.MTF
{
    public class MTFFileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public ResultFilePath Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---MTF Compress---");

            Console.WriteLine("Compress...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\MTF", $"{fileName}.comp");

            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var compressor = new MTFCompressor();

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
                            compressor.Add(new MTFLetter(fByte.Value, sByte.Value));
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
            Console.WriteLine("---MTF Uncompress---");

            Console.WriteLine("Read files...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\MTF", $"{fileName}_decomp.{extensionName}");
            var sourceFilePath = $"{Path}\\Result\\MTF\\{fileName}.comp";

            if (sourcePath != "")
                sourceFilePath = sourcePath;

            using (var stream = File.OpenRead(sourceFilePath))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var uncompressor = new MTFUncompressor(reader.ReadByte());

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
