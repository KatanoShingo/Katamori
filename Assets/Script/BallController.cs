using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 5.0f; // 速度
    private Rigidbody rb;
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

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
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

            Debug.Log((checkTransform.GetComponent<Tag>()!=null).ToString());
        // 特定のタグ"NotAttachable"を持つオブジェクトを除外し、許容サイズの範囲内であるか確認
        if (checkTransform.CompareTag("Collectible") && checkTransform.localScale.y <= currentMaxSizeY)
        {
            Debug.Log("当たった");
            // タグが"Collectible"であれば、そのオブジェクトを球体の子オブジェクトにする
            checkTransform.SetParent(transform);

            // 衝突したオブジェクトのコライダー（親があれば親のコライダー、なければそのオブジェクトのコライダー）のisTriggerをtrueに設定
            Collider targetCollider = hitTransform.GetComponent<Collider>() ? hitTransform.GetComponent<Collider>() : checkTransform.GetComponent<Collider>();
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
