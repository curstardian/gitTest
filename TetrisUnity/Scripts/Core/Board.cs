using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris.Core
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;

        public static readonly Vector2Int Size = new Vector2Int(10, 20);
        // Extra 2 buffer rows above the visible board for spawn zone
        private const int BufferRows = 2;

        private int[,] _grid; // 0=empty, typeIndex+1=filled

        private void Awake()
        {
            _grid = new int[Size.x, Size.y + BufferRows];
        }

        // ── Public tile API ──────────────────────────────────────────────

        public void SetTile(Vector2Int pos, Tile tile, int typeIndex)
        {
            if (!IsInBounds(pos)) return;
            _grid[pos.x, pos.y] = typeIndex;
            _tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
        }

        public void ClearTile(Vector2Int pos)
        {
            if (!IsInBounds(pos)) return;
            _grid[pos.x, pos.y] = 0;
            _tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
        }

        public bool IsEmpty(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Size.x) return false;
            if (pos.y < 0 || pos.y >= Size.y + BufferRows) return false;
            return _grid[pos.x, pos.y] == 0;
        }

        public bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < Size.x &&
                   pos.y >= 0 && pos.y < Size.y + BufferRows;
        }

        // ── Line clear ───────────────────────────────────────────────────

        public int ClearLines()
        {
            int cleared = 0;
            for (int y = 0; y < Size.y; y++)
            {
                if (IsLineFull(y))
                {
                    ClearRow(y);
                    DropRowsAbove(y);
                    y--;
                    cleared++;
                }
            }
            return cleared;
        }

        private bool IsLineFull(int y)
        {
            for (int x = 0; x < Size.x; x++)
                if (_grid[x, y] == 0) return false;
            return true;
        }

        private void ClearRow(int y)
        {
            for (int x = 0; x < Size.x; x++)
            {
                _grid[x, y] = 0;
                _tilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }

        private void DropRowsAbove(int clearedY)
        {
            for (int y = clearedY + 1; y < Size.y + BufferRows; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    int typeIndex = _grid[x, y];
                    _grid[x, y - 1] = typeIndex;
                    _grid[x, y] = 0;

                    var tile = _tilemap.GetTile(new Vector3Int(x, y, 0));
                    _tilemap.SetTile(new Vector3Int(x, y - 1, 0), tile);
                    _tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }

        public void ClearAll()
        {
            _tilemap.ClearAllTiles();
            System.Array.Clear(_grid, 0, _grid.Length);
        }
    }
}
