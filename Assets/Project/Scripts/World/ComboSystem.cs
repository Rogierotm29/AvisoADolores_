using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] private GameConfig config;

    [Tooltip("Segundos que tiene el jugador para recoger otra herradura antes de perder la racha")]
    [SerializeField] private float comboWindow = 6f;

    [Tooltip("Multiplicador maximo que puede alcanzar la racha")]
    [SerializeField] private int maxMultiplier = 4;

    public int Combo { get; private set; }
    public int Multiplier => Mathf.Clamp(Combo, 1, maxMultiplier);
    public int MaxCombo { get; private set; }

    private float windowTimer;

    private void OnEnable()
    {
        GameEvents.OnCollect += HandleCollect;
        GameEvents.OnHit += HandleHit;

    }

    private void OnDisable()
    {
        GameEvents.OnCollect -= HandleCollect;
        GameEvents.OnHit -= HandleHit;
    }

    private void Start() => GameEvents.RaiseComboChanged(0, 1);

    private void Update()
    {
        if (!SectionManager.IsRunning || Combo == 0) return;

        windowTimer += Time.deltaTime;
        if (windowTimer <= 0f) ResetCombo();
    }

    private void HandleCollect(CollectibleType type)
    {
        if (type == CollectibleType.Relay)
        {
            if (Combo > 0) windowTimer = comboWindow;
            return;
        }

        Combo++;
        windowTimer = comboWindow;
        if (Combo > MaxCombo) MaxCombo = Combo;

        int bonus = config.pointsPerHorseshoe * (Multiplier - 1);
        if (bonus > 0) GameEvents.RaiseBonusScore(bonus);

        GameEvents.RaiseComboChanged(Combo, Multiplier);
    }

    private void HandleHit()
    {
        if (Combo > 0) ResetCombo();
    }

    private void ResetCombo()
    {
        Combo = 0;
        windowTimer = 0f;
        GameEvents.RaiseComboChanged(0, 1); 
    }
}
