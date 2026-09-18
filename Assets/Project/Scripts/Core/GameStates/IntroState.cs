using UnityEngine;


public class IntroState : IGameState
{
    private readonly GameManager game;
    public GameStateId Id => GameStateId.Intro;

    public IntroState(GameManager game) => this.game = game;

    public void Enter()
    {
        Time.timeScale = 1f;
        SectionManager.IsRunning = false;
        game.Player.ControlEnabled = false;
    }

    public void Tick()
    {
        if (game.ConfirmPressed)
            game.Machine.ChangeState(game.Playing);
    }

    public void Exit() { }
}
