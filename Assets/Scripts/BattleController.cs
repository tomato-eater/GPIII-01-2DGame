using Cysharp.Threading.Tasks;
using R3.Collections;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    int liveCount;

    // Awake is called once before the first execution of Update after the MonoBehaviour is createdvoid
    private void Start()
    {
        SetStatus();
        SetHpGauge();
        GameManager.MyGameInstance.LoadPanel(false);

        StartStandby().Forget();
    }
    
    /// <summary> 全体にステータスを割り振る </summary>
    void SetStatus() {
        LiveTemp[] live = FindObjectsByType<LiveTemp>(FindObjectsSortMode.None);
        liveCount = live.Length;
        foreach (var l in live) {
            switch (l.GetId()) {
                //ステータスを必要としない者
                case -1: continue;

                //Player
                case 0: 
                    l.SetStatus(GameManager.MyGameInstance.GetMyStatus());
                    break;

                //その他、敵
                default:
                    l.SetStatus(GameManager.MyGameInstance.GetEnStatus(l.GetId()));
                    break;
            }
        }
    }
    /// <summary> PlayerのHpGaugeの設定 </summary>
    void SetHpGauge()
    {
        LifeGauge gauge = FindAnyObjectByType<LifeGauge>();
        gauge.SetUp();
    }

    async UniTask StartStandby()
    {

    }
}