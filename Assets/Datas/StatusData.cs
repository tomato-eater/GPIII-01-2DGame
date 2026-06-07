using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusBox
{
    public string Nama;
    public int Lvl;
    public int Hp;
    public int Atk;
    public int Def;
    public int Spd;
    public int Jpw;
    public int Pit;

    public StatusBox Clone() => (StatusBox)this.MemberwiseClone();
}

/// <summary>
/// ステータスなどのデータを格納するクラス
/// </summary>
[CreateAssetMenu(fileName = "StatusData", menuName = "ScriptableObjects/StatusData")]
public class StatusData : ScriptableObject
{
    [SerializeField, Tooltip("ID")] int Id;
    public int id => Id;

    [SerializeField] StatusBox statusBox;

    public StatusBox GetStatus() { return statusBox; }
}