using UnityEngine;

public class Menu : MonoBehaviour
{
    GameObject M_F_Panel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        M_F_Panel = transform.Find("M_Image/FrontImage").gameObject;
        M_F_Panel.SetActive(false);


        GameManager.MyGameInstance.LoadPanel(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
