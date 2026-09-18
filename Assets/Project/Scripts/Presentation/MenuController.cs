using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    private const string BestScoreKey = "AvisoADolores_BestScore";

    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Paneles")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Opcional")]
    [SerializeField] private TMP_Text bestScoreText;

    private void Start()
    {
        Time.timeScale = 1f;
        ShowMain();

        if (bestScoreText != null)
        {
            int best = PlayerPrefs.GetInt(BestScoreKey, 0);
            bestScoreText.text = best > 0 ? $"Mejor puntaje: {best}" : "";
        }
    }

    public void Play() => SceneManager.LoadScene(gameSceneName);

    public void ShowMain() => Show(mainPanel);
    public void ShowInstructions() => Show(instructionsPanel);
    public void ShowCredits() => Show(creditsPanel);

    private void Show(GameObject panel)
    {
        if (mainPanel != null) mainPanel.SetActive(panel == mainPanel);
        if (instructionsPanel != null) instructionsPanel.SetActive(panel == instructionsPanel);
        if (creditsPanel != null) creditsPanel.SetActive(panel == creditsPanel);
    }
}
