using System.Collections.Generic;
using System;
using UnityEngine;
using R3;

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
    Give,
    Finish
}

/// <summary>
/// 自分や敵のクラスのテンプレートクラス
/// </summary>
public class LiveTemp : MonoBehaviour
{
    /// <summary>
    /// ヒットポイント
    /// </summary>
    public ReactiveProperty<float> Hp { get; private set; } = new();

    /// <summary>
    /// 攻撃力
    /// </summary>
    public float AttackPower;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// ジャンプ力
    /// </summary>
    public float JumpPower;

    /// <summary>
    /// 接地判定
    /// </summary>
    protected bool IsGround;

    /// <summary>
    /// 物理演算コンポーネント
    /// </summary>
    protected Rigidbody2D Rb2d;

    /// <summary>
    /// アニメータ－コンポーネント
    /// </summary>
    protected Animator Anima;

    /// <summary>
    /// 行動規定
    /// </summary>
    public ModeTypeList ModeType;

    /// <summary>
    /// enum
    /// </summary>
    public Dictionary<ModeTypeList, Action> action;

    /*-----*/

    /// <summary>
    /// ゲーム開始前に呼び出される関数s
    /// </summary>
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
    public virtual void Damage(float damageAmount, float posX) { Debug.Log("テンプレートDamage"); }

    /// <summary>
    /// 死亡時に呼び出される関数
    /// </summary>
    public virtual void Death() { Debug.Log("テンプレートDeath"); }

    /// <summary>
    /// ゲーム終了時に呼び出される関数
    /// </summary>
    public virtual void Finish() { Debug.Log("テンプレートFinish"); }
}