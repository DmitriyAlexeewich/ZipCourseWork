using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanTreeUncompressor
    {
        public List<byte> Result { get { return _result; } }

        private int _emptyBitsCount;
        private HeaderItemsCount _headerItemsCount;
        private bool _hasEmptyByte;
        private HuffmanHeaderItemsList _headerItems;
        private string _code;
        private List<byte> _result = new List<byte>();

        public HuffmanTreeUncompressor(byte source)
        {
            ParseFirstByte(source);
        }

        private void ParseFirstByte(byte source)
        {
            var bits = source.GetBits();
            _emptyBitsCount = ParseEmptyBitsCount(bits.Take(3).ToArray());
            _headerItemsCount = new HeaderItemsCount(bits.Take(new Range(3, 5)).ToArray());
            _hasEmptyByte = bits[5];
        }

        public void Add(byte value, bool isLastByte)
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

            if (isLastByte)
                bits = bits.Take(8 - _emptyBitsCount).ToList();

            foreach (var bit in bits)
            {
                _code += bit.ToString();

                if (!_headerItems.ItemsByPath.ContainsKey(_code))
                    continue;

                var item = _headerItems.ItemsByPath[_code];
                _code = String.Empty;
                _result.AddRange(item.Letter);
            }

            if(isLastByte && _hasEmptyByte)
                _result.RemoveAt(_result.Count - 1);
        }

        private int ParseEmptyBitsCount(bool[] emptyBits)
        {
            if (emptyBits[0])
            {
                if (emptyBits[1])
                {
                    if (emptyBits[2])
                        return 7;
                    else
                        return 6;
                }
                else
                {
                    if (emptyBits[2])
                        return 5;
                    else
                        return 4;
                }
            }
            else
            {
                if (emptyBits[1])
                {
                    if (emptyBits[2])
                        return 3;
                    else
                        return 2;
                }
                else
                {
                    if (emptyBits[2])
                        return 1;
                    else
                        return 0;
                }
            }
        }
        
    }
}
