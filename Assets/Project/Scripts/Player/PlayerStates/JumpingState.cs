using UnityEngine;


public class JumpingState : IPlayerState
{
    private readonly PlayerController player;
    private float timeInState;

    public JumpingState(PlayerController player) => this.player = player;

    public void Enter()
    {
        timeInState = 0f;
        player.SetJumpPose(true);
    }

    public void Tick()
    {
        timeInState += Time.deltaTime;
        float vy = player.VerticalVelocity;

        if (player.JumpReleased && vy > 0f)
            player.CutJump();

        if (player.DuckHeld)
            player.FastFall();

        player.SetTilt(vy);

        if (timeInState > 0.1f && player.IsGrounded && vy <= 0.01f)
        {
            player.Machine.ChangeState(player.DuckHeld
                ? (IPlayerState)player.Ducking
                : player.Running);
        }
    }

    public void Exit()
    {
        player.SetJumpPose(false);
        player.ResetTilt();
    }
}
