using System;
using UniRx;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IDamageable, ICharacter
{

    [SerializeField]
    private float meleeRange;
    [SerializeField]
    private float meleeCooldown;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float acceleration;

    private Enemy.Pool pool;

    private HP HP;
    private Rigidbody enemyRigidbody;
    private LootDrop lootDrop;

    public IObservable<int> HPChanged => HP.HPChanged;
    public int MaxHP => HP.MaxHP;

    public Subject<CharacterState> State { get; } = new Subject<CharacterState>();

    private void Awake()
    {
        HP = GetComponent<HP>();
        lootDrop = GetComponentInChildren<LootDrop>();
        enemyRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        HPChanged
        .Where(x => x == 0)
        .TakeUntilDisable(this)
        .Subscribe((x) => DestroyEnemy());

        Observable.EveryUpdate()
        .CombineLatest(Player.PlayerInstance, (l, r) => r)
        .Select(r => IsPlayerInRange(r.transform.position))
        .DistinctUntilChanged()
        .Where(inRange => !inRange)
        .TakeUntilDisable(this)
        .Subscribe(x => {
            State.OnNext(CharacterState.Running);
        });

        Observable.EveryUpdate()
        .CombineLatest(Player.PlayerInstance, (l, r) => r)
        .Where(r => IsPlayerInRange(r.transform.position))
        .ThrottleFirst(TimeSpan.FromSeconds(meleeCooldown))
        .TakeUntilDisable(this)
        .Subscribe(HandleMelee);

        Observable.EveryFixedUpdate()
        .CombineLatest(Player.PlayerInstance, (l, r) => r)
        .Where(r => !IsPlayerInRange(r.transform.position))
        .TakeUntilDisable(this)
        .Subscribe(HandleMovement);
    }

    private void OnDisable()
    {
        enemyRigidbody.velocity = Vector3.zero;
    }

    [Inject]
    public void Construct(Enemy.Pool pool)
    {
        this.pool = pool;
    }

    public void RecieveDamage(int damage)
    {
        HP.RecieveDamage(damage);
    }

    private void HandleMelee(Player player)
    {
        player.RecieveDamage(damage);
        State.OnNext(CharacterState.Attack);
    }

    private void HandleMovement(Player player)
    {
        transform.LookAt(player.transform);
        enemyRigidbody.position += transform.forward * acceleration * Time.fixedDeltaTime;
    }

    private bool IsPlayerInRange(Vector3 position)
    {
        Vector3 distance = transform.position - position;
        return Vector3.SqrMagnitude(distance) <= (meleeRange * meleeRange);
    }

    private void DestroyEnemy()
    {
        lootDrop?.Drop();
        gameObject.SetActive(false);
        pool.Despawn(this);
    }

    public class Pool : MemoryPool<Enemy>
    {
        protected override void OnDespawned(Enemy item)
        {
            base.OnDespawned(item);
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(Enemy item)
        {
            base.OnSpawned(item);
            item.gameObject.SetActive(true);
        }

        protected override void Reinitialize(Enemy item)
        {
            base.Reinitialize(item);
            item.enemyRigidbody.velocity = Vector3.zero;
        }
    }
}
