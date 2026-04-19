using UnityEngine;
using UnityEngine.InputSystem;
using Tetris.Data;
using Tetris.UI;

namespace Tetris.Core
{
    public enum GameState { MainMenu, Playing, Paused, GameOver }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Board            _board;
        [SerializeField] private HUDController    _hud;
        [SerializeField] private NextPieceDisplay _nextPieceDisplay;
        [SerializeField] private TetrominoData[]  _tetrominoes; // I O T S Z J L
        [SerializeField] private GameSettings     _settings;

        // Input
        private TetrisInputActions _inputActions;
        private bool _moveLeftHeld;
        private bool _moveRightHeld;
        private int  _lastMoveDir;
        private bool _softDropHeld;
        private bool _rotCWPressed;
        private bool _rotCCWPressed;
        private bool _hardDropPressed;
        private bool _pausePressed;

        // State
        private GameState    _state;
        private Piece        _activePiece;
        private SpawnManager _spawnManager;

        private int _score;
        private int _level;
        private int _linesCleared;

        // ── Lifecycle ─────────────────────────────────────────────────────

        private void Awake()
        {
            Debug.Log($"[GM] Awake — board={_board}, settings={_settings}, tetrominoes count={_tetrominoes?.Length}");
            _inputActions = new TetrisInputActions();
            _activePiece  = new Piece(_board, _settings);
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            var gp = _inputActions.Gameplay;

            gp.MoveLeft.performed  += _ => { _moveLeftHeld  = true;  _lastMoveDir = -1; };
            gp.MoveLeft.canceled   += _ => _moveLeftHeld  = false;
            gp.MoveRight.performed += _ => { _moveRightHeld = true;  _lastMoveDir =  1; };
            gp.MoveRight.canceled  += _ => _moveRightHeld = false;
            gp.SoftDrop.performed  += _ => _softDropHeld  = true;
            gp.SoftDrop.canceled   += _ => _softDropHeld  = false;

            gp.RotateCW.performed  += _ => _rotCWPressed    = true;
            gp.RotateCCW.performed += _ => _rotCCWPressed   = true;
            gp.HardDrop.performed  += _ => _hardDropPressed = true;
            gp.Pause.performed     += _ => _pausePressed    = true;
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void Start()
        {
            _state = GameState.MainMenu;
            _hud.ShowMainMenu();
        }

        // ── Update loop ───────────────────────────────────────────────────

        private void Update()
        {
            if (_pausePressed)
            {
                _pausePressed = false;
                HandlePause();
            }

            if (_state != GameState.Playing) return;

            if (_rotCWPressed)  { _rotCWPressed  = false; _activePiece.RotateCW(); }
            if (_rotCCWPressed) { _rotCCWPressed = false; _activePiece.RotateCCW(); }

            if (_hardDropPressed)
            {
                _hardDropPressed = false;
                int dropped = _activePiece.HardDrop();
                AddScore(_settings.HardDropPointsPerRow * dropped);
                LockPiece();
                return;
            }

            int moveDir = 0;
            if      (_moveLeftHeld && _moveRightHeld) moveDir = _lastMoveDir;
            else if (_moveLeftHeld)                   moveDir = -1;
            else if (_moveRightHeld)                  moveDir =  1;

            bool shouldLock = _activePiece.Tick(
                Time.deltaTime, _level,
                moveDir, _softDropHeld);

            if (shouldLock) LockPiece();
        }

        // ── Game flow ─────────────────────────────────────────────────────

        public void StartGame()
        {
            // 필수 참조 null 체크
            if (_board == null)    { Debug.LogError("[GameManager] _board is null. Run Tetris > 2. Build Scene again."); return; }
            if (_settings == null) { Debug.LogError("[GameManager] _settings is null. Run Tetris > 1. Setup Project, then 2. Build Scene."); return; }
            if (_tetrominoes == null || _tetrominoes.Length == 0) { Debug.LogError("[GameManager] _tetrominoes is empty. Run Tetris > 1. Setup Project, then 2. Build Scene."); return; }
            for (int i = 0; i < _tetrominoes.Length; i++)
            {
                if (_tetrominoes[i] == null) { Debug.LogError($"[GameManager] _tetrominoes[{i}] is null. Run Tetris > 1. Setup Project again."); return; }
            }

            Debug.Log("[GM] StartGame — all refs OK, starting...");
            _score        = 0;
            _level        = 1;
            _linesCleared = 0;

            _board.ClearAll();
            _spawnManager = new SpawnManager(_tetrominoes);

            _hud.HideOverlay();
            _hud.UpdateScore(0);
            _hud.UpdateLevel(1);
            _hud.UpdateLines(0);

            _state = GameState.Playing;
            SpawnNextPiece();
        }

        private void SpawnNextPiece()
        {
            TetrominoData data = _spawnManager.Dequeue();
            Debug.Log($"[GM] SpawnNextPiece — type={data.Type}, tile={data.Tile}, spawnRow={data.SpawnRow}, cells={data.Cells?.Length}");
            _nextPieceDisplay.Display(_spawnManager.NextPiece);

            var spawnPos = new Vector2Int(3, data.SpawnRow);

            var cells = data.Cells;
            for (int i = 0; i < cells.Length; i++)
            {
                if (!_board.IsEmpty(spawnPos + cells[i]))
                {
                    Debug.Log($"[GM] GameOver at spawn — cell {spawnPos + cells[i]} is occupied");
                    TriggerGameOver();
                    return;
                }
            }

            Debug.Log($"[GM] Initializing piece at {spawnPos}");
            _activePiece.Initialize(data, spawnPos);
        }

        private void LockPiece()
        {
            int cleared = _board.ClearLines();
            if (cleared > 0)
            {
                _linesCleared += cleared;
                AddScore(_settings.GetLinesClearedPoints(cleared) * _level);
                _level = (_linesCleared / 10) + 1;
                _hud.UpdateLines(_linesCleared);
                _hud.UpdateLevel(_level);
            }

            SpawnNextPiece();
        }

        private void AddScore(int amount)
        {
            _score += amount;
            _hud.UpdateScore(_score);
        }

        private void HandlePause()
        {
            if (_state == GameState.Playing)
            {
                _state = GameState.Paused;
                _hud.ShowPauseScreen();
            }
            else if (_state == GameState.Paused)
            {
                _state = GameState.Playing;
                _hud.HideOverlay();
            }
        }

        private void TriggerGameOver()
        {
            _state = GameState.GameOver;
            _hud.ShowGameOver(_score);
        }

        // Called by HUD buttons
        public void OnStartButtonClicked()   => StartGame();
        public void OnRestartButtonClicked() => StartGame();
        public void OnResumeButtonClicked()  => HandlePause();
    }
}
