using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    private List<Button> L_Buttons = new List<Button>();
    int NowSelectButton = -1;
    int nextSelecrtButton;

    GameObject front;
    private List<GameObject> M_Obj=new List<GameObject>();
    private List<GameObject> R_Obj=new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var box = transform.Find("L_Canvas/L_Image");
        for(int i = 1; i < box.childCount; i++)
        {
            L_Buttons.Add(box.GetChild(i).GetComponent<Button>());
        }

        front = transform.Find("M_Image/FrontImage").gameObject;
        front.SetActive(true);
        for(int i = 0; i < front.transform.childCount; i++)
        {
            M_Obj.Add(front.transform.GetChild(i).gameObject);
            M_Obj[i].SetActive(false);
        }
        front.SetActive(false);

        box = transform.Find("R_Image");
        for( int i = 0; i < box.childCount; i++)
        {
            R_Obj.Add(box.GetChild(i).gameObject);
            R_Obj[i].SetActive(false);
        }

        GameManager.MyGameInstance.LoadPanel(false);
    }

    public void L_ButtonTrigger(int no)
    {

    }

}
