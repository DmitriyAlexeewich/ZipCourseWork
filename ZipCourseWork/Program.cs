using ZipCourseWork.Implementation.Huffman;
using ZipCourseWork.Implementation.LZ77;
using ZipCourseWork.Implementation.MTF;
using ZipCourseWork.Implementation.RLE;

var huffmanCompressor = new HuffmanFileCompressor();
var rleCompressor = new RLEFileCompressor();
var lz77Compressor = new LZ77FileCompressor();
var mtfCompressor = new MTFFileCompressor();

Compress("test", "txt");
//Compress("test", "bmp");

void Compress(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}";

    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("---Single Huffman---");

    huffmanCompressor.Compress(filePath, fileName);
    huffmanCompressor.Uncompress(fileName, fileExtension);

    Console.WriteLine();
    Console.WriteLine("---Single Huffman Complete!---");

    Console.WriteLine();
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("---Single RLE---");

    rleCompressor.Compress(filePath, fileName);
    rleCompressor.Uncompress(fileName, fileExtension);

    Console.WriteLine();
    Console.WriteLine("---Single RLE Complete!---");

    Console.WriteLine();
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("---MTF + Huffman---");

    CompressMTFWithHuffman(sourceFileName, fileExtension);

    Console.WriteLine();
    Console.WriteLine("---MTF + Huffman Complete!---");

    Console.WriteLine();
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("---MTF + RLE + Huffman---");

    CompressMTFWithRLEWithHuffman(sourceFileName, fileExtension);

    Console.WriteLine();
    Console.WriteLine("---MTF + RLE + Huffman Complete!---");

    Console.WriteLine();
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("---Single LZ77---");

    lz77Compressor.Compress(filePath, fileName);
    lz77Compressor.Uncompress(fileName, fileExtension);

    Console.WriteLine();
    Console.WriteLine("---Single LZ77 Complete!---");

    Console.WriteLine();
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("---LZ77 + Huffman---");

    CompressLZ77WithHuffman(sourceFileName, fileExtension);

    Console.WriteLine();
    Console.WriteLine("---LZ77 + Huffman Complete!---");
}

void CompressMTFWithHuffman(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}_MTF_Huffman";

    var mtfFileResult = mtfCompressor.Compress(filePath, fileName);
    huffmanCompressor.Compress(mtfFileResult.FilePath, fileName);

    var huffmanFileResult = huffmanCompressor.Uncompress(fileName, fileExtension);
    mtfCompressor.Uncompress(fileName, fileExtension, huffmanFileResult.FilePath);
}

void CompressMTFWithRLEWithHuffman(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}_MTF_RLE_Huffman";

    var mtfFileResult = mtfCompressor.Compress(filePath, fileName);
    var rleFileResult = rleCompressor.Compress(mtfFileResult.FilePath, fileName);
    huffmanCompressor.Compress(rleFileResult.FilePath, fileName);

    var huffmanFileResult = huffmanCompressor.Uncompress(fileName, fileExtension);
    rleFileResult = rleCompressor.Uncompress(fileName, fileExtension, huffmanFileResult.FilePath);
    mtfCompressor.Uncompress(fileName, fileExtension, rleFileResult.FilePath);
}

void CompressLZ77WithHuffman(string sourceFileName, string fileExtension)
{
    var today = DateTime.Now;
    var filePath = $"{sourceFileName}.{fileExtension}";
    var fileName = $"Result_{today.Year}-{today.Month}-{today.Day} {today.Hour}-{today.Minute}-{today.Second}-{today.Millisecond}_LZ77_Huffman";

    var lz77FileResult = lz77Compressor.Compress(filePath, fileName);
    huffmanCompressor.Compress(lz77FileResult.FilePath, fileName);

    var huffmanFileResult = huffmanCompressor.Uncompress(fileName, fileExtension);
    lz77Compressor.Uncompress(fileName, fileExtension, huffmanFileResult.FilePath);
}
