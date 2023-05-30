using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI score;
    [SerializeField]
    private Slider healthbar;

    private CompositeDisposable disposables = new CompositeDisposable();

    void OnEnable()
    {
        Player.PlayerInstance
        .Where(x => x != null)
        .Subscribe(player =>
        {
            player.HPChanged
            .Select(x => (x, player.MaxHP))
            .Subscribe(OnHPChanged).AddTo(disposables);
        }).AddTo(disposables);


        MessageBroker.Default.Receive<BaseCollectLootEvent>().Subscribe(e =>
        {
            OnScoreChanged(e.count);
        }).AddTo(disposables);
    }

    void OnDisable()
    {
        disposables.Clear();
    }

    public void SetState(bool state)
    {
        transform.GetChild(0).gameObject.SetActive(state);
    }

    private void OnHPChanged((int value, int maxValue) hp)
    {
        var normalized = (float)hp.value / (float)hp.maxValue;
        healthbar.value = normalized;
    }

    private void OnScoreChanged(int value)
    {
        score.text = value.ToString();
    }
}
