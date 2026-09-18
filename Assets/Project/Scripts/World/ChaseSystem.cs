using UnityEngine;


public class ChaseSystem : MonoBehaviour
{
    [SerializeField] private GameConfig config;

    public float Distance { get; private set; }
    public bool InDanger => Distance < config.dangerThreshold;

    private float timeSinceHit;
    private bool caught;

    private void OnEnable()
    {
        GameEvents.OnHit += HandleHit;
        GameEvents.OnCollect += HandleCollect;
    }

    private void OnDisable()
    {
        GameEvents.OnHit -= HandleHit;
        GameEvents.OnCollect -= HandleCollect;
    }

    private void Start()
    {
        Distance = config.startDistance;
        timeSinceHit = config.recoveryDelay;
        GameEvents.RaiseDistanceChanged(Distance);
    }

    private void Update()
    {
        if (!SectionManager.IsRunning || caught) return;

        timeSinceHit += Time.deltaTime;
        if (timeSinceHit >= config.recoveryDelay && Distance < 100f)
            SetDistance(Distance + config.recoveryPerSecond * Time.deltaTime);
    }

    private void HandleHit()
    {
        if (caught) return;
        timeSinceHit = 0f;
        SetDistance(Distance - config.hitPenalty);
    }

    private void HandleCollect(CollectibleType type)
    {
        if (caught) return;
        float bonus = type == CollectibleType.Relay
            ? config.relayDistanceBonus
            : config.horseshoeDistanceBonus;
        SetDistance(Distance + bonus);
    }

    private void SetDistance(float value)
    {
        Distance = Mathf.Clamp(value, 0f, 100f);
        GameEvents.RaiseDistanceChanged(Distance);

        if (Distance <= 0f && !caught)
        {
            caught = true;
            Debug.Log("[ChaseSystem] La patrulla alcanzó al mensajero");
            GameEvents.RaiseGameOver();
        }
    }
}
