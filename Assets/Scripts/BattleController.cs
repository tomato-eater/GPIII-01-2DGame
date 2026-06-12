using Cysharp.Threading.Tasks;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    [SerializeField] bool Battle = true;
    public int EnemyCount = 0;
    [SerializeField] List<LiveTemp> Live = new List<LiveTemp>();
    bool Give = false;
    Animator Anima;
    float StartFade = 2f;

    // Awake is called once before the first execution of Update after the MonoBehaviour is createdvoid
    private void Start()
    {
        Anima = GetComponent<Animator>();
        var text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = Battle ? "Give Up ?" : "Return ?";
        text.color = Battle ? Color.yellow : Color.white;
        transform.GetChild(0).GetChild(0).localScale = Vector3.zero;

        SetStatus();

        var frontImage = transform.GetChild(0).Find("FrontImage").GetComponent<Image>();
        frontImage.gameObject.SetActive(true);
        frontImage.transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).Find("FinishText").gameObject.SetActive(false);

        if (Battle)
        {
            LifeGauge gauge = FindAnyObjectByType<LifeGauge>();
            gauge.SetUp();

            var color = frontImage.color;
            color = new Color32(50, 50, 50, 255);
        }
        else
        {
            frontImage.gameObject.SetActive(false);
        }

        StartStandby(frontImage).Forget();
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


    async UniTask StartStandby(Image image)
    {
        var input = GetComponent<PlayerInput>();
        input.enabled = false;
        await UniTask.Yield();
        GameManager.MyGameInstance.LoadPanel(false);
        if (Battle)
        {
            Color color = image.color;
            float rest = StartFade;
            while (rest > 0f)
            {
                color.a = rest / StartFade;
                image.color = color;
                await UniTask.Yield();
                rest -= Time.deltaTime;
            }

            image.transform.GetChild(0).gameObject.SetActive(true);
            var text = image.GetComponentInChildren<TextMeshProUGUI>();
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            text.text = "Ready ?";
            for(int i = 0; i < 3; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
                text.text += " .";
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            text.text = "Fight !";
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            image.gameObject.SetActive(false);
        }

        foreach (var l in Live)
            l.ModeType = ModeTypeList.Default;

        input.enabled = true;
    }


void OnGiveUp(InputValue value)
    {
        Anima.Play((Give = !Give) ? "Give01" : "Give02");
        Debug.Log("gagaga");
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
        if (--EnemyCount <= 0) MenuReturn().Forget();
    }

    async UniTask MenuReturn(bool Give = false)
    {
        GetComponent<PlayerInput>().enabled = false;
        foreach (var l in Live)
            if (l != null) l.ModeType = ModeTypeList.Finish;
        if (Battle)
        {
            var textT = transform.GetChild(0).Find("FinishText");

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            textT.gameObject.SetActive(true);
            Vector3 scale = new Vector3(0, 1, 1);
            while (scale.x < 1.5f)
            {
                textT.localScale = scale;
                await UniTask.Yield();
                scale.x += Time.deltaTime;
            }
            while (scale.x > 1f)
            {
                textT.localScale = scale;
                await UniTask.Yield();
                scale.x -= Time.deltaTime;
                if (scale.x <= 1f) scale.x = 1f;
            }
            textT.localScale = scale;
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

        }
        if (!Battle || EnemyCount == 0) GameManager.MyGameInstance.SetClearStage();
        GameManager.MyGameInstance.LoadPanel(true);
        GameManager.MyGameInstance.LoadScene("Menu");
    }
}