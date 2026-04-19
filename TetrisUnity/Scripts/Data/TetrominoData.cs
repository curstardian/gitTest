using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris.Data
{
    public enum TetrominoType { I, O, T, S, Z, J, L }

    [CreateAssetMenu(fileName = "TetrominoData", menuName = "Tetris/TetrominoData")]
    public class TetrominoData : ScriptableObject
    {
        [SerializeField] private TetrominoType _type;
        [SerializeField] private Tile _tile;
        [SerializeField] private Vector2Int[] _cells;
        [SerializeField] private int _spawnRow = 18;

        // SRS kick offsets: CW rows 0-3 (4×5=20) then CCW rows 0-3 (4×5=20) = 40 entries
        // O-piece: leave empty (size 0)
        [SerializeField] private Vector2Int[] _kickOffsets;

        public TetrominoType Type     => _type;
        public Tile           Tile     => _tile;
        public Vector2Int[]   Cells    => _cells;
        public int            SpawnRow => _spawnRow;

        /// <summary>
        /// Returns 5 kick offsets for the given rotation transition, or null for O-piece.
        /// fromRotation/toRotation: 0=spawn, 1=CW, 2=180, 3=CCW
        /// </summary>
        public Vector2Int[] GetKickOffsets(int fromRotation, int toRotation)
        {
            if (_kickOffsets == null || _kickOffsets.Length == 0)
                return null;

            bool isCW = ((toRotation - fromRotation + 4) % 4) == 1;
            int baseIndex = isCW
                ? fromRotation * 5
                : (4 + fromRotation) * 5;

            var result = new Vector2Int[5];
            for (int i = 0; i < 5; i++)
                result[i] = _kickOffsets[baseIndex + i];
            return result;
        }
    }
}
