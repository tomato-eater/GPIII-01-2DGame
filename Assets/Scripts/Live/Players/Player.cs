using Cysharp.Threading.Tasks;
using R3;               // R3 core
using R3.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary> Playerの操作クラス </summary>
public class Player : LiveTemp
{
    /// <summary> 移動量の取得 </summary>
    float MoveValue;

    /// <summary> 空中ジャンプの判定 </summary>
    bool DoubleJump;

    /// <summary> ジャンプを実行するかの判定 </summary>
    bool JumpTrigger;

    /// <summary> ダメージを受けるかの判定 </summary>
    public bool DisableDamage;

    ///<summary> 空中滞在時間 </summary>
    public float AirTime;

    /// <summary> コンポーネント取得等 </summary>
    private void Awake()
    {
        ModeType = ModeTypeList.First;
        action = new Dictionary<ModeTypeList, Action>() {
            { ModeTypeList.Default, Default },
        };

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

        JumpTrigger = false;
        DisableDamage = true;
        ModeType = ModeTypeList.Attack;
        Anima.Play("Attack");
        Rb2d.gravityScale = 8;
    }

    /// <summary> PlayerInputのMoveが操作されたのを検知・実行 </summary>
    /// <param name="value"></param>
    void OnMove(InputValue value) { MoveValue = value.Get<Vector2>().x; }

    /// <summary>
    /// PlayerInputのJumpが操作さたのを検知・実行
    /// </summary>
    /// <param name="value"></param>
    void OnJump(InputValue value)
    {
        if (DoubleJump || IsGround)
            JumpTrigger = true;
    }

    /// <summary> GiveUpButton </summary>
    /// <param name="value"></param>
    void OnGiveUp(InputValue value)
    {
        if (ModeType == ModeTypeList.Default)
        {
            ModeType = ModeTypeList.Give;
        }
        else if (ModeType == ModeTypeList.Give)
        {
            ModeType = ModeTypeList.Default;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //ModeTypeによって呼び出す関数を変えている
        if (action.ContainsKey(ModeType)) action[ModeType].Invoke();
        if (!IsGround)
        {
            AirTime += Time.deltaTime;
        }
        else if(AirTime > 0)
        {
            AirTime = 0;
        }
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
        ModeType = ModeTypeList.Default;
        DisableDamage = false;
    }

    /// <summary>
    /// 攻撃されたとき実行
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="posX"></param>
    public override void Damage(float damage, float posX)
    {
        if (DisableDamage) return;
        DisableDamage = true;
        ModeType = ModeTypeList.Damage;
        var dir = Mathf.Sign(transform.position.x - posX);
        Rb2d.AddForce(new Vector2(dir * 5, 5), ForceMode2D.Impulse);
        DamageProcess(damage).Forget();
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    async UniTask DamageProcess(float damage)
    {
        Hp.Value -= damage;
        if (Hp.Value <= 0)
        {
            Hp.Value = 0;
            ModeType= ModeTypeList.Death;
            Death();
            return;
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        ModeType = ModeTypeList.Default;
        DisableDamage = false;
    }

    /// <summary>
    /// 死んだら実行
    /// </summary>
    public override void Death()
    {
        Anima.Play("Die");
        Rb2d.simulated = false;
        if(TryGetComponent<BoxCollider2D>(out var coll))
        {
            coll.enabled = false;
        }
    }

    /// <summary>
    /// 足元が何かに当たったら
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D cont in collision.contacts)
        {
            if (cont.normal.y > 0.75f)
            {
                DoubleJump = true;
                if (collision.collider.CompareTag("Enemy"))
                {
                    EnemyJump();
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

    void EnemyJump()
    {
        Rb2d.AddForce(new Vector2(transform.localScale.x * 10, 5), ForceMode2D.Impulse);
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
            live.Damage((IsGround ? AttackPower : AttackPower * 1.5f) + AirTime, transform.position.x);
        }
    }
}