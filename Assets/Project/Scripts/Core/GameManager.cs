using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string BestScoreKey = "AvisoADolores_BestScore";

    [SerializeField] private PlayerController player;
    [SerializeField] private GameConfig config;
    [SerializeField] private string menuSceneName = "MenuScene";

    public PlayerController Player => player;

    // Estados 
    public GameStateMachine Machine { get; private set; }
    public IntroState Intro { get; private set; }
    public PlayingState Playing { get; private set; }
    public PausedState Paused { get; private set; }
    public VictoryState Victory { get; private set; }
    public GameOverState GameOver { get; private set; }

    // Datos de la partida
    public int Score { get; private set; }
    public int Horseshoes { get; private set; }
    public float ElapsedTime { get; set; }
    public int BestScore { get; private set; }
    public string SectionReached { get; private set; } = "";

    // Entrada
    private InputAction confirmAction;
    private InputAction pauseAction;
    public bool ConfirmPressed => confirmAction.WasPressedThisFrame();
    public bool EnterPressed => Keyboard.current != null &&
        (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame);
    public bool PausePressed => pauseAction.WasPressedThisFrame();

    private void Awake()
    {
        Instance = this;
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        confirmAction = new InputAction("Confirm", InputActionType.Button);
        confirmAction.AddBinding("<Keyboard>/enter");
        confirmAction.AddBinding("<Keyboard>/numpadEnter");
        confirmAction.AddBinding("<Mouse>/leftButton");

        pauseAction = new InputAction("Pause", InputActionType.Button);
        pauseAction.AddBinding("<Keyboard>/escape");
        pauseAction.AddBinding("<Keyboard>/p");

        Machine = new GameStateMachine();
        Intro = new IntroState(this);
        Playing = new PlayingState(this);
        Paused = new PausedState(this);
        Victory = new VictoryState(this);
        GameOver = new GameOverState(this);
    }

    private void OnEnable()
    {
        confirmAction.Enable();
        pauseAction.Enable();
        GameEvents.OnCollect += HandleCollect;
        GameEvents.OnSectionChanged += HandleSection;
        GameEvents.OnVictory += HandleVictory;
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        confirmAction.Disable();
        pauseAction.Disable();
        GameEvents.OnCollect -= HandleCollect;
        GameEvents.OnSectionChanged -= HandleSection;
        GameEvents.OnVictory -= HandleVictory;
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void OnDestroy()
    {
        confirmAction.Dispose();
        pauseAction.Dispose();
        Time.timeScale = 1f;
    }

    private void Start()
    {
        Machine.ChangeState(Intro);
        GameEvents.RaiseScoreChanged(Score);
    }

    private void Update() => Machine.Tick();

    // ---------- Observadores 

    private void HandleCollect(CollectibleType type)
    {
        if (type == CollectibleType.Horseshoe) Horseshoes++;
        Score += type == CollectibleType.Relay ? config.pointsPerHorseshoe * 5 : config.pointsPerHorseshoe;
        GameEvents.RaiseScoreChanged(Score);
    }

    private void HandleSection(int index, string sectionName) => SectionReached = sectionName;
    private void HandleVictory() => Machine.ChangeState(Victory);
    private void HandleGameOver() => Machine.ChangeState(GameOver);

    // ---------- Acciones 

    public void StartGame()
    {
        if (Machine.Current == Intro) Machine.ChangeState(Playing);
    }

    public void TogglePause()
    {
        if (Machine.Current == Playing) Machine.ChangeState(Paused);
        else if (Machine.Current == Paused) Machine.ChangeState(Playing);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void SaveBestScore()
    {
        if (Score <= BestScore) return;
        BestScore = Score;
        PlayerPrefs.SetInt(BestScoreKey, BestScore);
        PlayerPrefs.Save();
    }
}
