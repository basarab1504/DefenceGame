using System;
using UniRx;
using UnityEngine;
using Zenject;

public class Bullet : MonoBehaviour, IAmmo
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float lifetime;

    private Bullet.Pool pool;

    private Rigidbody bulletRigidbody;

    void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        Observable.Timer(TimeSpan.FromSeconds(lifetime))
        .TakeUntilDisable(this)
        .Subscribe(x => DestroyBullet());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<HP>(out var hp))
        {
            hp.RecieveDamage(damage);
        }
        DestroyBullet();
    }

    [Inject]
    public void Construct(Bullet.Pool pool)
    {
        this.pool = pool;
    }

    private void DestroyBullet()
    {
        pool.Despawn(this);
    }

    public void Fire(Vector3 force)
    {
        bulletRigidbody.AddForce(force, ForceMode.Impulse);
    }

    public class Pool : MemoryPool<Bullet>
    {
        protected override void OnDespawned(Bullet item)
        {
            base.OnDespawned(item);
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(Bullet item)
        {
            base.OnSpawned(item);
            item.gameObject.SetActive(true);
        }

        protected override void Reinitialize(Bullet item)
        {
            base.Reinitialize(item);
            item.bulletRigidbody.velocity = Vector3.zero;
        }
    }
}
