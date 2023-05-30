using UnityEngine;
using UniRx;
using System;

public class LootScanner : MonoBehaviour
{
    private Subject<ILoot> picked = new Subject<ILoot>();
    public IObservable<ILoot> Picked => picked;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ILoot>(out var loot))
        {
            picked.OnNext(loot);
            loot.Pick();
        }
    }
}
