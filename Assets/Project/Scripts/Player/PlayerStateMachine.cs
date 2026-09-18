


public class PlayerStateMachine
{
    public IPlayerState Current { get; private set; }

    public void Initialize(IPlayerState startState)
    {
        Current = startState;
        Current.Enter();
    }

    public void ChangeState(IPlayerState next)
    {
        if (next == null || next == Current) return;
        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void Tick() => Current?.Tick();
}
