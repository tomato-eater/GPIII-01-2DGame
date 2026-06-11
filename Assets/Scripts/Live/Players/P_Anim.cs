using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary> PlayerのAnimatorで制御するクラス </summary>
public class P_Anim : MonoBehaviour
{
    /// <summary>
    /// Player
    /// </summary>
    LiveTemp Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = transform.parent.GetComponent<Player>();
    }

    /// <summary>
    /// Attackで呼び出す
    /// </summary>
    async void StayAttack()
    {
        Player.Anima.speed = 0;
        while(!Player.IsGround) //空中ならAnimatorを止める
        {
            await UniTask.Yield();
        }
        Player.Anima.speed = 1;
    }
    /// <summary>
    /// Attackが終わったら呼び出す
    /// </summary>
    public void EndAttack()
    {
        Player.Attack();
    }
}
