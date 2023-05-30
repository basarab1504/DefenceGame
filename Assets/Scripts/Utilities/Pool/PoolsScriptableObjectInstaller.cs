using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PoolConfigSO", menuName = "ScriptableObjects/PoolConfigSO", order = 1)]
public class PoolsScriptableObjectInstaller : ScriptableObjectInstaller
{
    [SerializeField]
    private Enemy enemy;
    [SerializeField]
    private Bullet bullet;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Loot loot;

    public override void InstallBindings()
    {
        Container.BindMemoryPool<Enemy, Enemy.Pool>().FromComponentInNewPrefab(enemy);
        Container.BindMemoryPool<Bullet, Bullet.Pool>().FromComponentInNewPrefab(bullet);
        Container.BindMemoryPool<Player, Player.Pool>().FromComponentInNewPrefab(player);
        Container.BindMemoryPool<Loot, Loot.Pool>().FromComponentInNewPrefab(loot);
    }
}
