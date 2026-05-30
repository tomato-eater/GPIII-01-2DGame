using Cysharp.Threading.Tasks;
using R3;               // R3 core
using R3.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Playerの操作クラス
/// </summary>
public class Player : LiveTemp
{
    /// <summary>
    /// 移動量の取得
    /// </summary>
    float MoveValue;

    /// <summary>
    /// 空中ジャンプの判定
    /// </summary>
    bool DoubleJump;

    /// <summary>
    /// ジャンプを実行するかの判定
    /// </summary>
    bool JumpTrigger;

    /// <summary>
    /// HpやAttackPower等を取得
    /// </summary>
    private void Awake()
    {
        Hp.Value = 10;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        action = new Dictionary<ModeTypeList, Action>() {
            { ModeTypeList.Default, Default },
            { ModeTypeList.Attack,  Attack  },
            { ModeTypeList.Death,   Death   }};

        Rb2d = GetComponent<Rigidbody2D>();
        Anima = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// PlayerInputのAttackが操作されたのを検知・実行
    /// </summary>
    /// <param name="value"></param>
    void OnAttack(InputValue value)
    {
        if (ModeType != ModeTypeList.Default) return;

        ModeType = ModeTypeList.Attack;
        Anima.Play("Attack");
        Rb2d.gravityScale = 8;
    }

    /// <summary>
    /// PlayerInputのMoveが操作されたのを検知・実行
    /// </summary>
    /// <param name="value"></param>
    void OnMove(InputValue value)
    {
        var Value = value.Get<Vector2>();
        MoveValue = Value.x;
    }

    /// <summary>
    /// PlayerInputのJumpが操作さたのを検知・実行
    /// </summary>
    /// <param name="value"></param>
    void OnJump(InputValue value)
    {
        if (DoubleJump || IsGround)
            JumpTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        //ModeTypeによって呼び出す関数を変えている
        if (action.ContainsKey(ModeType)) action[ModeType].Invoke();
    }

    /// <summary>
    /// Playerの移動、ジャンプ等
    /// </summary>
    public override void Default()
    {
        Rb2d.linearVelocityX = MoveValue * (IsGround ? MoveSpeed : MoveSpeed * 0.5f);

        if (IsGround)
        {
            Rb2d.gravityScale = 3;

            if (MoveValue != 0)
            {
                transform.localScale = new(Mathf.Sign(MoveValue), 1, 1);
            } 
            Anima.Play(MoveValue == 0 ? "Idle" : "Run");
        }

        if (JumpTrigger) 
        {
            JumpTrigger = false;
            DoubleJump = IsGround;
            Rb2d.linearVelocityY = IsGround ? JumpPower : JumpPower * 0.8f;

            if (MoveValue != 0)
                transform.localScale = new(Mathf.Sign(MoveValue), 1, 1);

            if (!DoubleJump)
                Anima.Play("Jump");
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public override void Attack()
    {
        JumpTrigger = false;
        if (Anima.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
            ModeType = ModeTypeList.Default;
    }

    /// <summary>
    /// 死んだら実行
    /// </summary>
    public override void Death()
    {
       
    }

    /// <summary>
    /// 足元が何かに当たったら
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D cont in collision.contacts)
        {
            if (cont.normal.y > 0.5f)
            {
                DoubleJump = true;
                if (collision.collider.CompareTag("Enemy"))
                {
                    //EnemyJump();
                    break;
                }
                else if(collision.collider.CompareTag("Floor"))
                {
                    IsGround = true;
                    DoubleJump = true;
                }
            }
        }
    }

    /// <summary>
    /// 地面から離れた
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            IsGround = false;
        }
    }

    /// <summary>
    /// 攻撃が敵に当たった
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<LiveTemp>(out var live))
        {
            Debug.Log("Playerの攻撃が敵に当たった");
            Debug.Log(live.gameObject.name);
        }
        Hp.Value -= 0.5f;
    }
}