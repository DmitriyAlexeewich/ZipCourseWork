using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using ZipCourseWork.Implementation.Helpers;
using ZipCourseWork.Implementation.RLE;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanFileCompressor
    {
        private HuffmanTree _huffmanTree = new HuffmanTree();
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(FileStream stream, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Compress---");

            Console.WriteLine("Build tree...");

            Console.WriteLine("Compress...");

            using (var reader = new StreamReader(stream, false))
            {
                var tree = new HuffmanTree();

                while (reader.Peek() >= 0)
                    tree.AddToCompress((char)reader.Read());

                tree.Build();

                File.WriteAllBytes($"{Path}\\Result\\Huffman\\{fileName}.comp", tree.Compress());
            }

            Console.WriteLine("Completed!");
        }

        public void Uncompress(string fileName, string extensionName)
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Uncompress---");

            Console.WriteLine("Uncompress...");

            using (var stream = File.OpenRead($"{Path}\\Result\\Huffman\\{fileName}.comp"))
            {
                HuffmanTreeUncompressor huffmanTreeUncompressor = null;

                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    while (true)
                    {
                        try
                        {
                            if (huffmanTreeUncompressor is null)
                                huffmanTreeUncompressor = new HuffmanTreeUncompressor(reader.ReadByte());
                        }
                        catch (EndOfStreamException e)
                        {
                            break;
                        }
                    }

                    //File.WriteAllBytes($"{Path}\\Result\\Huffman\\{fileName}_decomp.{extensionName}", rleCompressor.UnCompress());
                }
            }

            Console.WriteLine("Completed!");
        }
    }
}
