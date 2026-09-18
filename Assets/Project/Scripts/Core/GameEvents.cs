using System;

public enum CollectibleType { Horseshoe, Relay }

public enum GameStateId { Intro, Playing, Paused, Victory, GameOver }


public static class GameEvents
{
    public static event Action OnHit;
    public static event Action<CollectibleType> OnCollect;
    public static event Action<float> OnDistanceChanged;     
    public static event Action<int, string> OnSectionChanged; 
    public static event Action<float> OnProgressChanged;     
    public static event Action<int> OnScoreChanged;
    public static event Action<GameStateId> OnGameStateChanged;
    public static event Action OnVictory;
    public static event Action OnGameOver;

    public static void RaiseHit() => OnHit?.Invoke();
    public static void RaiseCollect(CollectibleType type) => OnCollect?.Invoke(type);
    public static void RaiseDistanceChanged(float value) => OnDistanceChanged?.Invoke(value);
    public static void RaiseSectionChanged(int index, string name) => OnSectionChanged?.Invoke(index, name);
    public static void RaiseProgressChanged(float value) => OnProgressChanged?.Invoke(value);
    public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public static void RaiseGameStateChanged(GameStateId state) => OnGameStateChanged?.Invoke(state);
    public static void RaiseVictory() => OnVictory?.Invoke();
    public static void RaiseGameOver() => OnGameOver?.Invoke();
}
