using System.Text;

namespace ZipCourseWork.Implementation.RLE
{
    public class RLEFileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(FileStream stream, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---RLE Compress---");

            Console.WriteLine("Compress...");

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

                File.WriteAllBytes($"{Path}\\Result\\RLE\\{fileName}.comp", rleCompressor.Compress());
            }

            Console.WriteLine("Completed!");
        }

        public void Uncompress(string fileName, string extensionName)
        {
            Console.WriteLine();
            Console.WriteLine("---RLE Uncompress---");

            Console.WriteLine("Read files...");

            using (var stream = File.OpenRead($"{Path}\\Result\\RLE\\{fileName}.comp"))
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

                    File.WriteAllBytes($"{Path}\\Result\\RLE\\{fileName}_decomp.{extensionName}", rleCompressor.UnCompress());
                }
            }

            Console.WriteLine("Completed!");
        }
    }
}
