using UnityEngine;

namespace Tetris.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Tetris/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Gravity — seconds per row drop per level (index 0 = level 1)")]
        [SerializeField] private float[] _gravitySeconds = new float[]
        {
            1.00f, 0.79f, 0.61f, 0.47f, 0.35f,
            0.26f, 0.19f, 0.14f, 0.10f, 0.08f,
            0.07f, 0.06f, 0.05f, 0.04f, 0.03f,
        };

        [Header("Lock Delay")]
        [SerializeField] private float _lockDelaySeconds = 0.5f;
        [SerializeField] private int   _lockResetMax     = 15;

        [Header("Soft Drop")]
        [SerializeField] private float _softDropSeconds = 0.05f;

        [Header("DAS / ARR")]
        [SerializeField] private float _dasSeconds = 0.133f;
        [SerializeField] private float _arrSeconds = 0.033f;

        [Header("Scoring")]
        [SerializeField] private int[] _linesClearedPoints = new int[] { 0, 100, 300, 500, 800 };
        [SerializeField] private int   _softDropPointsPerRow = 1;
        [SerializeField] private int   _hardDropPointsPerRow = 2;

        public float LockDelaySeconds     => _lockDelaySeconds;
        public int   LockResetMax         => _lockResetMax;
        public float SoftDropSeconds      => _softDropSeconds;
        public float DasSeconds           => _dasSeconds;
        public float ArrSeconds           => _arrSeconds;
        public int   SoftDropPointsPerRow => _softDropPointsPerRow;
        public int   HardDropPointsPerRow => _hardDropPointsPerRow;

        public float GetGravitySeconds(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, _gravitySeconds.Length - 1);
            return _gravitySeconds[idx];
        }

        public int GetLinesClearedPoints(int linesCleared)
        {
            if (linesCleared < 0 || linesCleared >= _linesClearedPoints.Length) return 0;
            return _linesClearedPoints[linesCleared];
        }
    }
}
