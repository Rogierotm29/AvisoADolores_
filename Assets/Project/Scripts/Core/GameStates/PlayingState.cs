using UnityEngine;


public class PlayingState : IGameState
{
    private readonly GameManager game;
    public GameStateId Id => GameStateId.Playing;

    public PlayingState(GameManager game) => this.game = game;

    public void Enter()
    {
        Time.timeScale = 1f;
        SectionManager.IsRunning = true;
        game.Player.ControlEnabled = true;
    }

    public void Tick()
    {
        game.ElapsedTime += Time.deltaTime;

        if (game.PausePressed)
            game.Machine.ChangeState(game.Paused);
    }

    public void Exit() { }
}
