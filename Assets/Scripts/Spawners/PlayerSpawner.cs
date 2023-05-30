using UniRx;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private CompositeDisposable disposables = new CompositeDisposable();
    
    private Player.Pool pool;

    [Inject]
    public void Construct(Player.Pool pool)
    {
        this.pool = pool;
    }

    void Start()
    {
        MessageBroker.Default
        .Receive<GameRestarted>()
        .Subscribe((e) =>
        {
            SpawnPlayer();
        })
        .AddTo(disposables);

        SpawnPlayer();
    }

    void OnDisable()
    {
        disposables.Clear();
    }

    private void SpawnPlayer()
    {
        var player = pool.Spawn();
        player.transform.position = transform.position;
    }
}