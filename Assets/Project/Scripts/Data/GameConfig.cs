using UnityEngine;


[CreateAssetMenu(fileName = "GameConfig", menuName = "Aviso a Dolores/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Salto")]
    public float jumpForce = 14f;
    [Tooltip("Multiplicador de la velocidad vertical al soltar el salto antes de tiempo")]
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.45f;
    [Tooltip("Velocidad de caída al presionar abajo en el aire")]
    public float fastFallSpeed = 22f;

    [Header("Inclinación durante el salto (grados)")]
    public float tiltUp = 12f;
    public float tiltDown = -10f;
    public float tiltPerVelocity = 1.2f;

    [Header("Agacharse")]
    [Range(0.3f, 1f)] public float duckHeightMultiplier = 0.55f;

    [Header("Choque")]
    public float hitStunTime = 0.3f;
    public float invulnerabilityTime = 1f;
    public float blinkInterval = 0.08f;

    [Header("Persecución (distancia de 0 a 100)")]
    public float startDistance = 100f;
    public float hitPenalty = 30f;
    [Tooltip("Distancia que se recupera por segundo sin choques")]
    public float recoveryPerSecond = 5f;
    [Tooltip("Segundos sin choques antes de empezar a recuperar distancia")]
    public float recoveryDelay = 1f;
    public float horseshoeDistanceBonus = 2f;
    public float relayDistanceBonus = 40f;
    [Tooltip("Distancia por debajo de la cual se considera peligro")]
    public float dangerThreshold = 30f;

    [Header("Puntuación")]
    public int pointsPerHorseshoe = 10;
}
