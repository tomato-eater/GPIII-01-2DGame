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

    ///<summary> 空中滞在時間 </summary>
    public float AirTime;

    /// <summary> コンポーネント取得等 </summary>
    private void Start() {
        ModeType = ModeTypeList.First;
        Action = new Dictionary<ModeTypeList, Action>() {
            { ModeTypeList.Default, Default },
        };

        Rb2d = GetComponent<Rigidbody2D>();
        Anima = GetComponentInChildren<Animator>();
    }

    /// <summary> Status登録 </summary>
    /// <param name="box"></param>
    public override void SetStatus(StatusBox box) { base.SetStatus(box); }

    /// <summary> PlayerInputのAttackが操作されたのを検知・実行 </summary>
    /// <param name="value"></param>
    void OnAttack(InputValue value) {
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

    /// <summary> PlayerInputのJumpが操作さたのを検知・実行 </summary>
    /// <param name="value"></param>
    void OnJump(InputValue value) { if (DoubleJump || IsGround) JumpTrigger = true; }

    // Update is called once per frame
    void Update() {
        //ModeTypeによって呼び出す関数を変えているが、あまり意味が無くなってしまった
        if (Action.ContainsKey(ModeType)) Action[ModeType].Invoke();
        if (!IsGround) { AirTime += Time.deltaTime; }
    }

    /// <summary> Playerの移動、ジャンプ等 </summary>
    protected override void Default() {
        Rb2d.linearVelocityX = MoveValue * (IsGround ? MoveSpeed : MoveSpeed * 0.5f);

        if (IsGround) {         //地上にいる
            Rb2d.gravityScale = 3;

            if (MoveValue != 0) {
                transform.localScale = new(Mathf.Sign(MoveValue), 1, 1);
            }
            Anima.Play(MoveValue == 0 ? "Idle" : "Run");
        }

        if (JumpTrigger) {      //ジャンプする
            JumpTrigger = false;
            DoubleJump = IsGround;
            Rb2d.linearVelocityY = IsGround ? JumpPower : JumpPower * 0.8f;

            if (MoveValue != 0) {
                transform.localScale = new(Mathf.Sign(MoveValue), 1, 1);
            }

            if (!DoubleJump) Anima.Play("Jump");
        }
    }

    /// <summary> 攻撃 </summary>
    public override void Attack() {
        if (ModeType == ModeTypeList.Finish) return;
        ModeType = ModeTypeList.Default;
        DisableDamage = false;
    }

    /// <summary> 攻撃されたとき実行 </summary>
    /// <param name="damage"></param>
    /// <param name="posX"></param>
    public override void Damage(float damage, float posX) {
        if (DisableDamage) return;

        base.Damage(damage, posX);

        if (Hp.Value <= 0) {    //死亡
            Hp.Value = 0;
            Death();
        }
    }

    /// <summary> 死んだら実行 </summary>
    protected override void Death() {
        ModeType = ModeTypeList.Death;
        Anima.Play("Die");
        if(TryGetComponent<BoxCollider2D>(out var coll))
        {
            coll.enabled = false;
        }
        Debug.Log("消える処理とか追加したい");
    }

    /// <summary> 足元が何かに当たったら </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision) {
        foreach (ContactPoint2D cont in collision.contacts) {
            if (cont.normal.y > 0.5f) {
                DoubleJump = true;
                if (collision.collider.CompareTag("Enemy")) {
                    if (ModeType != ModeTypeList.Attack) EnemyJump();
                    break;
                }
                else if(collision.collider.CompareTag("Floor")) {
                    IsGround = true;
                    AirTime = 0;
                }
            }
        }
    }

    /// <summary> 敵を踏んでいる時 </summary>
    void EnemyJump() { Rb2d.AddForce(new Vector2(transform.localScale.x * 10, 5), ForceMode2D.Impulse); }

    /// <summary> 地面から離れた </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Floor")) {
            IsGround = false;
        }
    }

    /// <summary> 攻撃が敵に当たった </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.TryGetComponent<LiveTemp>(out var live)) {
            live.Damage((IsGround ? AttackPower : AttackPower * 1.5f) + AirTime, transform.position.x);
        }
    }
}