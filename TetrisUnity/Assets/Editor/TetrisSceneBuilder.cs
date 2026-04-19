using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;
using Tetris.Core;
using Tetris.Data;
using Tetris.UI;

namespace Tetris.Editor
{
    /// <summary>
    /// Menu: Tetris > 2. Build Scene
    /// Builds the full scene hierarchy, wires all Inspector references automatically.
    /// Run AFTER "1. Setup Project".
    /// </summary>
    public static class TetrisSceneBuilder
    {
        [MenuItem("Tetris/2. Build Scene")]
        public static void BuildScene()
        {
            // Setup이 안 된 경우 자동 실행
            TetrisSetupWizard.SetupProject();

            // 기존 씬 오브젝트 제거 (재실행 시 중복 방지)
            foreach (var name in new[] { "Grid", "Canvas", "GameManager", "EventSystem" })
            {
                var existing = GameObject.Find(name);
                if (existing != null) Object.DestroyImmediate(existing);
            }

            // ── Camera ────────────────────────────────────────────────────
            var cam = Camera.main ?? new GameObject("Main Camera").AddComponent<Camera>();
            cam.gameObject.tag          = "MainCamera";
            cam.orthographic            = true;
            cam.orthographicSize        = 11f;
            cam.transform.position      = new Vector3(0, 0, -10);
            cam.backgroundColor         = new Color(0.1f, 0.1f, 0.18f, 1f);
            cam.clearFlags              = CameraClearFlags.SolidColor;

            // ── Grid + Tilemaps ───────────────────────────────────────────
            var gridGO = new GameObject("Grid");
            var grid = gridGO.AddComponent<Grid>();
            grid.cellSize = Vector3.one;
            gridGO.transform.position = new Vector3(-5f, -10f, 0f);

            // Sorting order: BG(-2) → Border(-1) → Board(0) → Preview(1)
            var bgGO     = CreateTilemapChild(gridGO, "Background", -2);
            var borderGO = CreateTilemapChild(gridGO, "Border",     -1);
            var boardGO  = CreateTilemapChild(gridGO, "Board",       0);
            var previewGO = CreateTilemapChild(gridGO, "Preview",    1);
            previewGO.transform.localPosition = new Vector3(12f, 16f, 0f);

            var bgTilemap     = bgGO.GetComponent<Tilemap>();
            var borderTilemap = borderGO.GetComponent<Tilemap>();
            var boardTilemap  = boardGO.GetComponent<Tilemap>();
            var previewTilemap = previewGO.GetComponent<Tilemap>();

            // Board.cs + BoardBackground.cs
            var boardComp = boardGO.AddComponent<Board>();
            var boardBg   = boardGO.AddComponent<BoardBackground>();

            // ── EventSystem (필수: 없으면 버튼 클릭 불가) ─────────────────
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                esGO.AddComponent<InputSystemUIInputModule>();
            }

            // ── Canvas + UI ───────────────────────────────────────────────
            var canvasGO = new GameObject("Canvas");
            var canvas   = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();

            // HUD panel (우상단 고정)
            var hudGO = CreateUIPanel(canvasGO, "HUD");
            var hudSize = new Vector2(120, 36);
            var scoreValue = CreateTMPLabel(hudGO, "ScoreValue", "0", new Vector2(-60,  -60), hudSize);
            var levelValue = CreateTMPLabel(hudGO, "LevelValue", "1", new Vector2(-60, -100), hudSize);
            var linesValue = CreateTMPLabel(hudGO, "LinesValue", "0", new Vector2(-60, -140), hudSize);
            CreateTMPLabel(hudGO, "ScoreLabel", "SCORE", new Vector2(-180,  -60), hudSize);
            CreateTMPLabel(hudGO, "LevelLabel", "LEVEL", new Vector2(-180, -100), hudSize);
            CreateTMPLabel(hudGO, "LinesLabel", "LINES", new Vector2(-180, -140), hudSize);

            // HUD 라벨들을 우상단 앵커로 설정
            foreach (Transform child in hudGO.transform)
            {
                var rt2 = child.GetComponent<RectTransform>();
                rt2.anchorMin = new Vector2(1, 1);
                rt2.anchorMax = new Vector2(1, 1);
                rt2.pivot     = new Vector2(1, 1);
            }

            // Overlay panel
            var overlayGO = CreateUIPanel(canvasGO, "Overlay");
            var overlayBg = overlayGO.AddComponent<Image>();
            overlayBg.color = new Color(0f, 0f, 0f, 0.75f);
            var overlayTitle = CreateTMPLabel(overlayGO, "OverlayTitle", "TETRIS",      new Vector2(0,  80), new Vector2(400, 80), 64);
            var overlayBody  = CreateTMPLabel(overlayGO, "OverlayBody",  "Press Start", new Vector2(0,  10), new Vector2(400, 40), 28);
            var startBtn     = CreateButton(overlayGO, "StartButton",   "START",   new Vector2(0,  -60));
            var restartBtn   = CreateButton(overlayGO, "RestartButton", "RESTART", new Vector2(0,  -60));
            var resumeBtn    = CreateButton(overlayGO, "ResumeButton",  "RESUME",  new Vector2(0, -120));

