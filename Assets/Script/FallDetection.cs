using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDetection : MonoBehaviour
{
    float fallThreshold = -50.0f; // 奈落の高さのしきい値

    void Update()
    {
        // プレイヤーの高さがしきい値よりも低いかどうかをチェック
        if (transform.position.y < fallThreshold)
        {
            // シーンを再リロード
            ReloadScene();
        }
    }

    void ReloadScene()
    {
        // 現在のシーンを取得
        Scene currentScene = SceneManager.GetActiveScene();
        // 現在のシーンを再リロード
        SceneManager.LoadScene(currentScene.name);
    }
}
