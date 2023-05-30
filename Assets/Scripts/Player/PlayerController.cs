using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float acceleration;

    private Rigidbody playerRigidbody;
    private ICharacter character;
    private PlayerInput playerInput;

    private Vector3 directionNormalized = new Vector3();

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        character = GetComponent<ICharacter>();
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        Observable.EveryFixedUpdate()
        .TakeUntilDisable(this)
        .Subscribe(HandleMovement);

        playerInput.currentActionMap.FindAction("Move", true).AsObservable()
        .Select(action => action.phase)
        .DistinctUntilChanged()
        .TakeUntilDisable(this)
        .Subscribe(HandlePhaseChange);

        playerInput.currentActionMap.FindAction("Move", true).AsObservable()
        .Select(action => action.phase.IsInProgress() ? action.ReadValue<Vector2>() : Vector2.zero)
        .TakeUntilDisable(this)
        .Subscribe(Move);
    }

    void OnDisable()
    {
        playerRigidbody.velocity = Vector3.zero;
    }

    private void Move(Vector2 vec)
    {
        directionNormalized = new Vector3(vec.x, 0, vec.y).normalized;
        if (directionNormalized != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionNormalized.normalized, transform.up);
        }
    }

    void HandlePhaseChange(InputActionPhase phase)
    {
        switch (phase)
        {
            case InputActionPhase.Performed:
                character.State.OnNext(CharacterState.Running);
                break;
            case InputActionPhase.Canceled:
                character.State.OnNext(CharacterState.Idle);
                break;
        }
    }

    void HandleMovement(long dt)
    {
        playerRigidbody.position += directionNormalized * Time.fixedDeltaTime * acceleration;
    }
}
