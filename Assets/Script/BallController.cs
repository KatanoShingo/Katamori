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
    
    void OnTriggerEnter(Collider other)
    {
        // 親オブジェクトが存在するかどうかをチェックし、存在しなければ直接otherを使用
        Transform checkTransform = other.transform.parent != null ? other.transform.parent : other.transform;

        // 親オブジェクト、またはそのオブジェクト自体がCollectibleタグを持っているかチェック
        if (checkTransform.CompareTag("Collectible"))
        {
            // タグがCollectibleであれば、そのオブジェクトを球体の子オブジェクトにする
            checkTransform.SetParent(transform);
        }
    }
}