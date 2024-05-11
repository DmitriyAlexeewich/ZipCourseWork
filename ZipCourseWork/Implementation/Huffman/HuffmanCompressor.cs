using System.Text.Json;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanCompressor
    {
        private HuffmanTree _huffmanTree = new HuffmanTree();
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(string fileName, string source)
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Compress---");

            Console.WriteLine("Build tree...");

            _huffmanTree.Build(source);

            Console.WriteLine("Compress...");

            var compressResult = _huffmanTree.Compress(source);

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            var headerJson = JsonSerializer.Serialize(compressResult.Header, options);

            File.WriteAllText($"{Path}\\Result\\Huffman\\{fileName}.info", headerJson);
            File.WriteAllBytes($"{Path}\\Result\\Huffman\\{fileName}.comp", compressResult.Compressed);

            Console.WriteLine("Completed!");
        }

        public void Uncompress(string fileName, string extensionName)
        {
            Console.WriteLine();
            Console.WriteLine("---Huffman Uncompress---");

            Console.WriteLine("Read files...");

            var headerText = File.ReadAllText($"{Path}\\Result\\Huffman\\{fileName}.info");
            var header = JsonSerializer.Deserialize<CompressedFileHeader>(headerText);

            var bytes = File.ReadAllBytes($"{Path}\\Result\\Huffman\\{fileName}.comp");

            Console.WriteLine("Uncompress...");

            var result = _huffmanTree.Uncompress(bytes, header);

            File.WriteAllBytes($"{Path}\\Result\\Huffman\\{fileName}_uncomp.{extensionName}", result);

            Console.WriteLine("Completed!");
        }
    }
}
