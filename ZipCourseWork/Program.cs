using ZipCourseWork.Implementation.Huffman;
using ZipCourseWork.Implementation.RLE;

var huffmanCompressor = new HuffmanCompressor();
var rleCompressor = new RLEFileCompressor();

Compress("test", "txt");
//Compress("test", "bmp");

void Compress(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}";

    using (var stream = File.OpenRead(filePath))
    {
        rleCompressor.Compress(stream, fileName);
    }

    rleCompressor.Uncompress(fileName, "txt");
}