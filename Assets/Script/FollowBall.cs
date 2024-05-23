using UnityEngine;

public class FollowBall : MonoBehaviour
{ 
    public Transform target; // ターゲットとなる球体
    public Vector3 offset = new Vector3(0, 5, -10); // カメラとターゲットの距離と方向のオフセット
    public float smoothSpeed = 0.125f; // カメラの動きを滑らかにするための速度
    public float minHeight = 1.0f; // カメラの最小高さ

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
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