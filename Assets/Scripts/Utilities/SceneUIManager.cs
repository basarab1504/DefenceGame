using System;
using UnityEngine;

public class SceneUIManager : MonoBehaviour, UIManager
{
    public static UIManager Instance;

    [SerializeField]
    private UIPlayerController playerController;
    [SerializeField]
    private UIRestartController restartController;

    private UIState currentState;
    public event Action<UIState> StateChanged;

    void Awake()
    {
        Instance = this;
    }

    public void SetState(UIState state)
    {
        currentState?.OnExit();
        currentState = state;
        currentState.OnEnter();
        StateChanged?.Invoke(currentState);
    }
}