            // HUDController
            var hudCtrlGO = new GameObject("HUDController");
            hudCtrlGO.transform.SetParent(canvasGO.transform, false);
            var hudCtrl = hudCtrlGO.AddComponent<HUDController>();

            // ── GameManager ───────────────────────────────────────────────
            var gmGO = new GameObject("GameManager");
            var gm   = gmGO.AddComponent<GameManager>();
            var npd  = gmGO.AddComponent<NextPieceDisplay>();

            // ── Wire all references via SerializedObject ───────────────────

            // Board._tilemap
            var boardSO = new SerializedObject(boardComp);
            boardSO.FindProperty("_tilemap").objectReferenceValue = boardTilemap;
            boardSO.ApplyModifiedPropertiesWithoutUndo();

            // BoardBackground
            var bbSO = new SerializedObject(boardBg);
            bbSO.FindProperty("_bgTilemap").objectReferenceValue     = bgTilemap;
            bbSO.FindProperty("_borderTilemap").objectReferenceValue = borderTilemap;
            bbSO.FindProperty("_bgTile").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Tile>("Assets/Tiles/Tile_BG.asset");
            bbSO.FindProperty("_borderTile").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Tile>("Assets/Tiles/Tile_Border.asset");
            bbSO.ApplyModifiedPropertiesWithoutUndo();

            // NextPieceDisplay._previewTilemap
            var npdSO = new SerializedObject(npd);
            npdSO.FindProperty("_previewTilemap").objectReferenceValue = previewTilemap;
            npdSO.ApplyModifiedPropertiesWithoutUndo();

            // GameManager fields
            var gmSO = new SerializedObject(gm);
            gmSO.FindProperty("_board").objectReferenceValue            = boardComp;
            gmSO.FindProperty("_hud").objectReferenceValue              = hudCtrl;
            gmSO.FindProperty("_nextPieceDisplay").objectReferenceValue = npd;
            gmSO.FindProperty("_settings").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameSettings>(
                    "Assets/ScriptableObjects/Settings/DefaultGameSettings.asset");

            // Wire tetrominoes array [I, O, T, S, Z, J, L]
            string[] tdNames = { "TD_I", "TD_O", "TD_T", "TD_S", "TD_Z", "TD_J", "TD_L" };
            var tetProp = gmSO.FindProperty("_tetrominoes");
            tetProp.arraySize = tdNames.Length;
            for (int i = 0; i < tdNames.Length; i++)
            {
                var td = AssetDatabase.LoadAssetAtPath<TetrominoData>(
                    "Assets/ScriptableObjects/Tetrominoes/" + tdNames[i] + ".asset");
                tetProp.GetArrayElementAtIndex(i).objectReferenceValue = td;
            }
            gmSO.ApplyModifiedPropertiesWithoutUndo();

            // HUDController fields
            var hudSO = new SerializedObject(hudCtrl);
            hudSO.FindProperty("_scoreText").objectReferenceValue       = scoreValue;
            hudSO.FindProperty("_levelText").objectReferenceValue       = levelValue;
            hudSO.FindProperty("_linesText").objectReferenceValue       = linesValue;
            hudSO.FindProperty("_overlayPanel").objectReferenceValue    = overlayGO;
            hudSO.FindProperty("_overlayTitleText").objectReferenceValue = overlayTitle;
            hudSO.FindProperty("_overlayBodyText").objectReferenceValue  = overlayBody;
            hudSO.FindProperty("_startButton").objectReferenceValue     = startBtn;
            hudSO.FindProperty("_restartButton").objectReferenceValue   = restartBtn;
            hudSO.FindProperty("_resumeButton").objectReferenceValue    = resumeBtn;
            hudSO.FindProperty("_gameManager").objectReferenceValue     = gm;
            hudSO.ApplyModifiedPropertiesWithoutUndo();

            // ── Input Actions asset ───────────────────────────────────────
            // Note: TetrisInputActions.cs must be generated from the Input Actions asset
            // (Assets/Input/TetrisInputActions) before this scene builder runs.
            // See README step 1-2.

            EditorUtility.SetDirty(boardComp);
            EditorUtility.SetDirty(boardBg);
            EditorUtility.SetDirty(npd);
            EditorUtility.SetDirty(gm);
            EditorUtility.SetDirty(hudCtrl);

            Debug.Log("[Tetris Scene] Scene built successfully! Press Play to start.");
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static GameObject CreateTilemapChild(GameObject parent, string name, int sortOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.AddComponent<Tilemap>();
            var rend = go.AddComponent<TilemapRenderer>();
            rend.sortingOrder = sortOrder;
            return go;
        }

        private static GameObject CreateUIPanel(GameObject parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return go;
        }

        private static TMP_Text CreateTMPLabel(
            GameObject parent, string name, string text,
            Vector2 anchoredPos, Vector2 size, float fontSize = 28f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta        = size;
            rt.anchoredPosition = anchoredPos;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.color     = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        private static Button CreateButton(
            GameObject parent, string name, string label, Vector2 anchoredPos)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta        = new Vector2(160, 40);
            rt.anchoredPosition = anchoredPos;
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.4f, 1f);
            var btn = go.AddComponent<Button>();

            // Label child
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var lrt = labelGO.AddComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.fontSize  = 22f;
            tmp.color     = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return btn;
        }
    }
}
