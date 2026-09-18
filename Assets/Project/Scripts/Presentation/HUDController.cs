using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HUDController : MonoBehaviour
{
    [SerializeField] private GameConfig config;

    [Header("Progreso")]
    [SerializeField] private RectTransform progressFill;

    [Header("Patrulla")]
    [SerializeField] private RectTransform chaseFill;
    [SerializeField] private Image chaseFillImage;
    [SerializeField] private TMP_Text chaseLabel;
    [SerializeField] private Color safeColor = new Color(0.36f, 0.75f, 0.42f);
    [SerializeField] private Color warningColor = new Color(0.95f, 0.78f, 0.25f);
    [SerializeField] private Color dangerColor = new Color(0.9f, 0.27f, 0.22f);

    [Header("Puntuación")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Nombre del tramo")]
    [SerializeField] private TMP_Text sectionText;
    [SerializeField] private CanvasGroup sectionGroup;
    [SerializeField] private float sectionShowTime = 3f;
    [SerializeField] private float sectionFadeTime = 0.5f;

    [Header("Peligro")]
    [SerializeField] private Image dangerOverlay;
    [SerializeField] private float dangerMaxAlpha = 0.18f;
    [SerializeField] private float dangerPulseSpeed = 4f;

    private bool inDanger;
    private Coroutine sectionRoutine;

    private void Awake()
    {
        SetFill(progressFill, 0f);
        SetFill(chaseFill, 1f);
        if (sectionGroup != null) sectionGroup.alpha = 0f;
        SetOverlayAlpha(0f);
    }

    private void OnEnable()
    {
        GameEvents.OnProgressChanged += HandleProgress;
        GameEvents.OnDistanceChanged += HandleDistance;
        GameEvents.OnScoreChanged += HandleScore;
        GameEvents.OnSectionChanged += HandleSection;
        GameEvents.OnGameStateChanged += HandleState;
    }

    private void OnDisable()
    {
        GameEvents.OnProgressChanged -= HandleProgress;
        GameEvents.OnDistanceChanged -= HandleDistance;
        GameEvents.OnScoreChanged -= HandleScore;
        GameEvents.OnSectionChanged -= HandleSection;
        GameEvents.OnGameStateChanged -= HandleState;
    }

    private void Update()
    {
        if (!inDanger)
        {
            SetOverlayAlpha(0f);
            return;
        }
        float pulse = (Mathf.Sin(Time.unscaledTime * dangerPulseSpeed) + 1f) / 2f;
        SetOverlayAlpha(pulse * dangerMaxAlpha);
    }

    // ---------- Observadores

    private void HandleProgress(float value) => SetFill(progressFill, value);

    private void HandleDistance(float distance)
    {
        float t = distance / 100f;
        SetFill(chaseFill, t);

        string label;
        Color color;
        if (distance >= 60f) { label = "Patrulla: lejos"; color = safeColor; }
        else if (distance >= config.dangerThreshold) { label = "Patrulla: cerca"; color = warningColor; }
        else { label = "¡Patrulla encima!"; color = dangerColor; }

        if (chaseFillImage != null) chaseFillImage.color = color;
        if (chaseLabel != null) chaseLabel.text = label;

        inDanger = distance > 0f && distance < config.dangerThreshold;
    }

    private void HandleScore(int score)
    {
        int horseshoes = GameManager.Instance != null ? GameManager.Instance.Horseshoes : 0;
        if (scoreText != null) scoreText.text = $"Herraduras: {horseshoes}\nPuntos: {score}";
    }

    private void HandleSection(int index, string sectionName)
    {
        if (sectionText == null || sectionGroup == null) return;
        string hint = SectionManager.Instance != null ? SectionManager.Instance.Current.hint : "";
        sectionText.text = string.IsNullOrEmpty(hint)
            ? $"Tramo {index + 1}: {sectionName}"
            : $"Tramo {index + 1}: {sectionName}\n<size=60%>{hint}</size>";
        if (sectionRoutine != null) StopCoroutine(sectionRoutine);
        sectionRoutine = StartCoroutine(ShowSection());
    }

    private void HandleState(GameStateId state)
    {
        if (state != GameStateId.Playing) inDanger = false;
    }

    // ---------- Utilidades 

    private IEnumerator ShowSection()
    {
        while (!SectionManager.IsRunning) yield return null;

        sectionGroup.alpha = 1f;
        yield return new WaitForSeconds(sectionShowTime);

        float t = 0f;
        while (t < sectionFadeTime)
        {
            t += Time.deltaTime;
            sectionGroup.alpha = 1f - t / sectionFadeTime;
            yield return null;
        }
        sectionGroup.alpha = 0f;
    }

    private static void SetFill(RectTransform fill, float value)
    {
        if (fill == null) return;
        fill.anchorMin = new Vector2(0f, 0f);
        fill.anchorMax = new Vector2(Mathf.Clamp01(value), 1f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (dangerOverlay == null) return;
        Color c = dangerOverlay.color;
        c.a = alpha;
        dangerOverlay.color = c;
    }
}
