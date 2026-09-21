using System;
using System.Globalization;
using System.IO;
using UnityEngine;


public class DynamicsLogger : MonoBehaviour
{
    [Tooltip("Nombre del archivo que se genera en la carpeta Telemetry, en la raiz del proyecto")]
    [SerializeField] private string fileName = "dynamics_log.csv";

    private const string Header =
        "sesion,intento,fecha_hora,resultado,tramo,duracion_s,saltos,agachadas," +
        "herraduras,relevos,choques,combo_max,puntaje";

    private static string sessionId;
    private static int attempt;

    private int jumps, ducks, horseshoes, relays, hits, maxCombo, score;
    private string section = "";
    private float duration;
    private bool written;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        sessionId = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        attempt = 0;
    }

    private void OnEnable()
    {
        GameEvents.OnJump += HandleJump;
        GameEvents.OnDuck += HandleDuck;
        GameEvents.OnCollect += HandleCollect;
        GameEvents.OnHit += HandleHit;
        GameEvents.OnComboChanged += HandleCombo;
        GameEvents.OnScoreChanged += HandleScore;
        GameEvents.OnSectionChanged += HandleSection;
        GameEvents.OnVictory += HandleVictory;
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnJump -= HandleJump;
        GameEvents.OnDuck -= HandleDuck;
        GameEvents.OnCollect -= HandleCollect;
        GameEvents.OnHit -= HandleHit;
        GameEvents.OnComboChanged -= HandleCombo;
        GameEvents.OnScoreChanged -= HandleScore;
        GameEvents.OnSectionChanged -= HandleSection;
        GameEvents.OnVictory -= HandleVictory;
        GameEvents.OnGameOver -= HandleGameOver;

        if (!written && duration > 1f) WriteRow("Abandono");
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(sessionId)) ResetStatics();
        attempt++;
    }

    private void Update()
    {
        if (SectionManager.IsRunning) duration += Time.deltaTime;
    }

    private void HandleJump() => jumps++;
    private void HandleDuck() => ducks++;
    private void HandleHit() => hits++;
    private void HandleScore(int value) => score = value;
    private void HandleSection(int index, string name) => section = name;

    private void HandleCollect(CollectibleType type)
    {
        if (type == CollectibleType.Horseshoe) horseshoes++;
        else relays++;
    }

    private void HandleCombo(int combo, int multiplier)
    {
        if (combo > maxCombo) maxCombo = combo;
    }

    private void HandleVictory() => WriteRow("Victoria");
    private void HandleGameOver() => WriteRow("Derrota");

    private void WriteRow(string result)
    {
        if (written) return;
        written = true;

        string row = string.Join(",",
            sessionId,
            attempt,
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            result,
            section,
            duration.ToString("F1", CultureInfo.InvariantCulture),
            jumps,
            ducks,
            horseshoes,
            relays,
            hits,
            maxCombo,
            score);

        Debug.Log($"[DynamicsLogger] {row}");

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
            Debug.LogWarning($"[DynamicsLogger] No se pudo escribir el archivo: {e.Message}");
        }
#endif
    }
}