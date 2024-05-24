using System.Collections;
using ZipCourseWork.Implementation.Huffman;
using ZipCourseWork.Implementation.LZ77;
using ZipCourseWork.Implementation.RLE;

var huffmanCompressor = new HuffmanFileCompressor();
var rleCompressor = new RLEFileCompressor();
var lz77Compressor = new LZ77FileCompressor();

Compress("test", "txt");
//Compress("test", "bmp");

void Compress(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}";

    //huffmanCompressor.Compress(filePath, fileName);
    //rleCompressor.Compress(filePath, fileName);
    lz77Compressor.Compress(filePath, fileName);

    //huffmanCompressor.Uncompress(fileName, fileExtension);
    //rleCompressor.Uncompress(fileName, fileExtension);
    lz77Compressor.Uncompress(fileName, fileExtension);
}