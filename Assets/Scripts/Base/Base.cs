using UniRx;
using UnityEngine;

// todo: наверное, имея разные типы лута, можно как нибудь тут его разбирать и что-то с ним делать
public class Base : MonoBehaviour, IBase
{
    private int collectedLoot;

    public void CollectLoot(ILoot loot)
    {
        collectedLoot += loot.Count;
        MessageBroker.Default.Publish(new BaseCollectLootEvent()
        {
            count = collectedLoot
        });
    }
}
