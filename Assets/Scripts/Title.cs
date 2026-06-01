using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトルを制御するクラス
/// </summary>
public class Title : MonoBehaviour
{
    /// <summary>
    /// 画面1の開始ボタン
    /// </summary>
    [SerializeField] Button S_Buton;

    /// <summary>
    /// 画面1の出口ボタン
    /// </summary>
    [SerializeField] Button E_Buton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (S_Buton == null)
            S_Buton = transform.Find("FirstImage/StartButton").GetComponent<Button>();
        if (E_Buton == null)
            E_Buton = transform.Find("FirstImage/ExitButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 画面1の開始ボタンを押下
    /// </summary>
    /// <param name="title"></param>
    public void StartButton(bool title)
    {
        PushButton(title);
    }
    /// <summary>
    /// 画面1の出口ボタンを押下
    /// </summary>
    public void ExitButton()
    {
        PushButton(false);
    }

    void PushButton(bool title)
    {
        S_Buton.interactable = title;
        E_Buton.interactable = title;
    }
}
