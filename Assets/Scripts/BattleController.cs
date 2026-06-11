using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    [SerializeField] bool Battle = true;
    public int EnemyCount = 0;
    List<LiveTemp> Live = new List<LiveTemp>();
    bool Give = false;
    Animator Anima;

    // Awake is called once before the first execution of Update after the MonoBehaviour is createdvoid
    private void Start()
    {
        Anima = GetComponent<Animator>();
        var text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = Battle ? "Give Up ?" : "Return ?";
        text.color = Battle ? Color.yellow : Color.white;
        transform.GetChild(0).GetChild(0).localScale = Vector3.zero;

        SetStatus();

        if (Battle)
        {
            LifeGauge gauge = FindAnyObjectByType<LifeGauge>();
            gauge.SetUp();
        }

        StartStandby().Forget();
    }
    
    /// <summary> 全体にステータスを割り振る </summary>
    void SetStatus() {
        LiveTemp[] live = FindObjectsByType<LiveTemp>(FindObjectsSortMode.None);
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
                    EnemyCount++;
                    break;
            }
            Live.Add(l);
        }
    }


    async UniTask StartStandby()
    {
        await UniTask.Yield();
        GameManager.MyGameInstance.LoadPanel(false);
        if (Battle)
        {

        }

        foreach (var l in Live)
            l.ModeType = ModeTypeList.Default;
    }


    void OnGiveUp(InputValue value)
    {
        Anima.Play((Give = !Give) ? "Give01" : "Give02");
    }

    void OnReturn(InputValue value)
    {
        if (!Give) return;
        Anima.Play("Give02");
        MenuReturn(true);
    }

    void OnBack(InputValue value)
    {
        if (!Give) return;
        Give = false;
        Anima.Play("Give02");
    }

    /// <summary>
    /// Enemyを倒したら呼び出す
    /// </summary>
    public void KillEnemy()
    {
        if (--EnemyCount <= 0) MenuReturn();
    }

    void MenuReturn(bool Give = false)
    {
        GetComponent<PlayerInput>().enabled = false;
        if (Battle)
        {
            Debug.Log("gagaga");
        }
        if (!Battle || EnemyCount == 0) GameManager.MyGameInstance.SetClearStage();
        GameManager.MyGameInstance.LoadPanel(true);
        GameManager.MyGameInstance.LoadScene("Menu");
    }
}