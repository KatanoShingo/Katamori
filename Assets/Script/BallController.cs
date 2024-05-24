using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 5.0f; // 速度
    private Rigidbody rb;

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
        AttachObject(collision.transform);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    AttachObject(other.transform);
    //}

    private void AttachObject(Transform hitTransform)
    {
        // 親オブジェクトが存在するかチェックし、存在しなければ直接hitTransformを使用
        Transform checkTransform = hitTransform.parent != null ? hitTransform.parent : hitTransform;

        // 特定のタグ"NotAttachable"を持つオブジェクトを除外
        if (!checkTransform.CompareTag("NotAttachable"))
        {
            // タグが"NotAttachable"でなければ、そのオブジェクトを球体の子オブジェクトにする
            checkTransform.SetParent(transform);

            // さらに衝突したオブジェクトのisTriggerをtrueに設定して、他の衝突を防ぐ
            checkTransform.GetComponent<Collider>().isTrigger = true;
        }
    }
}
