using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Menuを制御するクラス </summary>
public class Menu : MonoBehaviour
{
    /// <summary> 左のButton </summary>
    private List<Button> L_Buttons = new List<Button>();
    int SelectButton = -1;

    GameObject Front;
    /// <summary> 真ん中のObj </summary>
    private List<GameObject> M_Obj=new List<GameObject>();

    /// <summary> 右のObj </summary>
    private GameObject R_Obj;
    ///<summary> 右のButtonのText </summary>
    private TextMeshProUGUI R_Text;

    [SerializeField] MenuStatus M_Status;
    [SerializeField] MenuBattle M_Battle;

    bool Save = true;
    bool Title = false;
    bool Quit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        M_Status.Setup();
        M_Battle.SetUp();

        var box = transform.Find("L_Canvas/L_Image");
        for(int i = 1; i < box.childCount; i++)
        {
            L_Buttons.Add(box.GetChild(i).GetComponent<Button>());
        }

        Front = transform.Find("M_Image/FrontImage").gameObject;
        for(int i = 0; i < Front.transform.childCount; i++)
        {
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

    public void L_ButtonTrigger(int no)
    {
        Front.SetActive(true);
        R_Obj.SetActive(true);
        for(int i = 0; i < 2; i++)
        {
            if (SelectButton >= 0)
            {
                L_Buttons[SelectButton].transform.Rotate(0, i == 0 ? -10 : 10, 0);
                M_Obj[SelectButton].SetActive(i == 1);
            }
            SelectButton = no;
        }

        R_Text.text = SelectButton switch
        {
            0 => "Press Allocation",
            1 => "Press Battle",
            2 => "Press Execute",
            _ => "Miss! Error"
        };
    }

    void ExitText(TextMeshProUGUI text, bool trigger)
    {
        text.text = trigger ? "Yes" : "No";
    }

    public void ExitButton(int no)
    {
        var box = transform.Find("M_Image/FrontImage/Exit/" + (no == 0 ? "SaveButton" : no == 1 ? "TitleButton" : "QuitButton"));
        ExitText(box.GetComponentInChildren<TextMeshProUGUI>(), no == 0 ? Save = !Save : no == 1 ? Title = !Title : Quit = !Quit);
    }


    public void R_ButtonTrigger()
    {
        switch (SelectButton)
        {
            case 0:
                M_Status.SetStatus();
                break;

            case 1:
                GameManager.MyGameInstance.LoadPanel(true);
                GameManager.MyGameInstance.LoadScene(M_Battle.GetSceneName());
                break;

            case 2:
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

            default:
                Debug.LogError("Miss_Select");
                break;
        }
    }
}
