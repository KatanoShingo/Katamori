using UnityEngine;

public class FollowBall : MonoBehaviour
{
    public Transform target; // ターゲットとなる球体
    public Vector3 initialOffset = new Vector3(0, 5, -10); // 初期のカメラとターゲットの距離と方向のオフセット
    public float smoothSpeed = 0.125f; // カメラの動きを滑らかにするための速度
    public float minHeight = 1.0f; // カメラの最小高さ
    public float rotationSpeed = 70.0f; // カメラの回転速度
    public float offsetMultiplier = 0.1f; // 子オブジェクトの数に応じたオフセットの倍率

    private float currentAngle = 0.0f; // カメラの現在の角度

    void LateUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // 入力に応じてカメラの角度を更新
        currentAngle += horizontalInput * rotationSpeed * Time.deltaTime;

        // ターゲットの子オブジェクトの数に基づいてオフセットを調整
        int childCount = target.childCount;
        Vector3 offset = initialOffset + Vector3.back * childCount * offsetMultiplier;

        // カメラの位置を計算
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // カメラとターゲットの間の視線をチェック
        RaycastHit hit;
        if (Physics.Raycast(target.position, -offset.normalized, out hit, offset.magnitude))
        {
            if (hit.collider.gameObject != target.gameObject)
            {
                // 障害物があればカメラを障害物の位置まで近づける
                // smoothedPosition = hit.point + offset.normalized * 0.5f; // 障害物から少し離れた位置に設定
            }
        }

        // カメラの高さを最小値以上に保つ
        smoothedPosition.y = Mathf.Max(smoothedPosition.y, minHeight);

        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}
