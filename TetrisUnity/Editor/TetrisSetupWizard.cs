using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Tetris.Data;

namespace Tetris.Editor
{
    /// <summary>
    /// Menu: Tetris > 1. Setup Project
    /// Creates folders, tile assets, and all ScriptableObjects with pre-filled values.
    /// </summary>
    public static class TetrisSetupWizard
    {
        [MenuItem("Tetris/1. Setup Project")]
        public static void SetupProject()
        {
            CreateFolders();
            CreateTiles();
            CreateGameSettings();
            CreateAllTetrominoData();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Tetris Setup] Done! Folders, Tiles, and ScriptableObjects created.");
        }

        // ── Folders ──────────────────────────────────────────────────────

        private static void CreateFolders()
        {
            EnsureFolder("Assets", "Scripts");
            EnsureFolder("Assets/Scripts", "Core");
            EnsureFolder("Assets/Scripts", "Data");
            EnsureFolder("Assets/Scripts", "UI");
            EnsureFolder("Assets", "ScriptableObjects");
            EnsureFolder("Assets/ScriptableObjects", "Tetrominoes");
            EnsureFolder("Assets/ScriptableObjects", "Settings");
            EnsureFolder("Assets", "Tiles");
            EnsureFolder("Assets", "Input");
            EnsureFolder("Assets", "Scenes");
            EnsureFolder("Assets", "Editor");
        }

