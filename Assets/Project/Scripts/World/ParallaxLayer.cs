using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("0 = fija, 1 = misma velocidad que el suelo")]
    [Range(0f, 1f)] [SerializeField] private float speedFactor = 0.3f;
    [Tooltip("Regresar al inicio al recorrer un tile (para capas repetibles)")]
    [SerializeField] private bool loop = true;

    private float startX;
    private float tileWidth;

    private void Start()
    {
        startX = transform.position.x;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        tileWidth = sprite.rect.width / sprite.pixelsPerUnit * transform.lossyScale.x;
    }

    private void Update()
    {
        if (!SectionManager.IsRunning) return;

        Vector3 pos = transform.position;
        pos.x -= SectionManager.Speed * speedFactor * Time.deltaTime;

        if (loop && startX - pos.x >= tileWidth)
            pos.x += tileWidth;

        transform.position = pos;
    }
}
