using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    // ------------------------------
    //
    //   Global event manager for all in-game and UI events
    //
    //   Created: 27/05/2024
    //
    // ------------------------------

    // Gameplay events
    public static event UnityAction SwitchToIntro;
    public static void OnSwitchToIntro() => SwitchToIntro?.Invoke();

    public static event UnityAction SwitchToEditing;
    public static void OnSwitchToEditing() => SwitchToEditing?.Invoke();

    public static event UnityAction SwitchToGameplay;
    public static void OnSwitchToGameplay() => SwitchToGameplay?.Invoke();

    public static event UnityAction SwitchToResults;
    public static void OnSwitchToResults() => SwitchToResults?.Invoke();

    public static event UnityAction SwitchToGameOver;
    public static void OnSwitchToGameOver() => SwitchToGameOver?.Invoke();
}
