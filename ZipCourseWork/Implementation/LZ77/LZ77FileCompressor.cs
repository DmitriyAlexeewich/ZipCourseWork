using System.Text;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77FileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---LZ77 Compress---");

            Console.WriteLine("Compress...");

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
                            if (!sByte.HasValue)
                                sByte = 0;

                            isEnd = true;
                        }

                        if (fByte.HasValue && sByte.HasValue)
                        {
                            compressor.Add(fByte.Value, sByte.Value);
                        }

                        if (isEnd)
                            break;
                    }

                    File.WriteAllBytes($"{Path}\\Result\\LZ77\\{fileName}.comp", compressor.Compress());
                }
            }

            Console.WriteLine("Completed!");
        }

        public void Uncompress(string fileName, string extensionName)
        {
            Console.WriteLine();
            Console.WriteLine("---LZ77 Uncompress---");

            Console.WriteLine("Read files...");

            using (var stream = File.OpenRead($"{Path}\\Result\\LZ77\\{fileName}.comp"))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var uncompressor = new LZ77Uncompressor();

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

                    File.WriteAllBytes($"{Path}\\Result\\LZ77\\{fileName}_decomp.{extensionName}", uncompressor.Uncompress());
                }
            }

            Console.WriteLine("Completed!");
        }
    }
}
