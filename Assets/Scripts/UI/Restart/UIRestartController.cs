using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIRestartController : MonoBehaviour
{
    [SerializeField]
    private Button restart;

    private CompositeDisposable disposables = new CompositeDisposable();

    // todo: в целом окно не должно бы само себя открывать, даже на эвент, но думаю можно пренебречь
    void Awake()
    {
        MessageBroker.Default
        .Receive<PlayerDead>()
        .Subscribe(e =>
        {
            SetState(true);
        })
        .AddTo(disposables);
    }

    void OnEnable()
    {
        restart.OnClickAsObservable()
        .Subscribe(OnClick)
        .AddTo(disposables);
    }

    void OnDisable()
    {
        disposables.Clear();
    }

    public void SetState(bool state)
    {
        transform.GetChild(0).gameObject.SetActive(state);
    }

    void OnClick(Unit u)
    {
        MessageBroker.Default.Publish<GameRestarted>(new GameRestarted());
        SetState(false);
    }
}