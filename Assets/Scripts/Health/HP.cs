using UnityEngine;
using UniRx;
using System;

public class HP : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxValue;

    private ReactiveProperty<int> Health;
    public IObservable<int> HPChanged => Health;
    public int MaxHP => maxValue;

    void Awake()
    {
        Health = new ReactiveProperty<int>(MaxHP);
    }

    void OnEnable()
    {
        Health.Value = MaxHP;
    }

    public void RecieveDamage(int damage)
    {
        Health.Value = Mathf.Max(0, Health.Value - damage);
    }
}
