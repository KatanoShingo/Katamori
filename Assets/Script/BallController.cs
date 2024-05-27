using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 5.0f; // 速度
    private Rigidbody rb;
    public Transform cameraTransform; // カメラのTransform

    private int attachedObjectsCount = 0; // くっついたオブジェクトの数をカウント
    private int currentMaxSizeY = 1; // 現在のsize.yの最大許容値

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // カメラの方向に基づいて移動方向を計算
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // カメラの方向を水平方向に制限
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 入力に基づいて移動方向を決定
        Vector3 movement = forward * moveVertical + right * moveHorizontal;

        rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        AttachObject(collision.transform, collision);
    }

    private void AttachObject(Transform hitTransform, Collision collision)
    {
        // 親オブジェクトが存在するかチェックし、存在しなければ直接hitTransformを使用
        Transform checkTransform = hitTransform.parent == null ? hitTransform : hitTransform.parent;

        // 特定のタグ"Collectible"を持つオブジェクトを確認し、許容サイズの範囲内であるかを確認
        if (checkTransform.CompareTag("Collectible") && checkTransform.localScale.y <= currentMaxSizeY)
        {
            // タグが"Collectible"であれば、そのオブジェクトを球体の子オブジェクトにする
            checkTransform.SetParent(transform);

            // 衝突したオブジェクトのコライダー（親があれば親のコライダー、なければそのオブジェクトのコライダー）のisTriggerをtrueに設定
            Collider targetCollider = hitTransform.GetComponent<Collider>() ?? checkTransform.GetComponent<Collider>();
            if (targetCollider != null)
            {
                targetCollider.isTrigger = true;
            }

            // カウンターを増やす
            attachedObjectsCount++;

            // 10個ごとに許容されるsize.yの最大値を増やす
            if (attachedObjectsCount % 10 == 0)
            {
                currentMaxSizeY++;
            }
        }
    }
}
