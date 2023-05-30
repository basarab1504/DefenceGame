using UniRx;

public interface ICharacter
{
    Subject<CharacterState> State { get; }
}