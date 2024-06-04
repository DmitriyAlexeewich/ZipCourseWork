using System.Text;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanFileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public ResultFilePath Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Compress---");

            Console.WriteLine("Build tree...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\Huffman", $"{fileName}.comp");

            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var tree = new HuffmanTree();

                    while (true)
                    {
                        var isEnd = false;
                        var bytes = new List<byte>();

                        try
                        {
                            bytes.Add(reader.ReadByte());
                            bytes.Add(reader.ReadByte());
                        }
                        catch (EndOfStreamException e)
                        {
                            if (bytes.Count == 1)
                            {
                                tree.SetEmptyBytesCount();
                                bytes.Add(0);
                            }

                            isEnd = true;
                        }

                        if (!bytes.IsNullOrEmpty())
                        {
                            var letter = BitConverter.ToChar(bytes.ToArray());
                            tree.AddToCompress(letter);
                        }

                        if (isEnd)
                            break;
                    }

                    tree.Build();

                    Console.WriteLine("Compress...");

                    File.WriteAllBytes(resultFilePath.FilePath, tree.Compress());
                }
            }

            Console.WriteLine("Completed!");

            return resultFilePath;
        }

        public ResultFilePath Uncompress(string fileName, string extensionName, string sourcePath = "")
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Uncompress---");

            Console.WriteLine("Uncompress...");

            var resultFilePath = new ResultFilePath($"{Path}\\Result\\Huffman", $"{fileName}_decomp.{extensionName}");
            var sourceFilePath = $"{Path}\\Result\\Huffman\\{fileName}.comp";

            if (sourcePath != "")
                sourceFilePath = sourcePath;

            using (var stream = File.OpenRead(sourceFilePath))
            {
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var huffmanTreeUncompressor = new HuffmanTreeUncompressor(reader.ReadByte());
                    var bytesCount = reader.BaseStream.Length - 1;

                    while (bytesCount > 0)
                    {
                        huffmanTreeUncompressor.Add(reader.ReadByte(), bytesCount == 1);
                        bytesCount--;
                    }

                    File.WriteAllBytes(resultFilePath.FilePath, huffmanTreeUncompressor.Result.ToArray());
                }
            }

            Console.WriteLine("Completed!");

            return resultFilePath;
        }
    }
}
