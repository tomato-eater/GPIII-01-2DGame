using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームを制御するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    public static GameManager MyGameInstance {  get; private set; }

    /// <summary>
    /// セーブファイルの番号
    /// </summary>
    int DataNo = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(MyGameInstance != null && MyGameInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        MyGameInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// ゲーム終了時、実行
    /// </summary>
    private void OnApplicationQuit()
    {
        MyGameInstance = null;
    }

    /// <summary>
    /// セーブ番号登録
    /// </summary>
    /// <param name="dataNo"></param>
    public void SetDataNo(int dataNo) { DataNo = dataNo; }

    /// <summary>
    /// セーブ番号取得
    /// </summary>
    /// <returns></returns>
    public int GetDataNo() { return DataNo; }

    /// <summary>
    /// ロード画面を表示させる
    /// </summary>
    public void LoadPanel(bool activ)
    {
        transform.GetChild(0).gameObject.SetActive(activ);
    }

    /// <summary>
    /// Scene切り替え
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name) { SceneManager.LoadScene(name); }


}
