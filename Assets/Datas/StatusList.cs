using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステータスデータのリストを格納するクラス
/// </summary>
[CreateAssetMenu(fileName = "StatusList", menuName = "ScriptableObjects/StatusList")]
public class StatusList : ScriptableObject
{
    public List<StatusData> statusDataList = new List<StatusData>();

    public StatusData GetStatusDataById(int id)
    {
        if (0 <= id && id < statusDataList.Count)
            return statusDataList.Find(status => status.id == id);
        Debug.LogError("StatusDataの範囲外");
        return null;
    }
}
