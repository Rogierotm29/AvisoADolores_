
public class GameStateMachine
{
    public IGameState Current { get; private set; }

    public void ChangeState(IGameState next)
    {
        if (next == null || next == Current) return;
        Current?.Exit();
        Current = next;
        Current.Enter();
        GameEvents.RaiseGameStateChanged(Current.Id);
    }

    public void Tick() => Current?.Tick();
}
