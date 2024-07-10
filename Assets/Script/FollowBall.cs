using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [SerializeField] Transform target; // ターゲットとなる球体
    Vector3 initialOffset = new Vector3(0, 7, -10); // 初期のカメラとターゲットの距離と方向のオフセット
    float smoothSpeed = 0.125f; // カメラの動きを滑らかにするための速度
    float minHeight = 1.0f; // カメラの最小高さ
    float rotationSpeed = 70.0f; // カメラの回転速度
    float offsetMultiplier = 0.2f; // 子オブジェクトの数に応じたオフセットの倍率
    float currentAngle = 0.0f; // カメラの現在の角度
    float magnification = 50f;

    Vector2 touchStart;
    Vector2 swipeDelta;
    bool isDragging;

    void LateUpdate()
    {
        float horizontalInput = 0;

        if (DeviceController.IsMobileDevice)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
                isDragging = true;
            }
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                swipeDelta = (Vector2)Input.mousePosition - touchStart;
                isDragging = false;

                // スクリーン幅で割ることでスケール調整
                horizontalInput = swipeDelta.x / Screen.width;

                horizontalInput *= magnification;

                swipeDelta = Vector2.zero; // スワイプ処理後、デルタをリセット
            }
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }

        // 入力に応じてカメラの角度を更新
        currentAngle += horizontalInput * rotationSpeed * Time.deltaTime;

        // ターゲットの子オブジェクトの数に基づいてオフセットを調整
        int childCount = target.childCount;
        Vector3 offset = initialOffset + Vector3.back * childCount * offsetMultiplier;

        // カメラの位置を計算
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // カメラの高さを最小値以上に保つ
        smoothedPosition.y = Mathf.Max(smoothedPosition.y, minHeight);

        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}
