using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tetris.Core;

namespace Tetris.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _linesText;

        [SerializeField] private GameObject _overlayPanel;
        [SerializeField] private TMP_Text   _overlayTitleText;
        [SerializeField] private TMP_Text   _overlayBodyText;

        [SerializeField] private Button _startButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _resumeButton;

        [SerializeField] private GameManager _gameManager;

        private void OnEnable()
        {
            _startButton.onClick.AddListener(_gameManager.OnStartButtonClicked);
            _restartButton.onClick.AddListener(_gameManager.OnRestartButtonClicked);
            _resumeButton.onClick.AddListener(_gameManager.OnResumeButtonClicked);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(_gameManager.OnStartButtonClicked);
            _restartButton.onClick.RemoveListener(_gameManager.OnRestartButtonClicked);
            _resumeButton.onClick.RemoveListener(_gameManager.OnResumeButtonClicked);
        }

        public void UpdateScore(int score) => _scoreText.text = score.ToString();
        public void UpdateLevel(int level) => _levelText.text = level.ToString();
        public void UpdateLines(int lines) => _linesText.text = lines.ToString();

        public void ShowMainMenu()
        {
            _overlayPanel.SetActive(true);
            _overlayTitleText.text = "TETRIS";
            _overlayBodyText.text  = "Press Start to play";
            _startButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(false);
            _resumeButton.gameObject.SetActive(false);
        }

        public void ShowPauseScreen()
        {
            _overlayPanel.SetActive(true);
            _overlayTitleText.text = "PAUSED";
            _overlayBodyText.text  = string.Empty;
            _startButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _resumeButton.gameObject.SetActive(true);
        }

        public void ShowGameOver(int finalScore)
        {
            _overlayPanel.SetActive(true);
            _overlayTitleText.text = "GAME OVER";
            _overlayBodyText.text  = "Score: " + finalScore;
            _startButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _resumeButton.gameObject.SetActive(false);
        }

        public void HideOverlay()
        {
            _overlayPanel.SetActive(false);
        }
    }
}
