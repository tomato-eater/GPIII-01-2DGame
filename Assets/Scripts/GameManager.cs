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

    private void OnApplicationQuit()
    {
        MyGameInstance = null;
    }

    /// <summary>
    /// ゲームのStartボタンが押されたら呼び出される関数
    /// </summary>
    /// <param name="dataNo"></param>
    /// <param name="nextS"></param>
    public void StartGame(int dataNo, int nextS)
    {
        DataNo = dataNo;
        transform.GetChild(0).gameObject.SetActive(true);
        
    }
}
