using UnityEngine;
using TMPro;

public class LoadUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float hopSpeed = 2.0f;   // 全体が一周する速度
    [SerializeField] float hopHeight = 15.0f;  // 跳ねる高さ
    [SerializeField] float hopDuration = 0.3f; // 1文字が跳ねている時間の長さ（0〜1）

    // Update is called once per frame
    void Update()
    {
        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;
        int characterCount = text.textInfo.characterCount;

        if (characterCount == 0) return;

        // 0 〜 文字数 の間で時間をループさせる
        float timeIndex = (Time.time * hopSpeed) % characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

            // 現在の時間（timeIndex）と、この文字の順番（i）の差を計算
            float diff = timeIndex - i;
            float offset = 0f;

            // 自分の番が来てから、hopDurationの時間が経過するまでの間だけ跳ねる
            if (diff >= 0 && diff < hopDuration)
            {
                // 進捗を 0 〜 1 に正規化
                float progress = diff / hopDuration;
                // サイン波の0〜180度を使って、0 → 1 → 0 の綺麗なジャンプ軌道を作る
                offset = Mathf.Sin(progress * Mathf.PI) * hopHeight;
            }

            Vector3 translation = new Vector3(0, offset, 0);

            sourceVertices[vertexIndex + 0] += translation;
            sourceVertices[vertexIndex + 1] += translation;
            sourceVertices[vertexIndex + 2] += translation;
            sourceVertices[vertexIndex + 3] += translation;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}