using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] StatusList list;

    /// <summary> セーブファイルの番号 </summary>
    private int DataNo = -1;

    [SerializeField] StatusBox Status;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
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
    private void OnApplicationQuit() { MyGameInstance = null; }

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

    /// <summary> Scene切り替え </summary>
    /// <param name="name"></param>
    public void LoadScene(string name = "Title") { SceneManager.LoadScene(name); }

    public void StartGame(bool NewGame, int no, string name = "")
    {
        DataNo = no;
        CheckDataFolder();
        var text = CheckDataFile(DataNo);
        if (NewGame)
        {
            Status = list.GetStatusDataById(0).GetStatus().Clone();
            Status.Nama = name;
        }
        else
        {
            if(text == "") {
                Quit();
                return;
            }

            Status.Nama = text.Substring(5);

            text = CheckDataFile(no, 1).Substring(5);
            Status.Lvl = int.Parse(text);

            text = CheckDataFile(no, 2).Substring(5);
            Status.Hp = int.Parse(text);

            text = CheckDataFile(no, 3).Substring(5);
            Status.Atk = int.Parse(text);

            text = CheckDataFile(no, 4).Substring(5);
            Status.Def = int.Parse(text);

            text = CheckDataFile(no, 5).Substring(5);
            Status.Spd = int.Parse(text);

            text = CheckDataFile(no, 6).Substring(5);
            Status.Jpw = int.Parse(text);

            text = CheckDataFile(no, 7).Substring(5);
            Status.Pit = int.Parse(text);
        }
    }

    public void DeleteData(int no)
    {
        CheckDataFolder();
        File.Delete(Path.Combine(SAVEFOLDERPATH, SAVEFAILPATH(no)));
    }

    public void SaveData()
    {
        CheckDataFolder();
        CheckDataFile(DataNo);
        string[] texts = { 
            "Name:" + Status.Nama, 
            "Lvl :" + Status.Lvl, 
            "Hp  :" + Status.Hp, 
            "Atk :" + Status.Atk, 
            "Dfe :" + Status.Def, 
            "Spd :" + Status.Spd, 
            "Jpw :" + Status.Jpw, 
            "Pit :" + Status.Pit 
        };
        File.WriteAllLines(Path.Combine(SAVEFOLDERPATH + "/" + SAVEFAILPATH(DataNo)), texts);
    }

    public StatusBox GetStatus() { return Status ?? new StatusBox(); }

    public void SetStatus(StatusBox status) {
        Status.Hp = status.Hp;
        Status.Atk = status.Atk;
        Status.Def = status.Def;
        Status.Spd = status.Spd;
        Status.Jpw = status.Jpw;
        Status.Pit = status.Pit;
    }
}