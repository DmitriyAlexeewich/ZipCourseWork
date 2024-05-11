namespace ZipCourseWork.Implementation.RLE
{
    public class RLECompressor
    {
        private RLEImplementation _rleImplementation = new RLEImplementation();
        private string Path = Directory.GetCurrentDirectory();

        public void Compress(string fileName, byte[] source)
        {
            Console.WriteLine();
            Console.WriteLine("---RLE Compress---");

            Console.WriteLine("Compress...");



            var compressResult = _rleImplementation.Compress(source);

            File.WriteAllBytes($"{Path}\\Result\\RLE\\{fileName}.comp", compressResult);

            Console.WriteLine("Completed!");
        }

        public void Uncompress(string fileName, string extensionName)
        {
            Console.WriteLine();
            Console.WriteLine("---RLE Uncompress---");

            Console.WriteLine("Read files...");

            var bytes = File.ReadAllBytes($"{Path}\\Result\\RLE\\{fileName}.comp");

            var compressResult = _rleImplementation.UnCompress(bytes);

            File.WriteAllBytes($"{Path}\\Result\\RLE\\{fileName}_uncomp.{extensionName}", compressResult);

            Console.WriteLine("Completed!");
        }
    }
}
