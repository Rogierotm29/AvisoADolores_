using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleType type = CollectibleType.Horseshoe;

    [Header("Animación de flotado")]
    [SerializeField] private float bobHeight = 0.12f;
    [SerializeField] private float bobSpeed = 4f;

    private Vector3 basePosition;

    private void Awake()
    {
        basePosition = transform.localPosition;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = basePosition + Vector3.up * offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameEvents.RaiseCollect(type);
        gameObject.SetActive(false);
    }
}
