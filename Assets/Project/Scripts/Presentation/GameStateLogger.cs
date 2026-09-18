using UnityEngine;


public class GameStateLogger : MonoBehaviour
{
    private void OnEnable() => GameEvents.OnGameStateChanged += Log;
    private void OnDisable() => GameEvents.OnGameStateChanged -= Log;

    private void Log(GameStateId state) => Debug.Log($"[Estado del juego] {state}");
}
