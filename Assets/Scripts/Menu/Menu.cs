using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Menuを制御するクラス </summary>
public class Menu : MonoBehaviour
{
    /// <summary> 左のButton </summary>
    private List<Button> L_Buttons = new List<Button>();
    /// <summary> 左のButtonの押下番号 </summary>
    int SelectButton = -1;

    /// <summary> 真ん中のObj(半透明) </summary>
    GameObject Front;
    /// <summary> 真ん中のObj(Main) </summary>
    private List<GameObject> M_Obj=new List<GameObject>();

    /// <summary> 右のObj </summary>
    private GameObject R_Obj;
    ///<summary> 右のButtonのText </summary>
    private TextMeshProUGUI R_Text;

    /// <summary> MenuでStatusを制御するクラス </summary>
    [SerializeField] MenuStatus M_Status;
    /// <summary> MenuでBattleを選択するクラス </summary>
    [SerializeField] MenuBattle M_Battle;

    //実行トリガー
    bool Save = true;
    bool Title = false;
    bool Quit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        M_Status.Setup();
        M_Battle.SetUp();

        var box = transform.Find("L_Canvas/L_Image");
        for(int i = 1; i < box.childCount; i++) {
            L_Buttons.Add(box.GetChild(i).GetComponent<Button>());
        }

        Front = transform.Find("M_Image/FrontImage").gameObject;
        for(int i = 0; i < Front.transform.childCount; i++) {
            M_Obj.Add(Front.transform.GetChild(i).gameObject);
            M_Obj[i].SetActive(false);
        }
        Front.SetActive(false);

        box = transform.Find("R_Image");
        R_Obj = box.GetChild(0).gameObject;
        R_Obj.SetActive(false);

        R_Text = R_Obj.GetComponentInChildren<TextMeshProUGUI>();

        box = transform.Find("M_Image/FrontImage/Exit");
        ExitText(box.transform.Find("SaveButton").GetComponentInChildren<TextMeshProUGUI>(), Save);
        ExitText(box.transform.Find("TitleButton").GetComponentInChildren<TextMeshProUGUI>(), Title);
        ExitText(box.transform.Find("QuitButton").GetComponentInChildren<TextMeshProUGUI>(), Quit);

        GameManager.MyGameInstance.LoadPanel(false);
    }

    /// <summary> Menuで左側のButtonが押下された </summary>
    /// <param name="no"></param>
    public void L_ButtonTrigger(int no)
    {
        Front.SetActive(true);
        R_Obj.SetActive(true);
        for(int i = 0; i < 2; i++) {    //押下滴やつ・押下したやつ
            if (SelectButton >= 0) {    //一回目なら無視
                L_Buttons[SelectButton].transform.Rotate(0, i == 0 ? -10 : 10, 0);
                M_Obj[SelectButton].SetActive(i == 1);
            }
            SelectButton = no;
        }

        R_Text.text = SelectButton switch {     //右側のButtonのTextを変える
            0 => "Press Allocation",
            1 => "Press Battle",
            2 => "Press Execute",
            _ => "Miss! Error"
        };
    }

    /// <summary> 実行トリガーで判断し、ButtonのTextを変える </summary>
    /// <param name="text"></param>
    /// <param name="trigger"></param>
    void ExitText(TextMeshProUGUI text, bool trigger) { text.text = trigger ? "Yes" : "No"; }

    /// <summary> 実行トリガーの反転 </summary>
    /// <param name="no"></param>
    public void ExitButton(int no) {
        var box = transform.Find("M_Image/FrontImage/Exit/" + (no == 0 ? "SaveButton" : no == 1 ? "TitleButton" : "QuitButton"));
        ExitText(box.GetComponentInChildren<TextMeshProUGUI>(), no == 0 ? Save = !Save : no == 1 ? Title = !Title : Quit = !Quit);
    }

    /// <summary> 左側Buttonに基づいて実行 </summary>
    public void R_ButtonTrigger() {
        switch (SelectButton) {
            case 0:     //Status
                M_Status.SetStatus();
                break;

            case 1:     //Battle
                M_Battle.StartBattle();
                break;

            case 2:     //Exit
                if (Save) GameManager.MyGameInstance.SaveData();
                if (Quit) {
                    GameManager.MyGameInstance.Quit();
                    return;
                }
                if (Title) {
                    GameManager.MyGameInstance.LoadPanel(true);
                    GameManager.MyGameInstance.LoadScene();
                }
                break;

            default:    //異常値
                Debug.LogError("Miss_Select");
                break;
        }
    }
}