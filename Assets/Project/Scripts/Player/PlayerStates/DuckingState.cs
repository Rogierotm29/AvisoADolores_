
public class DuckingState : IPlayerState
{
    private readonly PlayerController player;

    public DuckingState(PlayerController player) => this.player = player;

    public void Enter() => player.SetDuck(true);

    public void Tick()
    {
        if (player.JumpPressed)
        {
            player.Jump();
            player.Machine.ChangeState(player.Jumping);
        }
        else if (!player.DuckHeld)
        {
            player.Machine.ChangeState(player.Running);
        }
    }

    public void Exit() => player.SetDuck(false);
}
