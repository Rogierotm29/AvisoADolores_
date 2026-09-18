using UnityEngine;


public class HitState : IPlayerState
{
    private readonly PlayerController player;
    private float timer;

    public HitState(PlayerController player) => this.player = player;

    public void Enter() => timer = 0f;

    public void Tick()
    {
        timer += Time.deltaTime;
        if (timer < player.Config.hitStunTime) return;

        player.Machine.ChangeState(player.IsGrounded
            ? (IPlayerState)player.Running
            : player.Jumping);
    }

    public void Exit() { }
}
