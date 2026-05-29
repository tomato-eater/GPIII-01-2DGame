using Cysharp.Threading.Tasks;
using R3;               // R3 core
using R3.Triggers;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Playerの操作クラス
/// </summary>
public class Player : LiveTemp
{
    /// <summary>
    /// enum
    /// </summary>
    private Dictionary<ModeTypeList, Action> action;

    /// <summary>
    /// 移動量の取得
    /// </summary>
    float MoveValue;

    /// <summary>
    /// 空中ジャンプの判定
    /// </summary>
    bool DoubleJump;

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
    /// PlayerInputのMoveが操作されたのを検知
    /// </summary>
    /// <param name="value"></param>
    void OnMove(InputValue value)
    {
        var Value = value.Get<Vector2>();
        MoveValue = Value.x;
    }

    void OnJump(InputValue value)
    {

    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed && (ModeType == ModeTypeList.Default))
            ModeType = ModeTypeList.Attack;
    }

    // Update is called once per frame
    void Update()
    {
        //ModeTypeによって呼び出す関数を変えている
        if (action.ContainsKey(ModeType)) action[ModeType].Invoke();
    }


    public override void Default()
    {
        Debug.Log("PlayerのDefault");
    }

    public override void Attack()
    {
        CancellationToken ct=this.GetCancellationTokenOnDestroy();
        Anima.Play("Attack");
        Attacking(ct).Forget();
    }

    async UniTask Attacking(CancellationToken ct)
    {
        await UniTask.Yield(PlayerLoopTiming.Update, ct); // 1フレーム待機
        await UniTask.WaitUntil(() => Anima.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f, cancellationToken: ct);
        ModeType = ModeTypeList.Default;
    }

    //メモ　unity 再生したアニメーションが終わるまで待つ UniTask

    public override void Death()
    {
        Debug.Log("PlayerのDeath");
    }

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
                else
                {
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach(ContactPoint2D cont in collision.contacts)
        {
            if (cont.normal.y > 0.5f)
            {
                IsGround = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
    }
}