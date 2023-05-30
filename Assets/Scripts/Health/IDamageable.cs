using System;

public interface IDamageable
{
    void RecieveDamage(int damage);
    IObservable<int> HPChanged { get; }
    int MaxHP { get; }
}
