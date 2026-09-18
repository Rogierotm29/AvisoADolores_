

public interface IGameState
{
    GameStateId Id { get; }
    void Enter();
    void Tick();
    void Exit();
}
