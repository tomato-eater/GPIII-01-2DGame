using Cysharp.Threading.Tasks;
using UnityEngine;

public class P_Anim : MonoBehaviour
{
    LiveTemp Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = transform.parent.GetComponent<Player>();
    }

    async void StayAttack()
    {
        Player.Anima.speed = 0;
        while(!Player.IsGround)
        {
            await UniTask.Yield();
        }
        Player.Anima.speed = 1;
    }

    public void EndAttack()
    {
        Player.Attack();
    }
}
