using System.Collections;

namespace ZipCourseWork.Implementation.Helpers
{
    public static class Converter
    {
        public static byte[] RemoveExtraZero(this byte[] source)
        {
            var result = new List<byte>() { source[0] };

            for (int i = 1; i < source.Length; i++)
            {
                if (source[i] == 0)
                    break;

                result.Add(source[i]);
            }

            return result.ToArray();
        }

        public static bool[] GetBits(this int source, bool compress = false)
        {
            var bytes = BitConverter.GetBytes(source);
            var result = new List<bool>();

            result.AddRange(bytes[0].GetBits());

            if (!compress)
                return result.ToArray();

            for (int i = 1; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                    break;

                result.AddRange(bytes[i].GetBits());
            }

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

        public static bool[] GetBits(this byte[] source)
        {
            var bits = new BitArray(source);
            var result = new List<bool>();

            foreach (bool bit in bits)
                result.Add(bit);

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
