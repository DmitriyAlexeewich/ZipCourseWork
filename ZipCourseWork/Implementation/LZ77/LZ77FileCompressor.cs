using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using ZipCourseWork.Implementation.Helpers;

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
                        var bytes = new List<byte>();
                        var isEnd = false;

                        try
                        {
                            bytes.Add(reader.ReadByte());
                            bytes.Add(reader.ReadByte());
                        }
                        catch (EndOfStreamException e)
                        {
                            if (bytes.Count == 1)
                            {
                                bytes.Add(0);
                                compressor.SetHasEmptyByte();
                            }

                            isEnd = true;
                        }

                        if (!bytes.IsNullOrEmpty())
                        {
                            var letter = BitConverter.ToChar(bytes.ToArray());
                            compressor.Add(letter);
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


                    File.WriteAllBytes($"{Path}\\Result\\LZ77\\{fileName}_decomp.{extensionName}", uncompressor.Uncompress());
                }
            }

            Console.WriteLine("Completed!");
        }
    }
}
