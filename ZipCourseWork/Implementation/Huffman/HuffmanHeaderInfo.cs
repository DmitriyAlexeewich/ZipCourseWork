using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HeaderItemsCount
    {
        public int Count { get { return _count; } }
        public bool IsConfigured { get { return _count < 0; } }

        private int _countBytesLength;
        private List<byte> _countBytes = new List<byte>();
        private int _count = -1;

        public HeaderItemsCount(int countBytesLength)
        {
            _countBytesLength = countBytesLength;
        }

        public bool TryAddByte(byte source)
        {
            if (_countBytesLength > 0)
            {
                _countBytes.Add(source);
                _countBytesLength--;

                if (_countBytesLength == 0)
                    _count = BitConverter.ToInt32(_countBytes.ToArray());

                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItemLength
    {
        public bool IsConfigured { get { return _length != -1; } }
        public int Length { get { return _length; } }

        private List<bool> _bits = new List<bool>();
        private int _length = -1;

        public bool TryAddBit(bool bit)
        {
            if (_bits.Count < 2)
            {
                _bits.Add(bit);

                if(_bits.Count == 2)
                    _length = _bits.Take(2).GetByte() + 1;

                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItemPath
    {
        public List<bool> Path { get { return _path; } }

        private int _lengthBytesCount;
        private List<bool> _lengthBits = new List<bool>();
        private int _length;
        private List<bool> _path = new List<bool>();

        public HuffmanHeaderItemPath(int lengthBytesCount)
        {
            _lengthBytesCount = lengthBytesCount * 8;
        }

        public bool TryAddBit(bool bit)
        {
            if (_lengthBits.Count < _lengthBytesCount)
            {
                _lengthBits.Add(bit);

                if (_lengthBits.Count == _lengthBytesCount)
                {
                    var bits = new BitArray(_lengthBits.ToArray());
                    var bytes = new byte[bits.Count / 8];

                    bits.CopyTo(bytes, 0);
                    _length = BitConverter.ToInt32(bytes);
                }

                return true;
            }

            if (_path.Count < _length)
            {
                _path.Add(bit);
                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItemLetter
    {
        public bool IsConfigured { get { return _letter.HasValue; } }
        public char Letter { get { return _letter.Value; } }

        private int _letterBitsCount;
        private List<bool> _letterBits = new List<bool>();
        private char? _letter;

        public HuffmanHeaderItemLetter(int letterBytesCount)
        {
            _letterBitsCount = letterBytesCount * 8;
        }

        public bool TryAddBit(bool bit)
        {
            if (_letterBits.Count < _letterBitsCount)
            {
                _letterBits.Add(bit);

                if (_letterBits.Count == _letterBitsCount)
                {
                    var bits = new BitArray(_letterBits.ToArray());
                    var bytes = new byte[bits.Count / 8];

                    bits.CopyTo(bytes, 0);
                    _letter = BitConverter.ToChar(bytes);
                }

                return true;
            }

            return false;
        }
    }

    public class HuffmanHeaderItem
    {
        public bool IsConfigured { get { return _letter != null && _letter.IsConfigured; } }
        public List<bool> Path { get { return _path.Path; } }

        private HuffmanHeaderItemLength _letterBytesLength;
        private HuffmanHeaderItemLength _lengthBytesLength;
        private HuffmanHeaderItemPath _path;
        private HuffmanHeaderItemLetter _letter;

        public HuffmanHeaderItem()
        {
            _letterBytesLength = new HuffmanHeaderItemLength();
            _lengthBytesLength = new HuffmanHeaderItemLength();
        }

        public bool TryAdd(bool bit)
        {
            if (_letter is not null || _letter.IsConfigured)
                return false;

            if (_letterBytesLength.TryAddBit(bit))
            {
                if (_letterBytesLength.IsConfigured)
                    _letter = new HuffmanHeaderItemLetter(_letterBytesLength.Length);

                return true;
            }

            if (_lengthBytesLength.TryAddBit(bit))
            {
                if (_lengthBytesLength.IsConfigured)
                    _path = new HuffmanHeaderItemPath(_lengthBytesLength.Length);

                return true;
            }

            if (_path.TryAddBit(bit))
                return true;

            if(_letterBytesLength.TryAddBit(bit))
                return true;

            return false;
        }
    }

    public class HuffmanHeaderItemsList
    {
        public bool IsConfigured { get { return _items.Count == _maxCount && _items[_maxCount - 1].IsConfigured; } }
        public bool HasExtraValue { get { return !_extraValue.IsNullOrEmpty(); } }

        private int _maxCount = 0;
        private List<HuffmanHeaderItem> _items = new List<HuffmanHeaderItem>();
        private bool[] _extraValue;

        public HuffmanHeaderItemsList(int maxCount)
        {
            _maxCount = maxCount;
            _items.Add(new HuffmanHeaderItem());
            _extraValue = Array.Empty<bool>();
        }

        public bool TryAddByte(byte source)
        {
            if (_items.Count == _maxCount)
                return false;

            var bits = source.GetBits();
            var lastItem = _items.Last();
            int i = 0;

            for (i = 0; i < bits.Length; i++)
            {
                if (lastItem.TryAdd(bits[i]))
                    continue;

                if (_items.Count == _maxCount)
                    break;

                lastItem = new HuffmanHeaderItem();
                lastItem.TryAdd(bits[i]);
                _items.Add(lastItem);
            }

            if (_items.Count < _maxCount)
                return true;

            if (i == bits.Length)
                return false;

            _extraValue = bits.Take(new Range(i, bits.Length - 1)).ToArray();
            return false;
        }
    
        public bool[] GetExtraValues() => _extraValue;
    }
}
