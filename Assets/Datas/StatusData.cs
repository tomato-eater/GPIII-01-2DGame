using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステータスなどのデータを格納するクラス
/// </summary>
[CreateAssetMenu(fileName = "StatusData", menuName = "ScriptableObjects/StatusData")]
public class StatusData : ScriptableObject
{
    [SerializeField, Tooltip("ID")] int Id;
    public int id => Id;

    [SerializeField, Tooltip("Nam")] string Name;
    [SerializeField, Tooltip("Lvl")] int Lvl;
    [SerializeField, Tooltip( "HP")] int Hp;
    [SerializeField, Tooltip("ATK")] int Atk;
    [SerializeField, Tooltip("DEF")] int Def;
    [SerializeField, Tooltip("SPD")] int Spd;
    [SerializeField, Tooltip("JPW")] int Jpw;

    public (string name, int lvl, int hp, int atk, int def, int spd, int jpw) GetStatus()
    {
        return (Name, Lvl, Hp, Atk, Def, Spd, Jpw);
    }
}

/// <summary>
/// ステータスデータのリストを格納するクラス
/// </summary>
[CreateAssetMenu(fileName = "StatusDataList", menuName = "ScriptableObjects/StatusDataList")]
public class StatusDataList : ScriptableObject
{
    public List<StatusData> statusDataList = new List<StatusData>();

    public StatusData GetStatusDataById(int id)
    {
        return statusDataList.Find(status => status.id == id);
    }
}