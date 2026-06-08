using UnityEngine;
using UnityEngine.UI;
using R3;

/// <summary> Player‚ÌHPBar‚ğ§Œä‚·‚éƒNƒ‰ƒX </summary>
public class LifeGauge : MonoBehaviour
{
    LiveTemp Player;
    [SerializeField] Image gaugeImage;
    float MaxHp;

    /// <summary>
    /// Player‚ÌHp‚É‰‚¶‚Ä•Ï‰»‚·‚é‚æ‚¤‚É‚·‚é
    /// </summary>
    public void SetUp()
    {
        this.Player = GameObject.Find("Player").GetComponent<LiveTemp>();
        MaxHp = Player.Hp.Value;
        Player.Hp.Subscribe(Hp => UpGauge(Hp)).AddTo(this);
    }
    /// <summary>
    /// Gauge‚Ì•Ï‰»ˆ—
    /// </summary>
    /// <param name="hp"></param>
    void UpGauge(float hp)
    {
        if (hp < 0) hp = 0;
        float ratio = hp / MaxHp;

        gaugeImage.fillAmount = ratio;
    }
}
