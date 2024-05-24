using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public static class HeaderItemasBitsHelper
    {
        public static int ParsePairBits(bool[] bits)
        {
            if (bits[0])
            {
                if (bits[1])
                    return 4;
                else
                    return 3;
            }
            else
            {
                if (bits[1])
                    return 2;
                else
                    return 1;
            }
        }
    }

    public class HeaderItemsCount
    {
        public int Count { get { return _count; } }
        public int CountBytesLength { get { return _countBytesLength; } }
        public bool IsConfigured { get { return _count > 0; } }

        private int _deltaCountBytesLength;
        private int _countBytesLength;
        private List<byte> _countBytes = new List<byte>();
        private int _count = -1;

        public HeaderItemsCount(bool[] countBytesLength)
        {
            _countBytesLength = HeaderItemasBitsHelper.ParsePairBits(countBytesLength);
            _deltaCountBytesLength = _countBytesLength;
        }

        public bool TryAddByte(byte source)
        {
            if (_deltaCountBytesLength > 0)
            {
                _countBytes.Add(source);
                _deltaCountBytesLength--;

                if (_deltaCountBytesLength == 0)
                {
                    while (_countBytes.Count < 4)
                        _countBytes.Add(0);

                    _count = BitConverter.ToInt32(_countBytes.ToArray());
                }

                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItemPath
    {
        public string Path { get { return _pathString; } }

        private List<bool> _lengthBits = new List<bool>();
        private int _length;
        private List<bool> _path = new List<bool>();
        private string _pathString;

        public bool TryAddBit(bool bit)
        {
            if (_lengthBits.Count < 5)
            {
                _lengthBits.Add(bit);

                if (_lengthBits.Count == 5)
                {
                    _lengthBits.Add(false);
                    _lengthBits.Add(false);
                    _lengthBits.Add(false);

                    var bits = new BitArray(_lengthBits.ToArray());

                    var bytes = new byte[4];

                    bits.CopyTo(bytes, 0);
                    _length = BitConverter.ToInt32(bytes);
                }

                return true;
            }

            if (_path.Count < _length)
            {
                _path.Add(bit);

                if(_path.Count == _length)
                    _path.ForEach(x => _pathString += x.ToString());

                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItemLetter
    {
        public bool IsConfigured { get { return !_letter.IsNullOrEmpty(); } }
        public byte[] Letter { get { return _letter; } }

        private List<bool> _letterBits = new List<bool>();
        private byte[] _letter;

        public bool TryAddBit(bool bit)
        {
            if (_letterBits.Count < 16)
            {
                _letterBits.Add(bit);

                if (_letterBits.Count == 16)
                {
                    var bits = new BitArray(_letterBits.ToArray());
                    _letter = new byte[2];

                    bits.CopyTo(_letter, 0);
                }

                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItem
    {
        public bool IsConfigured { get { return _letter != null && _letter.IsConfigured; } }
        public string Path { get { return _path.Path; } }
        public byte[] Letter { get { return _letter.Letter; } }

        private HuffmanHeaderItemPath _path;
        private HuffmanHeaderItemLetter _letter;

        public HuffmanHeaderItem()
        {
            _path = new HuffmanHeaderItemPath();
            _letter = new HuffmanHeaderItemLetter();
        }

        public bool TryAdd(bool bit)
        {
            if (_path.TryAddBit(bit))
                return true;

            if(_letter.TryAddBit(bit))
                return true;

            return false;
        }
    }

    public class HuffmanHeaderItemsList
    {
        public bool IsConfigured { get { return _items.Count == _maxCount && _items[_maxCount - 1].IsConfigured; } }
        public bool HasExtraValue { get { return !_extraValue.IsNullOrEmpty(); } }
        public Dictionary<string, HuffmanHeaderItem> ItemsByPath { get { return _itemsByPath; } }

        private int _maxCount = 0;
        private List<HuffmanHeaderItem> _items = new List<HuffmanHeaderItem>();
        private bool[] _extraValue;
        private Dictionary<string, HuffmanHeaderItem> _itemsByPath = new Dictionary<string, HuffmanHeaderItem>();

        public HuffmanHeaderItemsList(int maxCount)
        {
            _maxCount = maxCount;
            _items.Add(new HuffmanHeaderItem());
            _extraValue = Array.Empty<bool>();
        }

        public bool TryAddByte(byte source)
        {
            if (!_itemsByPath.IsNullOrEmpty())
                return false;

            var bits = source.GetBits();
            var lastItem = _items.Last();
            int i = 0;

            for (i = 0; i < bits.Length; i++)
            {
                if (lastItem.TryAdd(bits[i]))
                    continue;

                if (_items.Count == _maxCount && lastItem.IsConfigured)
                    break;

                lastItem = new HuffmanHeaderItem();
                lastItem.TryAdd(bits[i]);
                _items.Add(lastItem);
            }

            if (_items.Count < _maxCount || !lastItem.IsConfigured)
                return true;

            _itemsByPath = _items.ToDictionary(x => x.Path, x => x);

            if (i == bits.Length)
                return true;

            _extraValue = bits.Take(new Range(i, bits.Length)).ToArray();
            return true;
        }

        public bool[] GetExtraValues()
        {
            var extraValue = _extraValue;
            _extraValue = Array.Empty<bool>();
            return extraValue;
        }
    }
}
