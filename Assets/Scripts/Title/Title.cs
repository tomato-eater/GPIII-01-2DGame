using NUnit.Framework;
using R3;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトルを制御するクラス
/// </summary>
public class Title : MonoBehaviour
{
    /// <summary>
    /// 画面2のObject
    /// </summary>
    [SerializeField] GameObject SecondImage;

    /// <summary>
    /// 画面1のボタン達
    /// </summary>
    List<Button> F_Button = new List<Button>();

    /// <summary>
    /// 画面2のボタン達
    /// </summary>
    List<Button> S_Button = new List<Button>();

    /// <summary>
    /// アニメーター
    /// </summary>
    [SerializeField] Animator Anima;

    /// <summary>
    /// ボタンの番号を格納する変数
    /// </summary>
    int Mode = 0;

    /// <summary>
    /// 右側のやつ
    /// </summary>
    Transform S_Box;

    /// <summary>
    /// データ番号を格納する変数
    /// </summary>
    int selectNo = -1;

    [SerializeField] StatusDataList statusDataList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var FirstImage = transform.Find("FirstImage").gameObject;
        SecondImage = SecondImage == null ? transform.Find("SecondImage").gameObject : SecondImage;

        var list = FirstImage.transform.Find("ButtonList");
        for (int i = 0; i < list.childCount; i++) 
            F_Button.Add(list.GetChild(i).GetComponent<Button>());

        list = SecondImage.transform.Find("ButtonList");
        for (int i = 0; i < list.childCount; i++)
        {
            S_Button.Add(list.GetChild(i).GetComponent<Button>());
            S_Button[i].interactable = false;
        }

        SecondImage.transform.Find("SelectedData").gameObject.SetActive(false);
        S_Box = SecondImage.transform.Find("Box");

