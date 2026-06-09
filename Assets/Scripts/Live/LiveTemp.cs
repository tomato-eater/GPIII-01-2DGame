using System.Collections.Generic;
using System;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

/// <summary>
/// 行動規定
/// </summary>
public enum ModeTypeList
{
    First,
    Default,
    Attack,
    Damage,
    Death,
    Finish
}

/// <summary>
/// 自分や敵のクラスのテンプレートクラス
/// </summary>
public class LiveTemp : MonoBehaviour
{
    ///<summary> ID </summary>
    public int Id;
    /// <summary> ヒットポイント </summary>
    public ReactiveProperty<float> Hp { get; private set; } = new();

    /// <summary> 攻撃力 </summary>
    protected float AttackPower;

    ///<summary> 防御力 </summary>
    protected float Defense;

    /// <summary> 移動速度 </summary>
    protected float MoveSpeed;

    /// <summary> ジャンプ力 </summary>
    protected float JumpPower;

    /// <summary> 接地判定 </summary>
    public bool IsGround;

    /// <summary> ダメージを受けるかの判定 </summary>
    protected bool DisableDamage;

    /// <summary> 物理演算コンポーネント </summary>
    protected Rigidbody2D Rb2d;

    /// <summary> アニメータ－コンポーネント </summary>
    public Animator Anima;

    /// <summary>
    /// 行動規定
    /// </summary>
    public ModeTypeList ModeType;

    /// <summary>
    /// enum
    /// </summary>
    protected Dictionary<ModeTypeList, Action> action;

    /*-----*/

    /// <summary> IDを取得 </summary>
    /// <returns></returns>
    public int GetId() {  return Id; }

    ///<summary> StatusをSet </summary>
    public virtual void SetStatus(StatusBox box) { Debug.Log("テンプレートSetStatus"); }

    /// <summary> ゲーム開始前に呼び出される関数 </summary>
    public virtual void First() { Debug.Log("テンプレートFirst"); }

    /// <summary>
    /// 通常時に呼び出される関数
    /// </summary>
    public virtual void Default() { Debug.Log("テンプレートMove"); }

    /// <summary>
    /// 攻撃時に呼び出される関数
    /// </summary>
    public virtual void Attack() { Debug.Log("テンプレートAttack"); }

    /// <summary>
    /// ダメージ時に呼び出される関数
    /// </summary>
    public virtual void Damage(float damageAmount, float posX) {
        DisableDamage = true;
        ModeType = ModeTypeList.Damage;
        var dir = Mathf.Sign(transform.position.x - posX);
        Rb2d.AddForce(new Vector2(dir * damageAmount, damageAmount), ForceMode2D.Impulse);
        Timer().Forget();
    }

    async UniTask Timer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        if (ModeType == ModeTypeList.Death) return;
        ModeType = ModeTypeList.Default;
        DisableDamage = false;
    }

    /// <summary>
    /// 死亡時に呼び出される関数
    /// </summary>
    public virtual void Death() { Debug.Log("テンプレートDeath"); }

    /// <summary>
    /// ゲーム終了時に呼び出される関数
    /// </summary>
    public virtual void Finish() { Debug.Log("テンプレートFinish"); }
}