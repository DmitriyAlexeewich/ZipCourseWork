using System;
using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77Compressor
    {
        private bool _hasEmptyByte;
        private LZ77GroupInfo[] _groups;
        private List<LZ77CompressedGroupInfo> _result = new List<LZ77CompressedGroupInfo>();

        public void Add(byte fByte, byte sByte)
        {
            if (_groups == null)
            {
                _groups =
                [
                    new LZ77GroupInfo(2),
                    new LZ77GroupInfo(3),
                    new LZ77GroupInfo(4),
                    new LZ77GroupInfo(5)
                ];
            }

            var isAdded = false;

            _groups.ForEach(x => isAdded = x.TryAdd(fByte, sByte));

            if (isAdded)
                return;

            CompressGroup(fByte, sByte);
        }

        private void CompressGroup(byte fByte, byte sByte, bool isBytesEmpty = false)
        {
            _groups.ForEach(x => x.FillRepeats());
            var groupWithRepeats = _groups.LastOrDefault(x => x.HasRepeats);

            if (groupWithRepeats is null)
            {
                var fulltext = _groups.Last().FullText;
                var firstLetter = fulltext[0];

                firstLetter.Bytes.ForEach(x => _result.Add(new LZ77CompressedGroupInfo(false, x)));

                fulltext.RemoveAt(0);
                AddExtraText(fulltext, fByte, sByte, isBytesEmpty);

                return;
            }

            _result.AddRange(groupWithRepeats.Compress());

            var extraText = groupWithRepeats.ExtraText;
            var fullText = _groups.Last().FullText;

            fullText.RemoveRange(0, groupWithRepeats.FullText.Count);
            extraText = extraText.Union(fullText).ToList();

            AddExtraText(extraText, fByte, sByte, isBytesEmpty);
        }

        private void AddExtraText(List<LZ77Letter> extraText, byte fByte, byte sByte, bool isLetterEmpty)
        {
            _groups =
            [
                new LZ77GroupInfo(2),
                new LZ77GroupInfo(3),
                new LZ77GroupInfo(4),
                new LZ77GroupInfo(5)
            ];

            foreach (var extraTextLetter in extraText)
                _groups.ForEach(x => x.TryAdd(extraTextLetter.FByte, extraTextLetter.SByte));

            if (!isLetterEmpty)
                _groups.ForEach(x => x.TryAdd(fByte, sByte));
        }

        public void SetHasEmptyByte() => _hasEmptyByte = true;

        public byte[] Compress()
        {
            if (_groups.Any(x => x.HasText))
                CompressGroup(0, 0, true);

            if (_groups.Any(x => x.HasText))
            {
                var leftText = _groups.Last().FullText;

                leftText.ForEach(x =>
                {
                    x.Bytes.ForEach(k => _result.Add(new LZ77CompressedGroupInfo(false, k)));
                });
            }

            var result = new List<byte>() { _hasEmptyByte ? (byte)1 : (byte)0 };

            while (!_result.IsNullOrEmpty())
            {
                var infoBits = new bool[8];
                var eightBytes = new List<byte>();

                for (var i = 0; i < 8 && !_result.IsNullOrEmpty(); i++)
                {
                    eightBytes.Add(_result[0].Data);
                    infoBits.SetValue(_result[0].IsCompressed, i);

                    _result.RemoveAt(0);
                }

                var info = new BitArray(infoBits);

                var infoBytes = new byte[1];
                info.CopyTo(infoBytes, 0);

                result.Add(infoBytes[0]);
                result.AddRange(eightBytes);                
            }

            return result.ToArray();
        }
    }
}
