using UnityEngine;
using Zenject;

public class LootDrop : MonoBehaviour
{
    [SerializeField]
    LootDropConfig[] loot;
    
    private Loot.Pool pool;

    [Inject]
    public void Construct(Loot.Pool pool)
    {
        this.pool = pool;
    }

    public void Drop()
    {
        Vector3 targetPosition = transform.position;
        foreach (var item in loot)
        {
            if (Random.value <= item.dropChance)
            {
                for (int i = 0; i < item.count; i++)
                {
                    float randomAngle = Random.Range(0f, Mathf.PI * 2f);
                    Vector3 spawnPosition = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * 1;
                    spawnPosition += targetPosition;
                    var loot = pool.Spawn();
                    loot.transform.position = spawnPosition;
                }
            }
        }
    }
}
