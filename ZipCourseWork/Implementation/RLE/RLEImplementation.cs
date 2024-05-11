using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.RLE
{
    public class RLEImplementation
    {
        public byte[] Compress(string source)
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

                result.AddRange(currentByte.Compress());

                if (source.Length - 1 == i)
                {
                    currentByte = new RLESingleBytesInfo(source[i]);
                    break;
                }

                var t = source[i - 1];
                var tt = source[i];
                var ttt = source[i+1];

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

            var result = new List<byte>();
            var sourceInBits = new BitArray(source);
            var sourceInBools = new List<bool>();

            foreach (bool item in sourceInBits)
                sourceInBools.Add(item);

            while (sourceInBools.Count > 0)
            {
                var isSingle = sourceInBools[0];
                sourceInBools.RemoveAt(0);

                if (isSingle && sourceInBools.Count > 0)
                {
                    result.AddRange(UncompressSingle(sourceInBools));
                    continue;
                }

                if (sourceInBools.Count < 1)
                    break;

                var byteCount = UncompressByteCount(sourceInBools);

                if (sourceInBools.Count < 1)
                    break;

                var singleByte = UncompressSingle(sourceInBools);

                if (singleByte.IsNullOrEmpty())
                    break;

                for (var i = 0; i < byteCount[0]; i++)
                    result.AddRange(singleByte);
            }

            return result.ToArray();
        }

        private byte[] UncompressSingle(List<bool> source)
        {
            var byteInBools = new List<bool>();

            for (var i = 0; i < 8; i++)
            {
                if(source.Count<1)
                    return Array.Empty<byte>();

                byteInBools.Add(source[0]);
                source.RemoveAt(0);
            }

            var byteInBits = new BitArray(byteInBools.ToArray());
            var bytes = new byte[1];

            byteInBits.CopyTo(bytes, 0);

            return bytes;
        }

        private byte[] UncompressByteCount(List<bool> source)
        {
            var byteInBools = new List<bool>();

            byteInBools.Add(false);

            for (var i = 0; i < 7; i++)
            {
                if (source.Count < 1)
                    return Array.Empty<byte>();

                byteInBools.Add(source[0]);
                source.RemoveAt(0);
            }

            var byteInBits = new BitArray(byteInBools.ToArray());
            var bytes = new byte[1];

            byteInBits.CopyTo(bytes, 0);

            return bytes;
        }
    }
}
