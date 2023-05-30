using UnityEngine;
using UniRx;
using Zenject;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Vector2 area;

    [SerializeField]
    private Enemy prefab;

    [SerializeField]
    private float perSpawn;

    [SerializeField]
    private float cooldown;
    private float currentCooldown;

    private Enemy.Pool pool;

    [Inject]
    public void Construct(Enemy.Pool pool)
    {
        this.pool = pool;
    }

    void OnEnable()
    {
        Observable.EveryUpdate()
        .ThrottleFirst(TimeSpan.FromSeconds(cooldown))
        .TakeUntilDisable(this)
        .Subscribe(x => Spawn());
    }

    public void Spawn()
    {
        for (int i = 0; i < perSpawn; i++)
        {
            var randX = transform.position.x + UnityEngine.Random.value - area.x / 2;
            var randZ = transform.position.z + UnityEngine.Random.value - area.y / 2;
            var spawnPos = new Vector3(randX, transform.position.y, randZ);
            Enemy enemy = pool.Spawn();
            enemy.transform.position = spawnPos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawCube(transform.position, new Vector3(area.x, 1, area.y));
    }
}
