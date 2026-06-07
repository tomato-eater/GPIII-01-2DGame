using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Menuで戦う敵を制御するクラス </summary>
public class MenuBattle : MonoBehaviour
{
    [SerializeField] SceneAsset[] S_Asset;
    int SelectScene = 0;

    /// <summary> MenuのBattleで表示・非表示するものを決める </summary>
    public void SetUp()
    {
        int lv = GameManager.MyGameInstance.GetStatus().Lvl;
        SelectScene = 0;
        int i = 0;
        var box = transform.GetChild(0).GetChild(0);
        while (i < box.childCount) 
        {
            var text = box.GetChild(i).Find("LevelText").GetComponent<Text>();
            if (int.Parse(text.text.Substring(1)) <= lv)
            {
                int index = i;
                box.GetChild(i).gameObject.SetActive(true);
                box.GetChild(i).GetComponent<Button>().onClick.AddListener(() => SelectBattle(index));
                var line = box.GetChild(i).GetComponent<Outline>();
                line.effectColor = SelectScene == i ? Color.white : Color.black;
            }
            else
            {
                box.GetChild(i).gameObject.SetActive(false);
            }
            i++;
        }
    }

    /// <summary> 次のシーンを登録する </summary>
    /// <param name="no"></param>
    void SelectBattle(int no) { SelectScene = no; }

    /// <summary> 次のシーンの名前を渡す </summary>
    /// <returns></returns>
    public string GetSceneName() { return S_Asset[SelectScene].name; }
}
