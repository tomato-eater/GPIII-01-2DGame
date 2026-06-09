using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Figurine : LiveTemp
{
    TextMeshProUGUI TotalText;
    TextMeshProUGUI RecentText;

    private void Start()
    {
        var box = GameObject.Find("UICanvas");
        TotalText = box.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TotalText.text = $"Total : {0}";
        RecentText = box.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        RecentText.text = $"Recently : {0}";

        Rb2d = GetComponent<Rigidbody2D>();
    }

    public override void SetStatus(StatusBox box)
    {
        base.SetStatus(box);
    }

    public override void Damage(float damageAmount, float posX)
    {
        if (DisableDamage) return;
        base.Damage(damageAmount, posX);
        RecentText.text = $"Recently : {damageAmount:0.##}";
        damageAmount += float.Parse(TotalText.text.Substring(8));
        TotalText.text = $"Total : {damageAmount:0.##}";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Wall"))
            Rb2d.AddForce(new Vector2(-Mathf.Sign(transform.position.x) * 5, 3f), ForceMode2D.Impulse);
    }
}
