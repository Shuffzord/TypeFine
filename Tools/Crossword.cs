using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tools
{
    public class Crossword
    {
        const string Letters = "abcdefghijklmnopqrstuvwxyząęćśźżółń";
        readonly int[] _dirX = { 0, 1 };
        readonly int[] _dirY = { 1, 0 };
        char[,] _board;
        readonly int[,] _hWords;
        readonly int[,] _vWords;
        readonly int _n;
        readonly int _m;
        int _hCount, _vCount;
        static Random _rand;
        private static IList<string> _wordsToInsert;
        private static char[,] _tempBoard;
        private static int _bestSol;
        DateTime _initialTime;

        public Crossword(int xDimen, int yDimen)
        {
            _board = new char[xDimen, yDimen];
            _hWords = new int[xDimen, yDimen];
            _vWords = new int[xDimen, yDimen];
            _n = xDimen;
            _m = yDimen;
            _rand = new Random();

            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _m; j++)
                {
                    _board[i, j] = ' ';
                }
            }
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _m; j++)
                {
                    result += Letters.Contains(_board[i, j].ToString(CultureInfo.InvariantCulture)) ? _board[i, j] : ' ';
                }
                if (i < _n - 1) result += '\n';
            }
            return result;
        }

        public char this[int i, int j]
        {
            get
            {
                return _board[i, j];
            }
            set
            {
                _board[i, j] = value;
            }
        }

        public int N
        {
            get
            {
                return _n;
            }
        }

        public int M
        {
            get
            {
                return _m;
            }
        }

        public bool InRtl { get; set; }

        bool IsValidPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _n && y < _m;
        }

        int CanBePlaced(string word, int x, int y, int dir)
        {
            var result = 0;
            if (dir == 0)
            {

                for (var j = 0; j < word.Length; j++)
                {
                    int x1 = x, y1 = y + j;
                    if (!(IsValidPosition(x1, y1) && (_board[x1, y1] == ' ' || _board[x1, y1] == word[j])))
                        return -1;
                    if (IsValidPosition(x1 - 1, y1))
                        if (_hWords[x1 - 1, y1] > 0)
                            return -1;
                    if (IsValidPosition(x1 + 1, y1))
                        if (_hWords[x1 + 1, y1] > 0)
                            return -1;
                    if (_board[x1, y1] == word[j])
                        result++;
                }

            }

            else
            {
                for (var j = 0; j < word.Length; j++)
                {
                    int x1 = x + j, y1 = y;
                    if (!(IsValidPosition(x1, y1) && (_board[x1, y1] == ' ' || _board[x1, y1] == word[j])))
                        return -1;
                    if (IsValidPosition(x1, y1 - 1))
                        if (_vWords[x1, y1 - 1] > 0)
                            return -1;
                    if (IsValidPosition(x1, y1 + 1))
                        if (_vWords[x1, y1 + 1] > 0)
                            return -1;
                    if (_board[x1, y1] == word[j])
                        result++;
                }
            }

            int xStar = x - _dirX[dir], yStar = y - _dirY[dir];
            if (IsValidPosition(xStar, yStar))
                if (!(_board[xStar, yStar] == ' ' || _board[xStar, yStar] == '*'))
                    return -1;

            xStar = x + _dirX[dir] * word.Length;
            yStar = y + _dirY[dir] * word.Length;
            if (IsValidPosition(xStar, yStar))
                if (!(_board[xStar, yStar] == ' ' || _board[xStar, yStar] == '*'))
                    return -1;

            return result == word.Length ? -1 : result;

        }

        void PutWord(string word, int x, int y, int dir, int value)
        {
            var mat = dir == 0 ? _hWords : _vWords;

            for (var i = 0; i < word.Length; i++)
            {
                int x1 = x + _dirX[dir] * i, y1 = y + _dirY[dir] * i;
                _board[x1, y1] = word[i];
                mat[x1, y1] = value;
            }

            int xStar = x - _dirX[dir], yStar = y - _dirY[dir];
            if (IsValidPosition(xStar, yStar)) _board[xStar, yStar] = '*';
            xStar = x + _dirX[dir] * word.Length;
            yStar = y + _dirY[dir] * word.Length;
            if (IsValidPosition(xStar, yStar)) _board[xStar, yStar] = '*';
        }

        public int AddWord(string word)
        {

            //var max = int.MaxValue;
            #region ubicate the word into the board
            var wordToInsert = word;
            var info = BestPosition(wordToInsert);
            if (info != null)
            {
                if (info.Item3 == 0)
                {
                    _hCount++;
                    if (InRtl)
                        wordToInsert = word.Aggregate("", (x, y) => y + x);
                }
                else
                    _vCount++;
                var value = info.Item3 == 0 ? _hCount : _vCount;
                PutWord(wordToInsert, info.Item1, info.Item2, info.Item3, value);
                return info.Item3;
            }
            #endregion

            return -1;

        }

        List<Tuple<int, int, int>> FindPositions(string word)
        {
            #region find best position to ubicate the word into the board
            var max = 0;
            var positions = new List<Tuple<int, int, int>>();
            for (var x = 0; x < _n; x++)
            {
                for (var y = 0; y < _m; y++)
                {
                    for (var i = 0; i < _dirX.Length; i++)
                    {
                        var dir = i;
                        var wordToInsert = i == 0 && InRtl ? word.Aggregate("", (a, b) => b + a) : word;
                        var count = CanBePlaced(wordToInsert, x, y, dir);

                        if (count < max) continue;
                        if (count > max)
                            positions.Clear();

                        max = count;
                        positions.Add(new Tuple<int, int, int>(x, y, dir));
                    }
                }
            }
            #endregion

            return positions;
        }

        Tuple<int, int, int> BestPosition(string word)
        {
            var positions = FindPositions(word);
            if (positions.Count > 0)
            {
                var index = _rand.Next(positions.Count);
                return positions[index];
            }
            return null;
        }

        public bool IsLetter(char a)
        {
            return Letters.Contains(a.ToString(CultureInfo.InvariantCulture));
        }

        public char[,] GetBoard
        {
            get
            {
                return _board;
            }
        }

        public void Reset()
        {
            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _m; j++)
                {
                    _board[i, j] = ' ';
                    _vWords[i, j] = 0;
                    _hWords[i, j] = 0;
                    _hCount = _vCount = 0;
                }
            }
        }

        public void AddWords(IList<string> words)
        {
            _wordsToInsert = words;
            _bestSol = N * M;
            _initialTime = DateTime.Now;
            Gen(0);

            _board = _tempBoard;
        }

        int FreeSpaces()
        {
            var count = 0;
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j < M; j++)
                {
                    if (_board[i, j] == ' ' || _board[i, j] == '*')
                        count++;
                }
            }
            return count;
        }

        void Gen(int pos)
        {

            if (pos >= _wordsToInsert.Count || (DateTime.Now - _initialTime).Minutes > 1)
                return;

            for (int i = pos; i < _wordsToInsert.Count; i++)
            {

                var posi = BestPosition(_wordsToInsert[i]);
                if (posi != null)
                {
                    var word = _wordsToInsert[i];
                    if (posi.Item3 == 0 && InRtl)
                        word = word.Aggregate("", (x, y) => y + x);
                    var value = posi.Item3 == 0 ? _hCount : _vCount;
                    PutWord(word, posi.Item1, posi.Item2, posi.Item3, value);
                    Gen(pos + 1);
                    RemoveWord(word, posi.Item1, posi.Item2, posi.Item3);
                }
                else
                {
                    Gen(pos + 1);
                }
            }

            var c = FreeSpaces();
            if (c >= _bestSol) return;
            _bestSol = c;
            _tempBoard = _board.Clone() as char[,];
        }

        private void RemoveWord(string word, int x, int y, int dir)
        {
            var mat = dir == 0 ? _hWords : _vWords;
            var mat1 = dir == 0 ? _vWords : _hWords;

            for (var i = 0; i < word.Length; i++)
            {
                int x1 = x + _dirX[dir] * i, y1 = y + _dirY[dir] * i;
                if (mat1[x1, y1] == 0)
                    _board[x1, y1] = ' ';
                mat[x1, y1] = 0;
            }

            int xStar = x - _dirX[dir], yStar = y - _dirY[dir];
            if (IsValidPosition(xStar, yStar) && HasFactibleValueAround(xStar, yStar))
                _board[xStar, yStar] = ' ';

            xStar = x + _dirX[dir] * word.Length;
            yStar = y + _dirY[dir] * word.Length;
            if (IsValidPosition(xStar, yStar) && HasFactibleValueAround(xStar, yStar))
                _board[xStar, yStar] = ' ';
        }

        bool HasFactibleValueAround(int x, int y)
        {
            for (var i = 0; i < _dirX.Length; i++)
            {
                int x1 = x + _dirX[i], y1 = y + _dirY[i];
                if (IsValidPosition(x1, y1) && (_board[x1, y1] != ' ' || _board[x1, y1] == '*'))
                    return true;
                x1 = x - _dirX[i];
                y1 = y - _dirY[i];
                if (IsValidPosition(x1, y1) && (_board[x1, y1] != ' ' || _board[x1, y1] == '*'))
                    return true;

            }
            return false;
        }


    }

}
