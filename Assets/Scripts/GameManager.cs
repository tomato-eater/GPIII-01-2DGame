using UnityEngine;

/// <summary>
/// ゲームを制御するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    public static GameManager MyGameInstance {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(MyGameInstance!=null || MyGameInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        MyGameInstance = this;
        DontDestroyOnLoad(gameObject);
    }

}
