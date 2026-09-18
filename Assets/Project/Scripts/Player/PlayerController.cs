using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameConfig config;
    [Tooltip("Hijo con el SpriteRenderer y el Animator")]
    [SerializeField] private Transform visual;
    [SerializeField] private LayerMask groundLayer;

    [Header("Pose de salto")]
    [Tooltip("Momento normalizado (0 a 1) de la animación de galope que se usa como pose en el aire")]
    [Range(0f, 0.99f)][SerializeField] private float jumpPoseTime = 0f;

    // Componentes
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator animator;
    private SpriteRenderer sprite;

    // Estados 
    public PlayerStateMachine Machine { get; private set; }
    public RunningState Running { get; private set; }
    public JumpingState Jumping { get; private set; }
    public DuckingState Ducking { get; private set; }
    public HitState Hit { get; private set; }

    // Entrada
    private InputAction jumpAction;
    private InputAction duckAction;

    public bool ControlEnabled { get; set; } = true;
    public bool JumpPressed => ControlEnabled && jumpAction.WasPressedThisFrame();
    public bool JumpReleased => jumpAction.WasReleasedThisFrame();
    public bool DuckHeld => ControlEnabled && duckAction.IsPressed();

    public GameConfig Config => config;
    public float VerticalVelocity => rb.linearVelocity.y;
    public bool IsGrounded { get; private set; }
    public bool IsInvulnerable { get; private set; }


    private Vector2 colliderSize;
    private Vector2 colliderOffset;
    private Vector3 visualScale;
    private Vector3 visualPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        animator = visual.GetComponent<Animator>();
        sprite = visual.GetComponent<SpriteRenderer>();

        colliderSize = col.size;
        colliderOffset = col.offset;
        visualScale = visual.localScale;
        visualPosition = visual.localPosition;

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Keyboard>/w");
        jumpAction.AddBinding("<Keyboard>/upArrow");

        duckAction = new InputAction("Duck", InputActionType.Button);
        duckAction.AddBinding("<Keyboard>/s");
        duckAction.AddBinding("<Keyboard>/downArrow");

        Machine = new PlayerStateMachine();
        Running = new RunningState(this);
        Jumping = new JumpingState(this);
        Ducking = new DuckingState(this);
        Hit = new HitState(this);
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        duckAction.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        duckAction.Disable();
    }

    private void OnDestroy()
    {
        jumpAction.Dispose();
        duckAction.Dispose();
    }

    private void Start() => Machine.Initialize(Running);

    private void Update()
    {
        IsGrounded = CheckGrounded();
        Machine.Tick();
    }

    // ---------- Acciones usadas por los estados 

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
    }

    public void CutJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * config.jumpCutMultiplier);
    }

    public void FastFall()
    {
        float vy = Mathf.Min(rb.linearVelocity.y, -config.fastFallSpeed);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, vy);
    }

    public void SetDuck(bool active)
    {
        if (active)
        {
            float h = colliderSize.y * config.duckHeightMultiplier;
            float bottom = colliderOffset.y - colliderSize.y / 2f;
            col.size = new Vector2(colliderSize.x, h);
            col.offset = new Vector2(colliderOffset.x, bottom + h / 2f);


            float newScaleY = visualScale.y * config.duckHeightMultiplier;
            float spriteBottom = sprite.sprite.bounds.min.y;
            visual.localScale = new Vector3(visualScale.x, newScaleY, visualScale.z);
            visual.localPosition = new Vector3(
                visualPosition.x,
                visualPosition.y + spriteBottom * (visualScale.y - newScaleY),
                visualPosition.z);
        }
        else
        {
            col.size = colliderSize;
            col.offset = colliderOffset;
            visual.localScale = visualScale;
            visual.localPosition = visualPosition;
        }
    }

    public void SetJumpPose(bool active)
    {
        if (animator == null) return;
        if (active)
        {
            int state = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            animator.Play(state, 0, jumpPoseTime);
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1f;
        }
    }

    public void SetTilt(float verticalVelocity)
    {
        float angle = Mathf.Clamp(verticalVelocity * config.tiltPerVelocity, config.tiltDown, config.tiltUp);
        visual.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ResetTilt() => visual.localRotation = Quaternion.identity;

    // ---------- Colisiones

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Obstacle") || IsInvulnerable) return;

        GameEvents.RaiseHit();
        Machine.ChangeState(Hit);
        StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        IsInvulnerable = true;
        float t = 0f;
        while (t < config.invulnerabilityTime)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(config.blinkInterval);
            t += config.blinkInterval;
        }
        sprite.enabled = true;
        IsInvulnerable = false;
    }

    // ---------- Utilidades ----------

    private bool CheckGrounded()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer) != null;
    }

    private void OnDrawGizmosSelected()
    {
        var c = GetComponent<BoxCollider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y - 0.05f), new Vector3(b.size.x * 0.9f, 0.1f));
    }
}