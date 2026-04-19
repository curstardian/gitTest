using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
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

            var boardGO = CreateTilemapChild(gridGO, "Board", 0);
            var previewGO = CreateTilemapChild(gridGO, "Preview", 1);
            previewGO.transform.localPosition = new Vector3(12f, 16f, 0f);

            var boardTilemap   = boardGO.GetComponent<Tilemap>();
            var previewTilemap = previewGO.GetComponent<Tilemap>();

            // Board.cs
            var boardComp = boardGO.AddComponent<Board>();

            // ── Canvas + UI ───────────────────────────────────────────────
            var canvasGO = new GameObject("Canvas");
            var canvas   = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // HUD panel
            var hudGO = CreateUIPanel(canvasGO, "HUD");
            var scoreValue = CreateTMPLabel(hudGO, "ScoreValue", "0",   new Vector2(-80, 80));
            var levelValue = CreateTMPLabel(hudGO, "LevelValue", "1",   new Vector2(-80, 40));
            var linesValue = CreateTMPLabel(hudGO, "LinesValue", "0",   new Vector2(-80, 0));
            CreateTMPLabel(hudGO, "ScoreLabel", "SCORE", new Vector2(-160, 80));
            CreateTMPLabel(hudGO, "LevelLabel", "LEVEL", new Vector2(-160, 40));
            CreateTMPLabel(hudGO, "LinesLabel", "LINES", new Vector2(-160, 0));

            // Overlay panel
            var overlayGO = CreateUIPanel(canvasGO, "Overlay");
            var overlayBg = overlayGO.AddComponent<Image>();
            overlayBg.color = new Color(0f, 0f, 0f, 0.75f);
            var overlayTitle = CreateTMPLabel(overlayGO, "OverlayTitle", "TETRIS",           new Vector2(0, 60), 48);
            var overlayBody  = CreateTMPLabel(overlayGO, "OverlayBody",  "Press Start",      new Vector2(0, 10), 24);
            var startBtn     = CreateButton(overlayGO, "StartButton",   "START",   new Vector2(0, -40));
            var restartBtn   = CreateButton(overlayGO, "RestartButton", "RESTART", new Vector2(0, -40));
            var resumeBtn    = CreateButton(overlayGO, "ResumeButton",  "RESUME",  new Vector2(0, -90));

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
            Vector2 anchoredPos, float fontSize = 28f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta      = new Vector2(120, 30);
            rt.anchoredPosition = anchoredPos;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text     = text;
            tmp.fontSize = fontSize;
            tmp.color    = Color.white;
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
