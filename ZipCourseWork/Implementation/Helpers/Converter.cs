using System.Collections;

namespace ZipCourseWork.Implementation.Helpers
{
    public static class Converter
    {
        public static List<byte> GetBytes(this char source)
        {
            var bytes = BitConverter.GetBytes(source);

            var normalizedBytes = new List<byte>() { bytes[0] };

            for (int i = 1; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                    break;

                normalizedBytes.Add(bytes[i]);
            }

            return normalizedBytes;
        }

        public static bool[] GetBits(this int source)
        {
            var bytes = BitConverter.GetBytes(source);
            var result = new List<bool>();

            bytes.ForEach(x => result.AddRange(x.GetBits()));

            return result.ToArray();
        }

        public static bool[] GetBits(this byte source)
        {
            var bits = new BitArray(new byte[] { source });
            var result = new List<bool>();

            foreach (bool bit in bits)
                result.Add(bit);

            return result.ToArray();
        }

        public static byte GetByte(this int source)
        {
            var bytes = BitConverter.GetBytes(source);

            return bytes[0];
        }

        public static byte[] GetBytes(this string source)
        {
            var result = new List<byte>();

            foreach(var letter in source)
                result.AddRange(letter.GetBytes());

            return result.ToArray();
        }

        public static byte GetByte(this IEnumerable<bool> source)
        {
            var sourceInBits = new BitArray(source.ToArray());
            var result = new byte[1];

            sourceInBits.CopyTo(result, 0);

            return result[0];
        }
    }
}
