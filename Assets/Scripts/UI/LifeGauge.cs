using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using R3.Triggers;


public class LifeGauge : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Image gaugeImage;
    [SerializeField] TextMeshProUGUI lifeText;
    float MaxHp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaxHp = player.Hp.Value;
        player.Hp.Subscribe(Hp => UpGauge(Hp)).AddTo(this);
    }

    void UpGauge(float hp)
    {
        if (hp < 0) hp = 0;
        lifeText.text = hp.ToString("0");
        float ratio = hp / MaxHp;

        gaugeImage.fillAmount = ratio;
    }
}
