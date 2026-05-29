using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 行動規定
/// </summary>
public enum ModeTypeList
{
    First,
    Default,
    Attack,
    Death,
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
    public float Hp;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public float Power;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float Speed;

    /// <summary>
    /// ジャンプ力
    /// </summary>
    public float JumpPower;

    /// <summary>
    /// 行動規定
    /// </summary>
    public ModeTypeList ModeType;

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
    /// 死亡時に呼び出される関数
    /// </summary>
    public virtual void Death() { Debug.Log("テンプレートDeath"); }

    /// <summary>
    /// ゲーム終了時に呼び出される関数
    /// </summary>
    public virtual void Finish() { Debug.Log("テンプレートFinish"); }
}