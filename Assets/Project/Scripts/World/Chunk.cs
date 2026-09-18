using UnityEngine;


public class Chunk : MonoBehaviour
{
    [Tooltip("Ancho del segmento en unidades, desde su origen hacia la derecha")]
    [SerializeField] private float width = 6f;
    [Tooltip("Posición X a partir de la cual el segmento se considera fuera de pantalla")]
    [SerializeField] private float despawnX = -14f;

    public float Width => width;

    private Collectible[] collectibles;

    private void Awake()
    {
        collectibles = GetComponentsInChildren<Collectible>(true);
    }

    private void OnEnable()
    {
        foreach (Collectible c in collectibles)
            c.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!SectionManager.IsRunning) return;

        transform.position += Vector3.left * SectionManager.Speed * Time.deltaTime;

        if (transform.position.x + width < despawnX)
            gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 p = transform.position;
        Gizmos.DrawLine(p, p + Vector3.right * width);
        Gizmos.DrawLine(p + Vector3.right * width, p + new Vector3(width, 3f));
    }
}
