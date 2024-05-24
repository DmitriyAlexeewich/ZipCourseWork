﻿using System.Text;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanFileCompressor
    {
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(string filePath, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Compress---");

            Console.WriteLine("Build tree...");

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

                    File.WriteAllBytes($"{Path}\\Result\\Huffman\\{fileName}.comp", tree.Compress());
                }
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
                using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
                {
                    var huffmanTreeUncompressor = new HuffmanTreeUncompressor(reader.ReadByte());
                    var bytesCount = reader.BaseStream.Length - 1;

                    while (bytesCount > 0)
                    {
                        huffmanTreeUncompressor.Add(reader.ReadByte(), bytesCount == 1);
                        bytesCount--;
                    }

                    File.WriteAllBytes($"{Path}\\Result\\Huffman\\{fileName}_decomp.{extensionName}", huffmanTreeUncompressor.Result.ToArray());
                }
            }

            Console.WriteLine("Completed!");
        }
    }
}
