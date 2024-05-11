using System.Text;
using ZipCourseWork.Implementation.Helpers;
using ZipCourseWork.Implementation.Huffman;
using ZipCourseWork.Implementation.RLE;
using static System.Net.Mime.MediaTypeNames;

var huffmanCompressor = new HuffmanCompressor();
var rleCompressor = new RLECompressor();

Compress("test", "txt");

void Compress(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}";

    using (var stream = File.Open(filePath, FileMode.Open))
    {
        var size = new FileInfo(filePath).Length;

        //huffmanCompressor.Compress($"Huffman{fileName}", text);
        //huffmanCompressor.Uncompress($"Huffman{fileName}", fileExtension);

        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
        {
            var source = reader.ReadBytes(Convert.ToInt32(size));

            rleCompressor.Compress($"RLE{fileName}", source);
            rleCompressor.Uncompress($"RLE{fileName}", fileExtension);
        }
    }
}