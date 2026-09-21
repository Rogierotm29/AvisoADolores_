using TMPro;
using UnityEngine;

public class ComboHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;

    [Tooltip("Escala momentanea del texto cada vez que sube la racha")]
    [SerializeField] private float punchScale = 1.4f;

    [Tooltip("Tiempo que tarda en volver a la escala normal")]
    [SerializeField] private float punchRecovery = 0.6f;

    [Header("Color por multiplicador")]
    [SerializeField] private Color colorX2 = new Color(0.91f, 0.64f, 0.24f);
    [SerializeField] private Color colorX3 = new Color(0.96f, 0.45f, 0.20f);
    [SerializeField] private Color colorX4 = new Color(0.90f, 0.22f, 0.22f);

    private int lastMultiplier = 1;

    private void OnEnable() => GameEvents.OnComboChanged += HandleCombo;
    private void OnDisable() => GameEvents.OnComboChanged -= HandleCombo;

    private void Start()
    {
        comboText.gameObject.SetActive(false);
        comboText.rectTransform.localScale = Vector3.one;
    }

    private void Update()
    {
        Transform t = comboText.rectTransform;
        t.localScale = Vector3.Lerp(t.localScale, Vector3.one, punchRecovery * Time.unscaledDeltaTime);
    }

    private void HandleCombo(int combo, int multiplier)
    {
        if (multiplier < 2)
        {
            comboText.gameObject.SetActive(false);
            lastMultiplier = 1;
            return;
        }

        comboText.gameObject.SetActive(true);
        comboText.text = $"COMBO x {multiplier}";
        comboText.color = multiplier >= 4 ? colorX4 : multiplier == 3 ? colorX3 : colorX2;

        if (multiplier > lastMultiplier)
            comboText.rectTransform.localScale = Vector3.one * punchScale;

        lastMultiplier = multiplier;
    }
}
