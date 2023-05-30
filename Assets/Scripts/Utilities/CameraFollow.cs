using UniRx;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset = new Vector3(0, 2, -10);

    void OnEnable()
    {
        Observable.EveryUpdate()
        .CombineLatest(Player.PlayerInstance, (l, r) => r)
        .Select(r => r.transform)
        .TakeUntilDisable(this)
        .Subscribe(HandleFollowing);
    }

    void HandleFollowing(Transform playerTfm)
    {
        transform.position = playerTfm.position + offset;
        transform.LookAt(playerTfm);
    }
}
