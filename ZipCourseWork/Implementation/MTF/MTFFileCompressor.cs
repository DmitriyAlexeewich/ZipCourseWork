using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipCourseWork.Implementation.LZ77;

namespace ZipCourseWork.Implementation.MTF
{
    public class MTFFileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---MTF Compress---");

            Console.WriteLine("Compress...");

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

                    File.WriteAllBytes($"{Path}\\Result\\MTF\\{fileName}.comp", compressor.Compress());
                }
            }

            Console.WriteLine("Completed!");
        }

        public void Uncompress(string fileName, string extensionName)
        {
            Console.WriteLine();
            Console.WriteLine("---MTF Uncompress---");

            Console.WriteLine("Read files...");

            using (var stream = File.OpenRead($"{Path}\\Result\\MTF\\{fileName}.comp"))
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

                    File.WriteAllBytes($"{Path}\\Result\\MTF\\{fileName}_decomp.{extensionName}", uncompressor.Uncompress());
                }
            }

            Console.WriteLine("Completed!");
        }
    }
}
