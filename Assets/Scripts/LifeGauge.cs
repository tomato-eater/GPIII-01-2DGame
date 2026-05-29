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
    [SerializeField] TextMeshProUGUI maxLifeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxLifeText.text = player.Hp.Value.ToString();
        player.Hp.Subscribe(Hp => lifeText.text = Hp.ToString()).AddTo(this);
        player.Hp.Select(Hp => Hp / float.Parse(maxLifeText.text)).Subscribe(Hp => gaugeImage.fillAmount = Hp).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
