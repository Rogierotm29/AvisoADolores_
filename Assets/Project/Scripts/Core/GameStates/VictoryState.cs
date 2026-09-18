
public class VictoryState : IGameState
{
    private readonly GameManager game;
    public GameStateId Id => GameStateId.Victory;

    public VictoryState(GameManager game) => this.game = game;

    public void Enter()
    {
        SectionManager.IsRunning = false;
        game.Player.ControlEnabled = false;
        game.SaveBestScore();
    }

    public void Tick()
    {
        if (game.EnterPressed) game.Restart();
    }

    public void Exit() { }
}
