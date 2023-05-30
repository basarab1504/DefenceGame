using System;
using UniRx;
using UnityEngine;
using Zenject;

public class Gun : MonoBehaviour, IWeapon
{
    [SerializeField]
    private float range;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float force;
    [SerializeField]
    private Transform gunPosition;

    private Bullet.Pool pool;

    [Inject]
    public void Construct(Bullet.Pool pool)
    {
        this.pool = pool;
    }

    private bool canAttack;

    void OnEnable()
    {
        Collider[] buffer = new Collider[1];

        Observable.EveryUpdate()
        .Where(x => canAttack)
        .Select(x => buffer)
        .ThrottleFirst(TimeSpan.FromSeconds(cooldown))
        .TakeUntilDisable(this)
        .Subscribe(HandleShooting);
    }

    public void SetState(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    public void Shoot(Vector3 direction)
    {
        var bullet = pool.Spawn();
        bullet.transform.position = gunPosition.position;
        bullet.Fire(direction * force);
    }

    private void HandleShooting(Collider[] buffer)
    {
        var hits = Physics.OverlapSphereNonAlloc(transform.position, range, buffer, LayerMask.GetMask("Enemy"));
        if (hits > 0)
        {
            Shoot((buffer[0].transform.position - transform.position).normalized);
        }
    }
}
