using System.Text;
using ZipCourseWork.Implementation.Helpers;
using ZipCourseWork.Implementation.Huffman;
using ZipCourseWork.Implementation.RLE;

var huffmanCompressor = new HuffmanCompressor();
var rleCompressor = new RLECompressor();

Compress("test", "txt");

void Compress(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}";

    using (var sr = new StreamReader(filePath))
    {
        var text = sr.ReadToEnd();

        //huffmanCompressor.Compress($"Huffman{fileName}", text);
        //huffmanCompressor.Uncompress($"Huffman{fileName}", fileExtension);

        rleCompressor.Compress($"RLE{fileName}", text);
        rleCompressor.Uncompress($"RLE{fileName}", fileExtension);
    }
}