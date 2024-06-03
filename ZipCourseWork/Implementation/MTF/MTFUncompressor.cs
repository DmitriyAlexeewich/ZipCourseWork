using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.MTF
{
    public class MTFUncompressor
    {
        private readonly Range _headerLengthByteLengthRange = new Range(1, 3);
        private readonly Range _letterBytesLengthRange = new Range(3, 5);

        private bool _hasEmptyByte;
        private MTFHeaderLetters _headersLetters;
        private MTFLettersCodes _lettersCodes;
        private List<byte> _result = new List<byte>();

        public MTFUncompressor(byte infoByte)
        {
            var infoBits = infoByte.GetBits();

            _hasEmptyByte = infoBits[0];
            _lettersCodes = new MTFLettersCodes(BitsToLength(infoBits.Take(_letterBytesLengthRange).ToArray()));
            _headersLetters = new MTFHeaderLetters(BitsToLength(infoBits.Take(_headerLengthByteLengthRange).ToArray()));
        }

        public void Add(byte source)
        {
            if (_headersLetters.TryAdd(source))
                return;

            _lettersCodes.Add(source);
        }

        public byte[] Uncompress()
        {
            _lettersCodes.ConfigLast();

            var result = new List<byte>();

            foreach (var code in _lettersCodes.Codes)
            {
                result.AddRange(_headersLetters.Letters[code].Letter);
                _headersLetters.MoveToFront(code);
            }

            if (_hasEmptyByte)
                result.RemoveAt(result.Count - 1);

            return result.ToArray();
        }

        private int BitsToLength(bool[] bits)
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
}
