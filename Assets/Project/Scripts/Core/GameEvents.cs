using System;
using UnityEditorInternal;


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

    //Lo de la actividad de mecanica
    public static event Action<int, int> OnComboChanged;

    public static event Action<int> OnBonusScore;

    //Lo de la actividad de dinamica
    public static event Action OnJump;
    public static event Action OnDuck;

    public static void RaiseHit() => OnHit?.Invoke();
    public static void RaiseCollect(CollectibleType type) => OnCollect?.Invoke(type);
    public static void RaiseDistanceChanged(float value) => OnDistanceChanged?.Invoke(value);
    public static void RaiseSectionChanged(int index, string name) => OnSectionChanged?.Invoke(index, name);
    public static void RaiseProgressChanged(float value) => OnProgressChanged?.Invoke(value);
    public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public static void RaiseGameStateChanged(GameStateId state) => OnGameStateChanged?.Invoke(state);
    public static void RaiseVictory() => OnVictory?.Invoke();
    public static void RaiseGameOver() => OnGameOver?.Invoke();

    //Lo de la actividad de mecanica
    public static void RaiseComboChanged(int combo, int multiplier) => OnComboChanged?.Invoke(combo, multiplier);
    public static void RaiseBonusScore(int points) => OnBonusScore?.Invoke(points);

    //Lo de la actividad de dinamica
    public static void RaiseJump() => OnJump?.Invoke();
    public static void RaiseDuck() => OnDuck?.Invoke();

}
