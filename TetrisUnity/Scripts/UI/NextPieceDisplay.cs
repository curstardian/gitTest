using UnityEngine;
using UnityEngine.Tilemaps;
using Tetris.Data;

namespace Tetris.UI
{
    /// <summary>
    /// Renders the next piece preview on a dedicated small Tilemap.
    /// </summary>
    public class NextPieceDisplay : MonoBehaviour
    {
        [SerializeField] private Tilemap _previewTilemap;

        private static readonly Vector2Int PreviewCenter = new Vector2Int(1, 1);

        public void Display(TetrominoData data)
        {
            _previewTilemap.ClearAllTiles();
            if (data == null) return;

            foreach (var cell in data.Cells)
            {
                var pos = new Vector3Int(
                    PreviewCenter.x + cell.x,
                    PreviewCenter.y + cell.y,
                    0);
                _previewTilemap.SetTile(pos, data.Tile);
            }
        }
    }
}
