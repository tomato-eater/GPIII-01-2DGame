using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary> Player‚ÌAnimator‚É‚æ‚é§Œä </summary>
public class P_Anim : MonoBehaviour
{
    /// <summary> Player </summary>
    LiveTemp Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { Player = transform.parent.GetComponent<Player>(); }

    /// <summary> Attack‚ÅŒÄ‚Ño‚· </summary>
    async void StayAttack() {
        Player.Anima.speed = 0;
        while(!Player.IsGround) {       //‹ó’†‚È‚çAnimator‚ğ~‚ß‚é
            await UniTask.Yield();
        }
        Player.Anima.speed = 1;
    }
    /// <summary> Attack‚ªI‚í‚Á‚½‚çŒÄ‚Ño‚· </summary>
    public void EndAttack() {
        Player.Attack();
    }
}
