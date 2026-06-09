using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Menuで戦う敵を制御するクラス </summary>
public class MenuBattle : MonoBehaviour
{
    [SerializeField] SceneAsset[] S_Asset;
    int SelectScene = 0;
    Transform Box;

    /// <summary> MenuのBattleで表示・非表示するものを決める </summary>
    public void SetUp()
    {
        int lv = GameManager.MyGameInstance.GetMyStatus().Lvl;
        SelectScene = 0;
        int i = 0;
        Box = transform.GetChild(0).GetChild(0);
        while (i < Box.childCount) 
        {
            if (Box.GetChild(i).Find("LevelText").TryGetComponent<Text>(out var text))
            {
                if (int.Parse(text.text.Substring(1)) <= lv)
                {
                    int index = i;
                    Box.GetChild(i).gameObject.SetActive(true);
                    Box.GetChild(i).GetComponent<Button>().onClick.AddListener(() => SelectBattle(index));
                    var line = Box.GetChild(i).GetComponent<Outline>();
                    line.effectColor = SelectScene == i ? Color.white : Color.black;
                    Box.GetChild(i).Find("ClearText").gameObject.SetActive(GameManager.MyGameInstance.GetClearStage(i));
                }
                else
                {
                    Box.GetChild(i).gameObject.SetActive(false);
                }
                i++;
            }
            else
            {
                Debug.LogError("BattleButtonの作成に問題が発生しています");
                return;
            }
        }
    }

    /// <summary> 次のシーンを登録する </summary>
    /// <param name="no"></param>
    void SelectBattle(int no) { 
        Box.GetChild(SelectScene).GetComponent<Outline>().effectColor = Color.black;
        SelectScene = no;
        Box.GetChild(SelectScene).GetComponent<Outline>().effectColor = Color.white;
    }

    /// <summary> 次のシーンの名前を渡す </summary>
    /// <returns></returns>
    public string GetSceneName() {
        if (0 <= SelectScene && SelectScene < S_Asset.Length) return S_Asset[SelectScene].name;
        Debug.LogError("読み込みシーンの配列外を指定");
        return null;
    }
}
