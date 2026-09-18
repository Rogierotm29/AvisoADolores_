using TMPro;
using UnityEngine;


public class ScreenController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Textos de resultados")]
    [SerializeField] private TMP_Text victoryStatsText;
    [SerializeField] private TMP_Text gameOverStatsText;

    private void Awake() => ShowOnly(null);

    private void OnEnable() => GameEvents.OnGameStateChanged += HandleState;
    private void OnDisable() => GameEvents.OnGameStateChanged -= HandleState;

    private void HandleState(GameStateId state)
    {
        switch (state)
        {
            case GameStateId.Intro:
                ShowOnly(introPanel);
                SetHud(false);
                break;

            case GameStateId.Playing:
                ShowOnly(null);
                SetHud(true);
                break;

            case GameStateId.Paused:
                ShowOnly(pausePanel);
                SetHud(true);
                break;

            case GameStateId.Victory:
                ShowOnly(victoryPanel);
                SetHud(false);
                if (victoryStatsText != null) victoryStatsText.text = BuildStats(true);
                break;

            case GameStateId.GameOver:
                ShowOnly(gameOverPanel);
                SetHud(false);
                if (gameOverStatsText != null) gameOverStatsText.text = BuildStats(false);
                break;
        }
    }

    private void ShowOnly(GameObject panel)
    {
        SetActive(introPanel, panel == introPanel);
        SetActive(pausePanel, panel == pausePanel);
        SetActive(victoryPanel, panel == victoryPanel);
        SetActive(gameOverPanel, panel == gameOverPanel);
    }

    private void SetHud(bool visible) => SetActive(hudRoot, visible);

    private static void SetActive(GameObject go, bool value)
    {
        if (go != null && go.activeSelf != value) go.SetActive(value);
    }

    private static string BuildStats(bool victory)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return "";

        int minutes = Mathf.FloorToInt(gm.ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(gm.ElapsedTime % 60f);
        string newRecord = gm.Score >= gm.BestScore && gm.Score > 0 ? "  ¡Nuevo récord!" : "";

        string line = victory
            ? $"Tiempo: {minutes}:{seconds:00}"
            : $"Llegaste hasta: {gm.SectionReached}";

        return $"{line}\n" +
               $"Herraduras: {gm.Horseshoes}\n" +
               $"Puntos: {gm.Score}{newRecord}\n" +
               $"Mejor puntaje: {gm.BestScore}";
    }
}
