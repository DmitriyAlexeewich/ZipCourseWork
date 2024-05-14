using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanTreeUncompressor
    {
        private int _emptyBitesCount;
        private int _headerItemsBytesLength;

        public HuffmanTreeUncompressor(byte source)
        {
            ParseFirstByte(source);
        }

        private void ParseFirstByte(byte source)
        {
            var bits = source.GetBits();
            _emptyBitesCount = bits.Take(3).GetByte();
            _headerItemsBytesLength = bits.Take(new Range(3, 4)).GetByte() + 1;
            var t = 0;
        }
    }
}
