using UnityEngine;
using Tetris.Data;

namespace Tetris.Core
{
    /// <summary>
    /// Active falling piece. Pure C# class — no MonoBehaviour.
    /// All tile writes go through Board.SetTile / Board.ClearTile.
    /// </summary>
    public class Piece
    {
        public TetrominoData Data          { get; private set; }
        public Vector2Int    Position      { get; private set; }
        public int           RotationIndex { get; private set; }

        private readonly Vector2Int[] _cells = new Vector2Int[4];

        // Lock delay
        private float _lockTimer;
        private int   _lockResetCount;
        private bool  _isLocking;

        // Gravity
        private float _gravityTimer;

        // DAS/ARR
        private float _dasTimer;
        private float _arrTimer;
        private int   _dasDirection;

        private readonly Board        _board;
        private readonly GameSettings _settings;

        public Piece(Board board, GameSettings settings)
        {
            _board    = board;
            _settings = settings;
        }

        public void Initialize(TetrominoData data, Vector2Int spawnPosition)
        {
            Data            = data;
            Position        = spawnPosition;
            RotationIndex   = 0;
            _lockTimer      = 0f;
            _lockResetCount = 0;
            _isLocking      = false;
            _gravityTimer   = 0f;
            _dasTimer       = 0f;
            _arrTimer       = 0f;
            _dasDirection   = 0;

            var src = data.Cells;
            for (int i = 0; i < 4; i++) _cells[i] = src[i];

            DrawOnBoard();
        }

        /// <summary>
        /// Called each frame from GameManager.Update().
        /// Returns true when lock delay expires (piece should lock).
        /// </summary>
        public bool Tick(float dt, int level, bool leftHeld, bool rightHeld, bool softDropHeld)
        {
            HandleDAS(dt, leftHeld, rightHeld);
            HandleGravity(dt, level, softDropHeld);

            if (_isLocking)
            {
                _lockTimer += dt;
                if (_lockTimer >= _settings.LockDelaySeconds)
                    return true;
            }
            return false;
        }

        // ── Input actions ────────────────────────────────────────────────

        public bool MoveLeft()  => Move(new Vector2Int(-1, 0));
        public bool MoveRight() => Move(new Vector2Int( 1, 0));
        public bool RotateCW()  => Rotate( 1);
        public bool RotateCCW() => Rotate(-1);

        public int HardDrop()
        {
            int dropped = 0;
            while (Move(new Vector2Int(0, -1))) dropped++;
            return dropped;
        }

        // ── Private helpers ───────────────────────────────────────────────

        private void HandleDAS(float dt, bool leftHeld, bool rightHeld)
        {
            int dir = 0;
            if (leftHeld  && !rightHeld) dir = -1;
            if (rightHeld && !leftHeld)  dir =  1;

            if (dir == 0)
            {
                _dasTimer = 0f;
                _arrTimer = 0f;
                _dasDirection = 0;
                return;
            }

            if (dir != _dasDirection)
            {
                _dasDirection = dir;
                _dasTimer = 0f;
                _arrTimer = 0f;
                return;
            }

            _dasTimer += dt;
            if (_dasTimer >= _settings.DasSeconds)
            {
                _arrTimer += dt;
                while (_arrTimer >= _settings.ArrSeconds)
                {
                    _arrTimer -= _settings.ArrSeconds;
                    Move(new Vector2Int(dir, 0));
                }
            }
        }

        private void HandleGravity(float dt, int level, bool softDropHeld)
        {
            float interval = softDropHeld
                ? _settings.SoftDropSeconds
                : _settings.GetGravitySeconds(level);

            _gravityTimer += dt;
            while (_gravityTimer >= interval)
            {
                _gravityTimer -= interval;
                if (!Move(new Vector2Int(0, -1)))
                {
                    if (!_isLocking)
                    {
                        _isLocking = true;
                        _lockTimer = 0f;
                    }
                }
                else
                {
                    _isLocking = false;
                }
            }
        }

        private bool Move(Vector2Int delta)
        {
            EraseFromBoard();
            Vector2Int newPos = Position + delta;

            if (!IsValidPosition(newPos, _cells))
            {
                DrawOnBoard();
                return false;
            }

            Position = newPos;
            DrawOnBoard();
            return true;
        }

        private bool Rotate(int direction)
        {
            int fromRot = RotationIndex;
            int toRot   = (RotationIndex + direction + 4) % 4;

            Vector2Int[] rotated = RotateCells(_cells, direction);
            var kickOffsets = Data.GetKickOffsets(fromRot, toRot);
            int testCount   = (kickOffsets != null) ? kickOffsets.Length : 1;

            for (int i = 0; i < testCount; i++)
            {
                Vector2Int offset  = (kickOffsets != null) ? kickOffsets[i] : Vector2Int.zero;
                Vector2Int testPos = Position + offset;

                if (IsValidPosition(testPos, rotated))
                {
                    EraseFromBoard();
                    Position      = testPos;
                    RotationIndex = toRot;
                    for (int c = 0; c < 4; c++) _cells[c] = rotated[c];
                    DrawOnBoard();
                    TryResetLockDelay();
                    return true;
                }
            }
            return false;
        }

        private void TryResetLockDelay()
        {
            if (!_isLocking) return;
            if (_lockResetCount >= _settings.LockResetMax) return;
            _lockTimer = 0f;
            _lockResetCount++;
        }

        private bool IsValidPosition(Vector2Int pos, Vector2Int[] cells)
        {
            for (int i = 0; i < 4; i++)
                if (!_board.IsEmpty(pos + cells[i])) return false;
            return true;
        }

        private void DrawOnBoard()
        {
            for (int i = 0; i < 4; i++)
                _board.SetTile(Position + _cells[i], Data.Tile, (int)Data.Type + 1);
        }

        private void EraseFromBoard()
        {
            for (int i = 0; i < 4; i++)
                _board.ClearTile(Position + _cells[i]);
        }

        // CW:  (x,y) → (y, -x)
        // CCW: (x,y) → (-y, x)
        private static Vector2Int[] RotateCells(Vector2Int[] cells, int direction)
        {
            var result = new Vector2Int[4];
            for (int i = 0; i < 4; i++)
            {
                Vector2Int c = cells[i];
                result[i] = (direction == 1)
                    ? new Vector2Int( c.y, -c.x)
                    : new Vector2Int(-c.y,  c.x);
            }
            return result;
        }
    }
}
