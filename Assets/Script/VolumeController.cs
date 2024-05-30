using UnityEngine;

public class VolumeController : MonoBehaviour
{
    public AudioSource audioSource; // AudioSourceを格納するための変数
    public AudioSource audioSourceSe; // AudioSourceを格納するための変数
    public float volumeChangeSpeed = 0.1f; // 音量変更の速さ

    void Update()
    {
        // JキーとKキーの入力を取得
        if (Input.GetKey(KeyCode.K))
        {
            // Kキーが押されたとき、音量を増加
            audioSource.volume = Mathf.Clamp(audioSource.volume + volumeChangeSpeed * Time.deltaTime, 0f, 1f);
            audioSourceSe.volume = Mathf.Clamp(audioSource.volume + volumeChangeSpeed * Time.deltaTime, 0f, 1f);
        }
        else if (Input.GetKey(KeyCode.J))
        {
            // Jキーが押されたとき、音量を減少
            audioSource.volume = Mathf.Clamp(audioSource.volume - volumeChangeSpeed * Time.deltaTime, 0f, 1f);
            audioSourceSe.volume = Mathf.Clamp(audioSource.volume - volumeChangeSpeed * Time.deltaTime, 0f, 1f);
        }
    }
}
