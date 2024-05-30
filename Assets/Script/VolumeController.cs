using UnityEngine;

public class VolumeController : MonoBehaviour
{
    [SerializeField] AudioSource audioSourceBgm; // BGM用のAudioSource
    [SerializeField] AudioSource audioSourceSe; // 効果音用のAudioSource
    float volumeChangeSpeed = 0.1f; // 音量変更の速さ

    void Update()
    {
        // KキーとJキーの入力に基づいて音量を変更
        if (Input.GetKey(KeyCode.K))
        {
            ChangeVolume(volumeChangeSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.J))
        {
            ChangeVolume(-volumeChangeSpeed * Time.deltaTime);
        }
    }

    // 音量を変更するメソッド
    private void ChangeVolume(float delta)
    {
        audioSourceBgm.volume = Mathf.Clamp(audioSourceBgm.volume + delta, 0f, 1f);
        audioSourceSe.volume = Mathf.Clamp(audioSourceSe.volume + delta, 0f, 1f);
    }
}
