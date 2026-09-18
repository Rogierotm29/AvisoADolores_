using UnityEngine;


public class PatrolFollower : MonoBehaviour
{
    [Tooltip("Posición X de la patrulla cuando la distancia es 100 (fuera de pantalla)")]
    [SerializeField] private float farX = -13f;
    [Tooltip("Posición X de la patrulla cuando la distancia es 0 (encima del jugador)")]
    [SerializeField] private float nearX = -6.2f;
    [SerializeField] private float smoothTime = 0.35f;

    private float targetX;
    private float velocity;

    private void Awake()
    {
        targetX = farX;
        Vector3 p = transform.position;
        transform.position = new Vector3(farX, p.y, p.z);
    }

    private void OnEnable() => GameEvents.OnDistanceChanged += HandleDistance;
    private void OnDisable() => GameEvents.OnDistanceChanged -= HandleDistance;

    private void HandleDistance(float distance)
    {
        targetX = Mathf.Lerp(nearX, farX, distance / 100f);
    }

    private void Update()
    {
        Vector3 p = transform.position;
        p.x = Mathf.SmoothDamp(p.x, targetX, ref velocity, smoothTime);
        transform.position = p;
    }
}
