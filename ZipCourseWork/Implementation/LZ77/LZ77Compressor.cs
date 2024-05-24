using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77Compressor
    {
        private bool _hasEmptyByte;
        private LZ77GroupInfo[] _groups;
        private List<LZ77CompressedGroupInfo> _result = new List<LZ77CompressedGroupInfo>();

        public void Add(char letter)
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

            _groups.ForEach(x => isAdded = x.TryAdd(letter));

            if (isAdded)
                return;

            _groups.ForEach(x => x.FillRepeats());
            var groupWithRepeats = _groups.LastOrDefault(x => x.HasRepeats);

            if (groupWithRepeats is null)
            {
                var fulltext = _groups.Last().FullText;
                var firstLetterBytes = BitConverter.GetBytes(fulltext[0]);

                firstLetterBytes.ForEach(x => _result.Add(new LZ77CompressedGroupInfo(false, x)));

                fulltext = fulltext.Remove(0, 1);
                AddExtraText(fulltext, letter);

                return;
            }

            _result.AddRange(groupWithRepeats.Compress());

            var extraText = _groups.Last().FullText;

            extraText = extraText.Remove(0, groupWithRepeats.FullText.Length);

            if (groupWithRepeats.HasExtraText)
                extraText += groupWithRepeats.ExtraText;

            AddExtraText(extraText, letter);
        }

        private void AddExtraText(string extraText, char letter)
        {
            _groups =
            [
                new LZ77GroupInfo(2),
                new LZ77GroupInfo(3),
                new LZ77GroupInfo(4),
                new LZ77GroupInfo(5)
            ];

            foreach (var extraTextLetter in extraText)
                _groups.ForEach(x => x.TryAdd(extraTextLetter));

            _groups.ForEach(x => x.TryAdd(letter));
        }

        public void SetHasEmptyByte() => _hasEmptyByte = true;

        public byte[] Compress()
        {
            var result = new List<byte>();

            result.Add(_hasEmptyByte ? (byte)1 : (byte)0);
            var t = 0;

            while (!_result.IsNullOrEmpty())
            {
                t++;
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