        GetData();
    }

    /// <summary>
    /// セーブデータの情報を取得する関数
    /// </summary>
    void GetData()
    {
        var list = S_Box.Find("DataList/Viewport/Content");
        var folder = Path.Combine(Application.streamingAssetsPath, "SaveData");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        for (int i = 0; i < 6; i++)
        {
            if (!File.Exists(Path.Combine(folder, "Data_" + i + ".txt")))
            {
                File.Create(Path.Combine(folder, "Data_" + i + ".txt")).Close();
            }
            string text = File.ReadAllLines(Path.Combine(folder, "Data_" + i + ".txt")).ElementAtOrDefault(0) ?? "";
            Debug.Log(text);
            if (text == "")
            {
                list.GetChild(i).Find("List/NameText").GetComponent<TextMeshProUGUI>().text = "";
                list.GetChild(i).Find("List").gameObject.SetActive(false);
                continue;
            }
            list.GetChild(i).Find("List/NameText").GetComponent<TextMeshProUGUI>().text = "Name : " + text.Substring(5);
        }
    }

    /// <summary>
    /// いずれかのボタンを押下
    /// </summary>
    /// <param name="no"></param>
    public void ButtonTrigger(int no)
    {
        bool title = no < 1;
        foreach (var button in F_Button)
            button.interactable = title;
        foreach (var button in S_Button)
            button.interactable = !title;

        Mode = no;

        switch(no)
        {
            case -1:
            case 0:
            case 1:
                Anima.Play("TitleFront2");
                return;

            default:
                for (int i = 0; i < S_Box.childCount; i++)
                    S_Box.GetChild(i).gameObject.SetActive(i == 1); 
                break;
        }

        var text = S_Box.transform.Find("DataList/TopText").GetComponent<TextMeshProUGUI>();
        text.text = Mode switch
        {
            2 => "Select Save File",
            3 => "Select Load File",
            4 => "Select Delete File",
            _ => "Miss!! Error!! "
        };
        text.color = Mode switch
        {
            2 => new Color32(202, 255, 196, 255),
            3 => new Color32(255, 239, 153, 255),
            4 => new Color32(255, 172, 172, 255),
            _ => Color.black
        };

        for (int i = 0; i < 6; i++)
        {
            var button = S_Box.Find("DataList/Viewport/Content").GetChild(i).GetComponent<Button>();
            button.interactable = Mode switch
            {
                2 => true,
                3 => button.gameObject.transform.Find("List").gameObject.activeSelf,
                4 => button.gameObject.transform.Find("List").gameObject.activeSelf,
                _ => false
            };
        }
    }

    /// <summary>
    /// フェードイン・アウトの完了を検知
    /// </summary>
    public void Fade()
    {
        switch(Mode)
        {
            case -1:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit()
#endif
                break;

            case 0:
                SecondImage.SetActive(false);
                Anima.Play("TitleFront1");
                break;
            case 1:
                SecondImage.SetActive(true);
                for (int i = 0; i < S_Box.childCount; i++)
                    S_Box.GetChild(i).gameObject.SetActive(i == 0);
                S_Box.Find("DataList/Viewport/Content").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                Anima.Play("TitleFront1");
                break;
            default:
                Debug.Log("Game Start");
                break;
        }
    }

    /// <summary>
    /// データのボタンを押下
    /// </summary>
    /// <param name="no"></param>
    public void DataButton(int no)
    {
        selectNo = no;

        var top = SecondImage.transform.Find("SelectedData");
        top.gameObject.SetActive(true);

        var text = top.transform.Find("TopText").GetComponent<TextMeshProUGUI>();
        text.text = Mode switch
        {
            2 => "Create Data",
            3 => "Load Data",
            4 => "Delete Data",
            _ => "Miss!! Error!! "
        };
        text.color = Mode switch
        {
            2 => new Color32(202, 255, 196, 255),
            3 => new Color32(255, 239, 153, 255),
            4 => new Color32(255, 172, 172, 255),
            _ => Color.black
        };

        text = top.transform.Find("ExpText").GetComponent<TextMeshProUGUI>();
        text.text = Mode switch
        {
            2 => "Input Your Name",
            3 => "Load Data Name",
            4 => "Delete Data Name",
            _ => "Miss!! Error!! "
        };

        var inputText = top.transform.Find("InputField").GetComponent<TMP_InputField>();
        if (Mode == 2)
        {
            inputText.text = "";
            inputText.interactable = true;
        }
        else
        {
            var list = S_Box.Find("DataList/Viewport/Content");
            var name = list.GetChild(no).Find("List/NameText").GetComponent<TextMeshProUGUI>().text;
            inputText.text = name.Substring(7);
            inputText.interactable = false;
        }

        top = top.Find("Image");
        for (int i = 0; i < top.childCount; i++)
        {
            top.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = Mode switch
            {
                2 => i == 0 ? "Cancel" : "Create",
                3 => i == 0 ? "Cancel" : "Load",
                4 => i == 0 ? "Delete" : "Cancel",
                _ => "Miss!! Error!! "
            };
            top.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// 最期のボタンを押下
    /// </summary>
    /// <param name="start"></param>
    public void FinalButton(bool start)
    {
        //連打防止
        var image = SecondImage.transform.Find("SelectedData/Image");
        for (int i = 0; i < image.childCount; i++)
            image.GetChild(i).GetComponent<Button>().interactable = false;

        var folder = Path.Combine(Application.streamingAssetsPath, "SaveData");
        string fail = "Data_" + selectNo + ".txt";
        switch (Mode)
        {
            //ニューゲームタブ
            case 2:
                if (!start)
                { 
                    SecondImage.transform.Find("SelectedData").gameObject.SetActive(false);
                    return;
                }

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                var data = statusDataList.statusDataList[0].GetStatus();
                using (StreamWriter sw = new StreamWriter(Path.Combine(folder, fail), false))
                {
                    sw.WriteLine("Name:" + SecondImage.transform.Find("SelectedData/InputField").GetComponent<TMP_InputField>().text);
                    sw.WriteLine("Lvl:" + data.lvl);
                    sw.WriteLine("HP :" + data.hp);
                    sw.WriteLine("ATK:" + data.atk);
                    sw.WriteLine("DEF:" + data.def);
                    sw.WriteLine("SPD:" + data.spd);
                    sw.WriteLine("JPW:" + data.jpw);
                }
                break;
            //ロードタブ
            case 3:
                if (!start)
                {
                    SecondImage.transform.Find("SelectedData").gameObject.SetActive(false);
                    return;
                }
                break;
            //データ削除タブ
            case 4:
                if (!start)
                {
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    File.Delete(Path.Combine(folder, fail));
                    GetData();
                }
                SecondImage.transform.Find("SelectedData").gameObject.SetActive(false);
                return;
            //エラー
            default:
                Debug.LogError("Miss!! Error!! ");
                return;
        }

        GameManager.MyGameInstance.StartGame(selectNo, 1);
    }
}
