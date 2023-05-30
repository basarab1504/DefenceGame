using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Zenject;

public class Player : MonoBehaviour, IDamageable, ICharacter
{
    private static ReactiveProperty<Player> playerInstance = new ReactiveProperty<Player>();
    public static IObservable<Player> PlayerInstance => playerInstance;

    private HP HP;
    private IWeapon weapon;
    private LootScanner scanner;
    private Stack<ILoot> inventory = new Stack<ILoot>();

    private Player.Pool pool;


    private CompositeDisposable disposables = new CompositeDisposable();

    public IObservable<int> HPChanged => HP.HPChanged;
    public int MaxHP => HP.MaxHP;

    public Subject<CharacterState> State { get; } = new Subject<CharacterState>();

    void Awake()
    {
        scanner = GetComponentInChildren<LootScanner>();
        HP = GetComponent<HP>();
        weapon = GetComponent<IWeapon>();
    }

    void OnEnable()
    {
        playerInstance.Value = this;

        scanner.Picked
        .Subscribe(OnLootPicked)
        .AddTo(disposables);

        HPChanged
        .Where(x => x <= 0)
        .Subscribe((x) => DestroyPlayer())
        .AddTo(disposables);
    }

    void OnDisable()
    {
        disposables.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IBase>(out var playerBase))
        {
            weapon.SetState(false);
            while (inventory.TryPop(out var loot))
            {
                playerBase.CollectLoot(loot);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IBase>(out var playerBase))
        {
            weapon.SetState(true);
        }
    }

    [Inject]
    public void Construct(Player.Pool pool)
    {
        this.pool = pool;
    }

    public void RecieveDamage(int damage)
    {
        HP.RecieveDamage(damage);
    }

    private void OnLootPicked(ILoot loot)
    {
        inventory.Push(loot);
    }

    private void DestroyPlayer()
    {
        inventory.Clear();
        gameObject.SetActive(false);
        pool.Despawn(this);
        MessageBroker.Default.Publish<PlayerDead>(new PlayerDead());
    }

    public class Pool : MemoryPool<Player>
    {
        protected override void OnDespawned(Player item)
        {
            base.OnDespawned(item);
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(Player item)
        {
            base.OnSpawned(item);
            item.gameObject.SetActive(true);
        }
    }
}
