using UnityEngine;
using Zenject;

// TODO: здесь должно быть разделение на тип лута. В простом случае можно и enum сделать, зависит от поведения лута
public class Loot : MonoBehaviour, ILoot
{
    [SerializeField]
    private int count;

    private Loot.Pool pool;
    private Rigidbody lootRigidbody;

    public int Count => count;

    void Awake()
    {
        lootRigidbody = GetComponent<Rigidbody>();
    }

    [Inject]
    public void Construct(Loot.Pool pool)
    {
        this.pool = pool;
    }

    public void Pick()
    {
        gameObject.SetActive(false);
        pool.Despawn(this);
    }

    public class Pool : MemoryPool<Loot>
    {
        protected override void OnDespawned(Loot item)
        {
            base.OnDespawned(item);
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(Loot item)
        {
            base.OnSpawned(item);
            item.gameObject.SetActive(true);
        }

        protected override void Reinitialize(Loot item)
        {
            base.Reinitialize(item);
            item.lootRigidbody.velocity = Vector3.zero;
        }
    }
}
