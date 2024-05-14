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
        private HeaderItemsCount _headerItemsCount;
        private HuffmanHeaderItemsList _headerItems;

        public HuffmanTreeUncompressor(byte source)
        {
            ParseFirstByte(source);
        }

        private void ParseFirstByte(byte source)
        {
            var bits = source.GetBits();
            _emptyBitesCount = bits.Take(3).GetByte();
            _headerItemsCount = new HeaderItemsCount(bits.Take(new Range(3, 4)).GetByte() + 1);
        }

        public void Add(byte value)
        {
            if (_headerItemsCount.TryAddByte(value))
            {
                if (_headerItemsCount.IsConfigured)
                    _headerItems = new HuffmanHeaderItemsList(_headerItemsCount.Count);
                return;
            }

            if (_headerItems.TryAddByte(value))
                return;

            var bits = new List<bool>();

            if(_headerItems.HasExtraValue)
                bits.AddRange(_headerItems.GetExtraValues());

            bits.AddRange(value.GetBits());


        }
    }
}
