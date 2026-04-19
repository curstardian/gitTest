# CLAUDE.md — Unity Tetris

## Tech Stack
Unity 6 (6000.x), C#, Input System (not legacy Input), Tilemap

## Unity Project Root
- Unity project root: `TetrisUnity/` (inside this repo)
- **All Unity files must go inside `TetrisUnity/Assets/`**
- Before creating any file, confirm `TetrisUnity/Assets/` exists
- Never place Unity scripts at the repo root (`gitTest/Assets/`)

## Project Structure
```
TetrisUnity/Assets/
Scripts/
├── Core/
│   ├── Board.cs         # Grid state, collision, line clear
│   ├── Piece.cs         # Movement, SRS rotation, lock delay
│   ├── GameManager.cs   # State machine (Playing/Paused/GameOver)
│   └── SpawnManager.cs  # 7-bag randomizer
├── Data/
│   ├── TetrominoData.cs # ScriptableObject: shape, kick tables
│   └── GameSettings.cs  # ScriptableObject: speed curve, DAS/ARR
└── UI/
    ├── HUDController.cs
    └── NextPieceDisplay.cs
```

## Editor Scene Builder Rules (`TetrisSceneBuilder.cs`)

씬을 코드로 생성할 때 반드시 지켜야 할 규칙:

**EventSystem**
- Canvas가 있는 씬에는 반드시 `EventSystem` + `StandaloneInputModule`을 생성해야 함
- 없으면 버튼 클릭이 전혀 동작하지 않음
- `Object.FindFirstObjectByType<EventSystem>() == null` 체크 후 생성
- **`StandaloneInputModule` 사용 금지** — 새 Input System과 충돌. 반드시 `InputSystemUIInputModule` 사용 (`using UnityEngine.InputSystem.UI`)

**Button 위치**
- 같은 Overlay 안 버튼들은 `anchoredPosition`이 겹치지 않도록 각각 다른 y값 사용
- 예: StartButton `(0,-60)`, RestartButton `(0,-60)` (ShowMainMenu에서 하나만 활성화), ResumeButton `(0,-120)`

**TMP_Text sizeDelta**
- `TextMeshProUGUI`는 `sizeDelta`가 텍스트보다 작으면 글자가 잘리거나 안 보임
- 오버레이 타이틀(64pt): 최소 `(400, 80)`
- 오버레이 본문(28pt): 최소 `(400, 40)`
- HUD 라벨(28pt): 최소 `(120, 36)`

**CanvasScaler**
- `ScaleMode = ScaleWithScreenSize`, `referenceResolution = (1920, 1080)` 설정
- 기본값(ConstantPixelSize)은 해상도에 따라 UI 크기가 달라짐

**재실행 안전성**
- `BuildScene()` 시작 시 기존 씬 오브젝트(`Grid`, `Canvas`, `GameManager`, `EventSystem`)를 `DestroyImmediate`로 제거 후 재생성

## Core Rules

**Grid**
- Size: 10×20, origin bottom-left (0,0)
- Rendering via `UnityEngine.Tilemaps.Tilemap`

**Piece Logic**
- Rotation: SRS with kick tables stored in `TetrominoData`
- Gravity: accumulated `float` timer in `Update()` — no `InvokeRepeating`
- Lock delay: 500ms, resets on move/rotate (max 15 resets)

**Do NOT**
- Modify `Board.grid` directly from `Piece.cs` — use `Board.SetTile()` / `Board.ClearTile()`
- Use `Find()` or `FindObjectOfType()` at runtime
- Use LINQ or heap allocations inside `Update()`

## Coding Conventions
- Namespace: `Tetris.Core`, `Tetris.UI`, `Tetris.Data`
- `[SerializeField] private` instead of `public` fields
- Subscribe events in `OnEnable`, unsubscribe in `OnDisable`
- Game logic in plain C# classes; MonoBehaviour only for Unity lifecycle

## References
- [Tetris Guideline](https://tetris.wiki/Tetris_Guideline)
- [SRS Kick Tables](https://tetris.wiki/SRS)
