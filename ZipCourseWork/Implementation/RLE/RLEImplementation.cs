using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.RLE
{
    public class RLEImplementation
    {
        public byte[] Compress(byte[] source)
        {
            if (source.IsNullOrEmpty())
                return Array.Empty<byte>();

            RLEByteInfo currentByte;

            if (source.Length < 2)
                currentByte = new RLESingleBytesInfo(source[0]);
            else
            {
                if (source[0] == source[1])
                    currentByte = new RLESingleBytesInfo(source[0]);
                else
                    currentByte = new RLEMultipleBytesInfo(source[0]);
            }

            var result = new List<byte>();

            for (int i = 1; i < source.Length; i++)
            {
                if (currentByte.TryAdd(source[i]))
                    continue;

                var compressed = currentByte.Compress();

                var t = compressed[0].GetBits();

                result.AddRange(compressed);

                if (source.Length - 1 == i)
                {
                    currentByte = new RLESingleBytesInfo(source[i]);
                    break;
                }

                if (source[i - 1] == source[i])
                {
                    currentByte = new RLESingleBytesInfo(source[i]);
                    continue;
                }

                currentByte = new RLEMultipleBytesInfo(source[i]);
            }

            result.AddRange(currentByte.Compress());

            return result.ToArray();
        }

        public byte[] UnCompress(byte[] source)
        {
            if (source.IsNullOrEmpty())
                return Array.Empty<byte>();

            var compressed = source.ToList();
            var result = new List<byte>();

            while (compressed.Count > 0)
            {
                var infoBits = compressed[0].GetBits();

                var isSingle = infoBits[0];
                var count = CalculateCount(infoBits, isSingle);
                compressed.RemoveAt(0);

                for (var i = 0; i < count; i++)
                {
                    if (isSingle)
                    {
                        result.Add(compressed[0]);
                        continue;
                    }

                    result.Add(compressed[0]);
                    compressed.RemoveAt(0);
                }

                if (isSingle)
                    compressed.RemoveAt(0);
            }

            return result.ToArray();
        }


        private int CalculateCount(bool[] infoBits, bool isSingle)
        {
            var bits = new List<bool>();

            for(var i=1; i<8; i++)
                bits.Add(infoBits[i]);

            bits.Add(false);

            int count = bits.GetByte();

            if (isSingle)
                count++;

            return count;
        }
    }
}
