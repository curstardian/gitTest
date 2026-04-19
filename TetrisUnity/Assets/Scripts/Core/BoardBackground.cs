using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris.Core
{
    /// <summary>
    /// 플레이 영역 배경(어두운 격자)과 테두리(밝은 경계선)를 그립니다.
    /// Board와 같은 GameObject에 붙이거나 별도 GO에 붙여도 됩니다.
    /// </summary>
    public class BoardBackground : MonoBehaviour
    {
        [SerializeField] private Tilemap _bgTilemap;     // sorting order: -2
        [SerializeField] private Tilemap _borderTilemap; // sorting order: -1
        [SerializeField] private Tile    _bgTile;        // 어두운 배경 타일
        [SerializeField] private Tile    _borderTile;    // 밝은 경계선 타일

        private void Start()
        {
            DrawBackground();
            DrawBorder();
        }

        private void DrawBackground()
        {
            if (_bgTilemap == null || _bgTile == null) return;
            for (int x = 0; x < Board.Size.x; x++)
                for (int y = 0; y < Board.Size.y; y++)
                    _bgTilemap.SetTile(new Vector3Int(x, y, 0), _bgTile);
        }

        private void DrawBorder()
        {
            if (_borderTilemap == null || _borderTile == null) return;

            int w = Board.Size.x;
            int h = Board.Size.y;

            // 좌우 벽
            for (int y = -1; y <= h; y++)
            {
                _borderTilemap.SetTile(new Vector3Int(-1, y, 0), _borderTile);
                _borderTilemap.SetTile(new Vector3Int(w,  y, 0), _borderTile);
            }
            // 바닥
            for (int x = 0; x < w; x++)
                _borderTilemap.SetTile(new Vector3Int(x, -1, 0), _borderTile);
        }
    }
}
