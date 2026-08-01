using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Menuで戦う敵を制御するクラス </summary>
public class MenuBattle : MonoBehaviour
{
    /// <summary> BattleSceneの登録所 </summary>
    [SerializeField] SceneAsset[] S_Asset;
    /// <summary> 選択番号 </summary>
    int SelectScene = 0;
    /// <summary> Buttonの親を保管 </summary>
    Transform Box;

    /// <summary> MenuのBattleで表示・非表示するものを決める </summary>
    public void SetUp() {
        int lv = GameManager.MyGameInstance.GetMyStatus().Lvl;
        SelectScene = 0;
        Box = transform.Find("Viewport/Content");
        for (int i = 0; i < Box.childCount; i++) {        //Buttonの数
            if (Box.GetChild(i).Find("LevelText").TryGetComponent<Text>(out var text)) {
                if (int.Parse(text.text.Substring(1)) <= lv) {      //設定したLevelを見て表示・非表示判断
                    int index = i;
                    Box.GetChild(i).gameObject.SetActive(true);
                    Box.GetChild(i).GetComponent<Button>().onClick.AddListener(() => SelectBattle(index));
                    var line = Box.GetChild(i).GetComponent<Outline>();
                    line.effectColor = SelectScene == i ? Color.white : Color.black;
                    Box.GetChild(i).Find("ClearText").gameObject.SetActive(GameManager.MyGameInstance.GetClearStage(i));    //Clear済みならClearと表示
                }
                else {      //非表示
                    Box.GetChild(i).gameObject.SetActive(false);
                }
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
    void SelectBattle(int no) {         //選択したものが分かりやすいようにOutLineを変えている
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

    /// <summary> Battle開始 </summary>
    public void StartBattle() {
        GameManager.MyGameInstance.LoadPanel(true);
        GameManager.MyGameInstance.LoadScene(GetSceneName(),SelectScene);
    }
}
