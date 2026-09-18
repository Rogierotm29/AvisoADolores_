
public class RunningState : IPlayerState
{
    private readonly PlayerController player;

    public RunningState(PlayerController player) => this.player = player;

    public void Enter() { }

    public void Tick()
    {
        if (!player.IsGrounded)
        {
            player.Machine.ChangeState(player.Jumping);
            return;
        }

        if (player.JumpPressed)
        {
            player.Jump();
            player.Machine.ChangeState(player.Jumping);
        }
        else if (player.DuckHeld)
        {
            player.Machine.ChangeState(player.Ducking);
        }
    }

    public void Exit() { }
}
