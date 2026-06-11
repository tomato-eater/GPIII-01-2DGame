using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> ゲームを制御するクラス </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> セーブファイルの数 </summary>
    public int SAVEFAILCOUNT => 6;
    /// <summary> セーブファイルの場所 </summary>
    public string SAVEFOLDERPATH => Path.Combine(Application.streamingAssetsPath, "SaveData");
    /// <summary> セーブファイルの名前 </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public string SAVEFAILPATH(int no) => $"Data_{no}.txt";

    /// <summary> ゲームマネージャー </summary>
    public static GameManager MyGameInstance {  get; private set; }

    /// <summary> StatusDataを保管してるやつ </summary>
    [SerializeField] StatusList List;

    /// <summary> セーブファイルの番号 </summary>
    private int DataNo = -1;

    /// <summary> Battleの番号 </summary>
    int BattleNo = -1;

    /// <summary> PlayerのStatus </summary>
    StatusBox P_Status;

    /// <summary> Stageの攻略状況 </summary>
    List<bool> ClearStage = new List<bool>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        if(MyGameInstance != null && MyGameInstance != this) {
            Destroy(gameObject);
            return;
        }
        MyGameInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary> GameQuit </summary>
    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit()
#endif
    }

    /// <summary> ゲーム終了時、実行 </summary>
    private void OnApplicationQuit() {
        P_Status = null;
        ClearStage.Clear();
        MyGameInstance = null;
    }

    /// <summary> セーブフォルダーの確認 </summary>
    public void CheckDataFolder() { if (!Directory.Exists(SAVEFOLDERPATH)) Directory.CreateDirectory(SAVEFOLDERPATH); }
    /// <summary> セーブファイルの確認 </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public string CheckDataFile(int no, int line = 0) {
        if (!File.Exists(Path.Combine(SAVEFOLDERPATH, SAVEFAILPATH(no)))) { 
            File.Create(Path.Combine(SAVEFOLDERPATH, SAVEFAILPATH(no))).Close();
            return "";
        }
        return File.ReadAllLines(Path.Combine(SAVEFOLDERPATH, SAVEFAILPATH(no))).ElementAtOrDefault(line) ?? "";
    }

    /// <summary> ロード画面を表示させる </summary>
    public void LoadPanel(bool active) { transform.GetChild(0).gameObject.SetActive(active); }

    /// <summary> Scene切り替え と Battle番号登録 </summary>
    /// <param name="name"></param>
    public void LoadScene(string name = "Title", int no = -1) {
        BattleNo = no;
        SceneManager.LoadScene(name); 
    }

    /// <summary> TitleSceneにて、ゲームを開始した </summary>
    /// <param name="NewGame"></param>
    /// <param name="no"></param>
    /// <param name="name"></param>
    public void StartGame(bool NewGame, int no, string name = "") {
        DataNo = no;
        CheckDataFolder();
        var text = CheckDataFile(DataNo);
        if (NewGame) {  //NewGame
            P_Status = List.GetStatusDataById(0).GetStatus().Clone();
            P_Status.Nama = name;
        }
        else {          //LoadGame
            if(text == "") {
                Quit();
                return;
            }

            P_Status = new StatusBox();

            P_Status.Nama = text.Substring(5);

            text = CheckDataFile(no, 1).Substring(5);
            P_Status.Lvl = int.Parse(text);

            text = CheckDataFile(no, 2).Substring(5);
            P_Status.Hp = int.Parse(text);

            text = CheckDataFile(no, 3).Substring(5);
            P_Status.Atk = int.Parse(text);

            text = CheckDataFile(no, 4).Substring(5);
            P_Status.Def = int.Parse(text);

            text = CheckDataFile(no, 5).Substring(5);
            P_Status.Spd = int.Parse(text);

            text = CheckDataFile(no, 6).Substring(5);
            P_Status.Jpw = int.Parse(text);

            text = CheckDataFile(no, 7).Substring(5);
            P_Status.Pit = int.Parse(text);

            string clear = CheckDataFile(no, 8).Substring(0);
            for (int i = 0; i < clear.Length; i++) {
                ClearStage.Add(int.Parse(clear.Substring(i)) == 1);
            }
        }
    }

    /// <summary> SaveDataを削除 </summary>
    /// <param name="no"></param>
    public void DeleteData(int no) {
        CheckDataFolder();
        File.Delete(Path.Combine(SAVEFOLDERPATH, SAVEFAILPATH(no)));
    }

    /// <summary> Save実行 </summary>
    public void SaveData() {
        CheckDataFolder();
        CheckDataFile(DataNo);
        List<string> texts = new List<string>{
            "Name:" + P_Status.Nama,
            "Lvl :" + P_Status.Lvl,
            "Hp  :" + P_Status.Hp,
            "Atk :" + P_Status.Atk,
            "Dfe :" + P_Status.Def,
            "Spd :" + P_Status.Spd,
            "Jpw :" + P_Status.Jpw,
            "Pit :" + P_Status.Pit,
            string.Join("", ClearStage.Select(b => b ? "1" : "0"))
        };
        File.WriteAllLines(Path.Combine(SAVEFOLDERPATH + "/" + SAVEFAILPATH(DataNo)), texts);
    }

    /// <summary> Player_Statusの数値を取得 </summary>
    /// <returns></returns>
    public StatusBox GetMyStatus() { return P_Status ?? List.GetStatusDataById(0).GetStatus().Clone(); }
    /// <summary> Enemy_Statusの数値を取得 </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public StatusBox GetEnStatus(int no) { return List.GetStatusDataById(no).GetStatus().Clone(); }   

    /// <summary> P_Statusの数値を更新 </summary>
    /// <param name="status"></param>
    public void SetStatus(StatusBox status) {
        P_Status.Hp = status.Hp;
        P_Status.Atk = status.Atk;
        P_Status.Def = status.Def;
        P_Status.Spd = status.Spd;
        P_Status.Jpw = status.Jpw;
        P_Status.Pit = status.Pit;
    }

    /// <summary> StageのClear状況を取得 </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public bool GetClearStage(int no) {
        if (no < 0) {
            Debug.LogError("StageNoが負");
            return false;
        }
        else if (no >= ClearStage.Count) ClearStage.Add(false);
        return ClearStage[no];
    }

    /// <summary> StageClear状況を更新 </summary>
    public void SetClearStage() {
        if (BattleNo < 0) return;
        if (BattleNo >= ClearStage.Count) Debug.LogError("ClearStageの連携Miss");
        ClearStage[BattleNo] = true;
    }
}