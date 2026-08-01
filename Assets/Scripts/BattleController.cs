using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary> Battleの進行を制御するクラス </summary>
public class BattleController : MonoBehaviour
{
    /// <summary> 訓練所か等、判断 </summary>
    [SerializeField] bool Battle;
    /// <summary> 敵の数 </summary>
    public int EnemyCount = 0;
    /// <summary> Player・Enemy のクラス保管 </summary>
    List<LiveTemp> Live = new List<LiveTemp>();
    bool Give = false;
    Animator Anima;
    [SerializeField] TextMeshProUGUI StartText;
    [SerializeField] TextMeshProUGUI JudgeText;
    bool LevelUp;

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
        var input = GetComponent<PlayerInput>();
        input.enabled = false;
        await UniTask.Yield();
        GameManager.MyGameInstance.LoadPanel(false);
        if (Battle)
        {
            Anima.Play("Start");
        }
        else
            StartBattle();

    }

    public void AddDot()
    {
        StartText.text += " .";
    }

    public void ChangeText()
    {
        StartText.text = "FIGHT !!";
    }

    public void StartBattle()
    {
        foreach (var l in Live)
            l.ModeType = ModeTypeList.Default;

        var input = GetComponent<PlayerInput>();
        input.enabled = true;
    }


    void OnGiveUp(InputValue value)
    {
        Anima.Play((Give = !Give) ? "Give01" : "Give02");
    }

    void OnReturn(InputValue value)
    {
        if (!Give) return;
        Anima.Play("Give02");
        FinishBattle(true);
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
        if (--EnemyCount <= 0) FinishBattle();
    }

    public void FinishBattle(bool Give = false)
    {
        GetComponent<PlayerInput>().enabled = false;
        foreach (var l in Live)
            if (l != null) l.ModeType = ModeTypeList.Finish;
        if (Battle)
        {
            Anima.Play("Finish");

        }
        else 
        { 
            GameManager.MyGameInstance.SetClearStage();
            ReturnMenu();
        }

    }

    public void Judge()
    {
        var player = Live.Find(p => p.gameObject.name == "Player");
        if (player == null) Debug.LogError("Playerが存在しません");
        if (player.Hp.Value > 0)
        {
            if (Give)
            {
                JudgeText.text = "GIVE";
                JudgeText.color = Color.yellow;
                LevelUp = false;
            }
            else
            {

                JudgeText.text = "WIN";
                JudgeText.color = Color.red;
                LevelUp = !GameManager.MyGameInstance.GetClearStage();
                GameManager.MyGameInstance.SetClearStage();
            }
        }
        else
        {
            JudgeText.text = "LOSE";
            JudgeText.color = Color.blue;
            LevelUp = false;
        }

    }

    public void CheckLevelUp()
    {
        if (LevelUp)
        {
            Anima.Play("LevelUp");
            GameManager.MyGameInstance.LevelUp();
        }
        else
        {
           ReturnMenu();
        }
    }

    public void ReturnMenu()
    {
        GameManager.MyGameInstance.LoadPanel(true);
        GameManager.MyGameInstance.LoadScene("Menu");
    }
}