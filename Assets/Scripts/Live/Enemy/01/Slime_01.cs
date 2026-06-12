using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>スライム(Blue)の制御クラス </summary>
public class Slime_01 : LiveTemp
{
    /// <summary> ターゲット </summary>
    Transform Player;

    /// <summary> 行動制限 </summary>
    public bool Ani = false;

    /// <summary> 向き </summary>
    float Dir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Player = FindAnyObjectByType<Player>().transform;
        ModeType = ModeTypeList.First;
        Action = new Dictionary<ModeTypeList, Action>() {
            { ModeTypeList.Default, Default },
        };
        Rb2d = GetComponent<Rigidbody2D>();
        Anima = GetComponentInChildren<Animator>();
    }

    /// <summary> ステータスを取得 </summary>
    public override void SetStatus(StatusBox box) { base.SetStatus(box); }

    // Update is called once per frame
    void Update() { if (Action.ContainsKey(ModeType)) Action[ModeType].Invoke(); }

    /// <summary> 通常時呼び出される </summary>
    protected override void Default() {
        if (Ani) return;
        Ani = true;

        Dir = Mathf.Sign(Player.position.x - transform.position.x);
        if (Dir == 0) Dir = 1;
        transform.localScale = new Vector3(Dir, 1, 1);

        //距離に応じて行動を変化
        var diff = Vector2.Distance(transform.position, Player.position);
        if (diff <= 2.5f) {
            Anima.Play("Attack");
        }
        else {
            Anima.Play("Move");
        }
    }

    /// <summary> 移動させる(Animatorで制御) </summary>
    public void Move() { Rb2d.AddForce(new Vector2(Dir * MoveSpeed, JumpPower), ForceMode2D.Impulse); }

    public override void Damage(float damageAmount, float posX) {
        if (DisableDamage) return;
        base.Damage(damageAmount, posX);

        if (Hp.Value <= 0) {    //死亡判別
            Hp.Value = 0;
            Death();
        }
    }

    protected override void Death()
    {
        ModeType = ModeTypeList.Death;
        Anima.Play("Die");
        if (TryGetComponent<BoxCollider2D>(out var coll)) {     //Collider無効化
            coll.enabled = false;
        }
        FindAnyObjectByType<BattleController>().KillEnemy();    
    }

    /// <summary> 着地判定 </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision) {
        foreach (ContactPoint2D cont in collision.contacts) {
            if (cont.normal.y > 0.5f) {                         //自身の下部分が接触
                if (collision.collider.CompareTag("Floor")) {   
                    IsGround = true;
                }
            }
        }
    }

    /// <summary> 離着判定 </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Floor")) {
            IsGround = false;
        }
    }

    /// <summary> 攻撃判定 </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        if (collision.TryGetComponent<LiveTemp>(out var live)) {
            live.Damage((IsGround ? AttackPower : AttackPower * 1.5f), transform.position.x);
        }
    }
}
