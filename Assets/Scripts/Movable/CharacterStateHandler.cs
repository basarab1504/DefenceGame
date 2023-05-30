using UniRx;
using UnityEngine;

public class CharacterStateHandler : MonoBehaviour
{
    private ICharacter character;
    private Animator animator;
    private CompositeDisposable disposables = new CompositeDisposable();

    void Awake()
    {
        character = GetComponent<ICharacter>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        character.State
        .Subscribe(HandleState)
        .AddTo(disposables);
    }

    void OnDisable()
    {
        disposables.Clear();
        animator.SetTrigger("DynIdle");
    }

    void HandleState(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Idle:
                animator.SetTrigger("DynIdle");
                break;
            case CharacterState.Running:
                animator.SetTrigger("Running");
                break;
            case CharacterState.Attack:
                animator.SetTrigger("Throw");
                break;
        }
    }
}
