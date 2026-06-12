using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary> MenuでStatusを制御するクラス </summary>
public class MenuStatus : MonoBehaviour
{
    /// <summary> Pointの表示限界 </summary>
    int POINTMAX => 999999;
    /// <summary> Statusの表示限界 </summary>
    int STATUSMAX => 99999;

    /// <summary> 変わる事のないNameText </summary>
    private TextMeshProUGUI T_Name;
    /// <summary> まれに変わるLevelText </summary>
    private TextMeshProUGUI T_Level;
    private List<List<TextMeshProUGUI>> V_Text = new List<List<TextMeshProUGUI>>();

    /// <summary> 値変更Button(arrow)の押下状態 </summary>
    bool Change = false;

    /// <summary> MenuのStatusで使用するText等を登録 </summary>
    public void Setup() {
        T_Name = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        T_Level = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < 2; i++) {       //変更前Text・変更後Text
            var valueBox = transform.GetChild(i);
            V_Text.Add(new List<TextMeshProUGUI>());
            for (int j = 0; j < valueBox.childCount; j++) {
                V_Text[i].Add(valueBox.GetChild(j).GetComponent<TextMeshProUGUI>());
            }
        }
        GetStatus();
    }

    /// <summary> ステータスを数値を入れる </summary>
    void GetStatus() {
        var status = GameManager.MyGameInstance.GetMyStatus();
        T_Name.text = status.Nama;
        T_Level.text = status.Lvl.ToString();

        for (int i = 0; i < V_Text[0].Count; i++) {
            var value = i switch {
                0 => status.Pit.ToString(),
                1 => status.Hp.ToString(),
                2 => status.Atk.ToString(),
                3 => status.Def.ToString(),
                4 => status.Spd.ToString(),
                5 => status.Jpw.ToString(),
                _ => "Miss! Error!"
            };
            V_Text[0][i].text = value;
            V_Text[1][i].text = value;
        }
    }

    /// <summary> ボタンを押下 </summary>
    /// <param name="no"></param>
    public void PressButton(int no) {
        Change = true;
        ChangeValue(no).Forget();
    }

    /// <summary> ボタンを離した・離れた </summary>
    public void PullButton() { Change = false; }

    /// <summary> 長押しの判断をする関数 </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    async UniTask ChangeValue(int no) {
        ChangeStatus(no);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));    //0.2秒押下したら長押し判定
        while (Change) {
            ChangeStatus(no);
            await UniTask.Delay(100);
        }
        
    }

    /// <summary> 数値を変更する関数 </summary>
    /// <param name="no"></param>
    private void ChangeStatus(int no) {
        bool ud = no % 2 == 0;      //結果が 0 => 増加 ・ 1 => 減少 させる
        int point = int.Parse(V_Text[1][0].text);

        no /= 2;                    //数値によって何を増減させるか判断する
        int status = int.Parse(V_Text[1][no + 1].text);

        if (ud) {   //増加
            if(--point < 0 || ++status > STATUSMAX) {       //Statusが最大に・Pointが最低になったら中止
                Change = false;
                return;
            }
        }
        else {      //増加
            if (--status < 1 || ++point > POINTMAX) {       //Statusが最低に・Pointが最高になったら中止
                Change = false;
                return;   
            }
        }
        //仮登録
        V_Text[1][0].text = point.ToString();
        V_Text[1][no + 1].text = status.ToString();
    }

    /// <summary> Statusの変更を反映する </summary>
    public void SetStatus() {
        var s_Box= new StatusBox();
        s_Box.Pit = int.Parse(V_Text[1][0].text);
        s_Box.Hp  = int.Parse(V_Text[1][1].text);
        s_Box.Atk = int.Parse(V_Text[1][2].text);
        s_Box.Def = int.Parse(V_Text[1][3].text);
        s_Box.Spd = int.Parse(V_Text[1][4].text);
        s_Box.Jpw = int.Parse(V_Text[1][5].text);
        GameManager.MyGameInstance.SetStatus(s_Box);
        GetStatus();
    }
}