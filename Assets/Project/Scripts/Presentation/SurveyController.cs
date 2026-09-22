using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SurveyController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject surveyPanel;

    [Header("Botones por pregunta (5 cada una, en orden 1 a 5)")]
    [SerializeField] private Button[] botonesReto = new Button[5];
    [SerializeField] private Button[] botonesTension = new Button[5];
    [SerializeField] private Button[] botonesJusticia = new Button[5];

    [Header("Enviar")]
    [SerializeField] private Button botonEnviar;

    [Header("Colores de selección")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.91f, 0.64f, 0.24f);

    [Header("Archivo")]
    [SerializeField] private string fileName = "survey_log.csv";

    private const string Header = "sesion,intento,reto,tension,justicia";

    private int reto = -1;
    private int tension = -1;
    private int justicia = -1;

    private void Awake()
    {
        surveyPanel.SetActive(false);

        RegistrarBotones(botonesReto, v => reto = v);
        RegistrarBotones(botonesTension, v => tension = v);
        RegistrarBotones(botonesJusticia, v => justicia = v);

        botonEnviar.onClick.AddListener(Enviar);
    }

    private void OnEnable()
    {
        GameEvents.OnVictory += Mostrar;
        GameEvents.OnGameOver += Mostrar;
    }

    private void OnDisable()
    {
        GameEvents.OnVictory -= Mostrar;
        GameEvents.OnGameOver -= Mostrar;
    }

    private void RegistrarBotones(Button[] grupo, Action<int> asignar)
    {
        for (int i = 0; i < grupo.Length; i++)
        {
            int valor = i + 1;
            Button b = grupo[i];
            b.onClick.AddListener(() =>
            {
                asignar(valor);
                PintarSeleccion(grupo, valor);
            });
        }
    }

    private void PintarSeleccion(Button[] grupo, int valor)
    {
        for (int i = 0; i < grupo.Length; i++)
        {
            var img = grupo[i].GetComponent<Image>();
            if (img != null) img.color = (i + 1 == valor) ? selectedColor : normalColor;
        }
    }

    private void Mostrar()
    {
        Debug.Log("[SurveyController] Mostrando encuesta");
        surveyPanel.SetActive(true);
        surveyPanel.transform.SetAsLastSibling();
        Time.timeScale = 0f;
    }

    private void Enviar()
    {
        if (reto < 0 || tension < 0 || justicia < 0) return;

        GameEvents.RaiseSurveySubmitted(reto, tension, justicia);
        Guardar();
        surveyPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Guardar()
    {
        string row = string.Join(",",
            DynamicsLogger.SessionId,
            DynamicsLogger.Attempt,
            reto, tension, justicia);

        Debug.Log($"[SurveyController] {row}");

#if UNITY_EDITOR || UNITY_STANDALONE
        try
        {
            string folder = Path.Combine(Application.dataPath, "..", "Telemetry");
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, fileName);

            if (!File.Exists(path)) File.WriteAllText(path, Header + Environment.NewLine);
            File.AppendAllText(path, row + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SurveyController] No se pudo escribir el archivo: {e.Message}");
        }
#endif
    }
}