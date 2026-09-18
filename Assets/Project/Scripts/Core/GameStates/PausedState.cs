using UnityEngine;


public class PausedState : IGameState
{
    private readonly GameManager game;
    public GameStateId Id => GameStateId.Paused;

    public PausedState(GameManager game) => this.game = game;

    public void Enter()
    {
        Time.timeScale = 0f;
        game.Player.ControlEnabled = false;
    }

    public void Tick()
    {
        if (game.PausePressed)
            game.Machine.ChangeState(game.Playing);
    }

    public void Exit() => Time.timeScale = 1f;
}
