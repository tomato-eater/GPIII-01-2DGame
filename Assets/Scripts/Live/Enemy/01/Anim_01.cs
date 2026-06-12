using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary> Slime_01のAnimationによる制御 </summary>
public class Anim_01 : MonoBehaviour
{
    /// <summary> 親クラス </summary>
    Slime_01 Parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { Parent = transform.parent.GetComponent<Slime_01>(); }

    /// <summary> アニメーションが一段落した </summary>
    public void EndAnimation() { Parent.Ani = false; }

    /// <summary> 移動(Animatorと動きを合わせるため) </summary>
    public void MoveAnim() { Parent.Move(); }

    /// <summary> 着地したか否か(Animatorと動きを合わせるため) </summary>
    public async UniTask MoveStop() {
        Parent.Anima.speed = 0;
        while (!Parent.IsGround) {      //空中ならAnimatorを止める
            await UniTask.Yield();
        }
        Parent.Anima.speed = 1;
    }
}