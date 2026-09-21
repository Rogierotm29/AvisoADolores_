using UnityEngine;


[CreateAssetMenu(fileName = "SectionData", menuName = "Aviso a Dolores/Section Data")]
public class SectionData : ScriptableObject
{
    public string sectionName = "Querétaro";
    [Tooltip("Instrucción que se muestra al entrar al tramo (vacío = ninguna)")]
    public string hint = "ESPACIO = saltar";

    [Tooltip("Duración aproximada del tramo en segundos si no hay choques")]
    public float duration = 30f;

    [Header("Velocidad (unidades por segundo)")]
    public float startSpeed = 6f;
    public float endSpeed = 7f;

    [Header("Generación de obstáculos")]
    [Tooltip("Segundos mínimos entre segmentos")]
    public float minSpawnInterval = 2.2f;
    [Tooltip("Segundos máximos entre segmentos")]
    public float maxSpawnInterval = 2.8f;
    [Tooltip("Segmentos (chunks) que pueden aparecer al azar en este tramo")]
    public GameObject[] chunks;

    [Header("Segmentos especiales")]
    [Tooltip("Se genera una vez al iniciar el tramo (por ejemplo, el letrero que enseña una acción)")]
    public GameObject introChunk;
    [Tooltip("Se genera una vez a la mitad del tramo (caballo de relevo)")]
    public GameObject relayChunk;
    [Range(0f, 1f)] public float relayAt = 0.5f;
}
