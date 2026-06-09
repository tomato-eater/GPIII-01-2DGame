using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;

/// <summary> Player‚ÌHPBar‚ğ§Œä‚·‚éƒNƒ‰ƒX </summary>
public class LifeGauge : MonoBehaviour
{
    LiveTemp Player;
    [SerializeField] Image gaugeImage;
    [SerializeField] TextMeshProUGUI Name;
    float MaxHp;

    /// <summary>
    /// Player‚ÌName_Hp‚ğ“o˜^
    /// </summary>
    public void SetUp()
    {
        this.Player = GameObject.Find("Player").GetComponent<LiveTemp>();
        Name.text = GameManager.MyGameInstance.GetMyStatus().Nama;
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
