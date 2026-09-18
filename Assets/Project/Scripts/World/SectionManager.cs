using UnityEngine;

public class SectionManager : MonoBehaviour
{
    public static SectionManager Instance { get; private set; }

    public static float Speed { get; private set; }

    public static bool IsRunning { get; set; }

    [SerializeField] private SectionData[] sections;
    [Tooltip("Iniciar el recorrido automáticamente (útil para pruebas antes de tener los estados del juego)")]
    [SerializeField] private bool autoStart = true;

    [Header("Modificadores de velocidad")]
    [SerializeField] private float hitSpeedMultiplier = 0.6f;
    [SerializeField] private float hitRecoveryTime = 1.5f;
    [SerializeField] private float horseshoeBoost = 1.05f;
    [SerializeField] private float relayBoost = 1.35f;
    [SerializeField] private float boostDuration = 2f;

    public int CurrentIndex { get; private set; }
    public SectionData Current => sections[CurrentIndex];
    public float Progress => totalDistance > 0f ? traveled / totalDistance : 0f;

    public float SectionProgress
    {
        get
        {
            float start = CurrentIndex == 0 ? 0f : sectionEnds[CurrentIndex - 1];
            return Mathf.InverseLerp(start, sectionEnds[CurrentIndex], traveled);
        }
    }
    public float SecondsToGoal => Speed > 0f ? (totalDistance - traveled) / Speed : float.MaxValue;

    private float[] sectionEnds;
    private float totalDistance;
    private float traveled;
    private float speedMultiplier = 1f;
    private float boostTimer;
    private bool finished;

    private void Awake()
    {
        Instance = this;
        IsRunning = autoStart;

        sectionEnds = new float[sections.Length];
        float acc = 0f;
        for (int i = 0; i < sections.Length; i++)
        {
            SectionData s = sections[i];
            acc += s.duration * (s.startSpeed + s.endSpeed) / 2f;
            sectionEnds[i] = acc;
        }
        totalDistance = acc;
        Speed = sections[0].startSpeed;
    }

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
        GameEvents.RaiseSectionChanged(0, sections[0].sectionName);
        GameEvents.RaiseProgressChanged(0f);
    }

    private void Update()
    {
        if (!IsRunning || finished) return;

        UpdateMultiplier();

        float sectionStart = CurrentIndex == 0 ? 0f : sectionEnds[CurrentIndex - 1];
        float sectionT = Mathf.InverseLerp(sectionStart, sectionEnds[CurrentIndex], traveled);
        float baseSpeed = Mathf.Lerp(Current.startSpeed, Current.endSpeed, sectionT);
        Speed = baseSpeed * speedMultiplier;

        traveled += Speed * Time.deltaTime;
        GameEvents.RaiseProgressChanged(Progress);

        if (traveled >= sectionEnds[CurrentIndex])
        {
            if (CurrentIndex < sections.Length - 1)
            {
                CurrentIndex++;
                GameEvents.RaiseSectionChanged(CurrentIndex, Current.sectionName);
            }
            else
            {
                finished = true;
                IsRunning = false;
                Speed = 0f;
                GameEvents.RaiseVictory();
            }
        }
    }

    private void UpdateMultiplier()
    {
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f) speedMultiplier = 1f;
        }
        else if (speedMultiplier < 1f)
        {
            speedMultiplier = Mathf.MoveTowards(speedMultiplier, 1f,
                (1f - hitSpeedMultiplier) / hitRecoveryTime * Time.deltaTime);
        }
    }

    private void HandleHit()
    {
        boostTimer = 0f;
        speedMultiplier = hitSpeedMultiplier;
    }

    private void HandleCollect(CollectibleType type)
    {
        float boost = type == CollectibleType.Relay ? relayBoost : horseshoeBoost;
        if (boost > speedMultiplier)
        {
            speedMultiplier = boost;
            boostTimer = boostDuration;
        }
    }
}