        private static void EnsureFolder(string parent, string folder)
        {
            string path = parent + "/" + folder;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, folder);
        }

        // ── Tiles ─────────────────────────────────────────────────────────

        private static readonly (string name, Color color)[] TileData =
        {
            ("Tile_I", new Color(0f,    0.94f, 0.94f)), // cyan
            ("Tile_O", new Color(0.94f, 0.94f, 0f   )), // yellow
            ("Tile_T", new Color(0.63f, 0f,    0.94f)), // purple
            ("Tile_S", new Color(0f,    0.94f, 0f   )), // green
            ("Tile_Z", new Color(0.94f, 0f,    0f   )), // red
            ("Tile_J", new Color(0f,    0f,    0.94f)), // blue
            ("Tile_L", new Color(0.94f, 0.63f, 0f   )), // orange
        };

        private static void CreateTiles()
        {
            foreach (var (tileName, color) in TileData)
            {
                string path = "Assets/Tiles/" + tileName + ".asset";
                if (AssetDatabase.LoadAssetAtPath<Tile>(path) != null) continue;

                var tile = ScriptableObject.CreateInstance<Tile>();
                tile.color = color;
                AssetDatabase.CreateAsset(tile, path);
            }
        }

        // ── GameSettings ─────────────────────────────────────────────────

        private static void CreateGameSettings()
        {
            const string path = "Assets/ScriptableObjects/Settings/DefaultGameSettings.asset";
            if (AssetDatabase.LoadAssetAtPath<GameSettings>(path) != null) return;

            var gs = ScriptableObject.CreateInstance<GameSettings>();
            AssetDatabase.CreateAsset(gs, path);
        }

        // ── TetrominoData ─────────────────────────────────────────────────

        private static void CreateAllTetrominoData()
        {
            CreateTetrominoData(TetrominoType.I, "TD_I", "Tile_I", 20,
                new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) },
                KickOffsets_I());

            CreateTetrominoData(TetrominoType.O, "TD_O", "Tile_O", 18,
                new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
                new Vector2Int[0]);

            CreateTetrominoData(TetrominoType.T, "TD_T", "Tile_T", 18,
                new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1) },
                KickOffsets_JLSTZ());

            CreateTetrominoData(TetrominoType.S, "TD_S", "Tile_S", 18,
                new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(-1,1), new Vector2Int(0,1) },
                KickOffsets_JLSTZ());

            CreateTetrominoData(TetrominoType.Z, "TD_Z", "Tile_Z", 18,
                new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) },
                KickOffsets_JLSTZ());

            CreateTetrominoData(TetrominoType.J, "TD_J", "Tile_J", 18,
                new[] { new Vector2Int(-1,1), new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0) },
                KickOffsets_JLSTZ());

            CreateTetrominoData(TetrominoType.L, "TD_L", "Tile_L", 18,
                new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1) },
                KickOffsets_JLSTZ());
        }

        private static void CreateTetrominoData(
            TetrominoType type, string assetName, string tileName,
            int spawnRow, Vector2Int[] cells, Vector2Int[] kicks)
        {
            string path = "Assets/ScriptableObjects/Tetrominoes/" + assetName + ".asset";
            if (AssetDatabase.LoadAssetAtPath<TetrominoData>(path) != null) return;

            var data = ScriptableObject.CreateInstance<TetrominoData>();

            // Use SerializedObject to set private serialized fields
            var so = new SerializedObject(data);
            so.FindProperty("_type").enumValueIndex   = (int)type;
            so.FindProperty("_spawnRow").intValue      = spawnRow;

            // Cells
            var cellsProp = so.FindProperty("_cells");
            cellsProp.arraySize = cells.Length;
            for (int i = 0; i < cells.Length; i++)
            {
                var elem = cellsProp.GetArrayElementAtIndex(i);
                elem.FindPropertyRelative("x").intValue = cells[i].x;
                elem.FindPropertyRelative("y").intValue = cells[i].y;
            }

            // Kick offsets
            var kicksProp = so.FindProperty("_kickOffsets");
            kicksProp.arraySize = kicks.Length;
            for (int i = 0; i < kicks.Length; i++)
            {
                var elem = kicksProp.GetArrayElementAtIndex(i);
                elem.FindPropertyRelative("x").intValue = kicks[i].x;
                elem.FindPropertyRelative("y").intValue = kicks[i].y;
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(data, path);

            // Wire tile after creation (asset must exist first)
            var tileAsset = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Tiles/" + tileName + ".asset");
            var so2 = new SerializedObject(data);
            so2.FindProperty("_tile").objectReferenceValue = tileAsset;
            so2.ApplyModifiedPropertiesWithoutUndo();
        }

        // ── SRS Kick Tables ───────────────────────────────────────────────
        // Layout: CW transitions (4×5=20) then CCW transitions (4×5=20) = 40 total

        private static Vector2Int[] KickOffsets_JLSTZ()
        {
            return new Vector2Int[]
            {
                // CW 0→1
                new( 0, 0), new(-1, 0), new(-1, 1), new( 0,-2), new(-1,-2),
                // CW 1→2
                new( 0, 0), new( 1, 0), new( 1,-1), new( 0, 2), new( 1, 2),
                // CW 2→3
                new( 0, 0), new( 1, 0), new( 1, 1), new( 0,-2), new( 1,-2),
                // CW 3→0
                new( 0, 0), new(-1, 0), new(-1,-1), new( 0, 2), new(-1, 2),
                // CCW 1→0
                new( 0, 0), new( 1, 0), new( 1, 1), new( 0,-2), new( 1,-2),
                // CCW 2→1
                new( 0, 0), new(-1, 0), new(-1,-1), new( 0, 2), new(-1, 2),
                // CCW 3→2
                new( 0, 0), new(-1, 0), new(-1, 1), new( 0,-2), new(-1,-2),
                // CCW 0→3
                new( 0, 0), new( 1, 0), new( 1,-1), new( 0, 2), new( 1, 2),
            };
        }

        private static Vector2Int[] KickOffsets_I()
        {
            return new Vector2Int[]
            {
                // CW 0→1
                new( 0, 0), new(-2, 0), new( 1, 0), new(-2,-1), new( 1, 2),
                // CW 1→2
                new( 0, 0), new(-1, 0), new( 2, 0), new(-1, 2), new( 2,-1),
                // CW 2→3
                new( 0, 0), new( 2, 0), new(-1, 0), new( 2, 1), new(-1,-2),
                // CW 3→0
                new( 0, 0), new( 1, 0), new(-2, 0), new( 1,-2), new(-2, 1),
                // CCW 1→0
                new( 0, 0), new( 2, 0), new(-1, 0), new( 2, 1), new(-1,-2),
                // CCW 2→1
                new( 0, 0), new( 1, 0), new(-2, 0), new( 1,-2), new(-2, 1),
                // CCW 3→2
                new( 0, 0), new(-2, 0), new( 1, 0), new(-2,-1), new( 1, 2),
                // CCW 0→3
                new( 0, 0), new(-1, 0), new( 2, 0), new(-1, 2), new( 2,-1),
            };
        }
    }
}
