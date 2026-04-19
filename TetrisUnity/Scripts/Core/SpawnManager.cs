using UnityEngine;
using Tetris.Data;

namespace Tetris.Core
{
    /// <summary>
    /// 7-bag randomizer. Pure C# class — no MonoBehaviour needed.
    /// </summary>
    public class SpawnManager
    {
        private readonly TetrominoData[] _allPieces;
        private readonly int[]           _bag = new int[7];
        private int                      _bagIndex;

        public TetrominoData NextPiece { get; private set; }

        public SpawnManager(TetrominoData[] allPieces)
        {
            _allPieces = allPieces;
            _bagIndex  = 7; // force refill on first draw
            PrimeLookahead();
        }

        public TetrominoData Dequeue()
        {
            TetrominoData current = NextPiece;
            PrimeLookahead();
            return current;
        }

        private void PrimeLookahead()
        {
            NextPiece = _allPieces[DrawFromBag()];
        }

        private int DrawFromBag()
        {
            if (_bagIndex >= 7) ShuffleBag();
            return _bag[_bagIndex++];
        }

        private void ShuffleBag()
        {
            for (int i = 0; i < 7; i++) _bag[i] = i;
            // Fisher-Yates — no LINQ, no allocation
            for (int i = 6; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int tmp = _bag[i];
                _bag[i] = _bag[j];
                _bag[j] = tmp;
            }
            _bagIndex = 0;
        }
    }
}
