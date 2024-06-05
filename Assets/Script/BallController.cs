using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform; // カメラのTransform
    Rigidbody rb;
    AudioSource audioSource; // AudioSource
    SphereCollider sphereCollider; // スフィアコライダー
    
    float colliderGrowthRate = 0.5f; // スフィアコライダーの成長率
    float speed = 5.0f; // 速度
    float magnificationHorizontal = 50f;
    float magnificationVertical = 100f;

    int attachedObjectsCount = 0; // くっついたオブジェクトの数をカウント
    int currentMaxSizeY = 1; // 現在のsize.yの最大許容値

    Vector2 touchStart;
    Vector2 swipeDelta;
    bool isDragging;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = 0;
        float moveVertical = 0;

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
                moveHorizontal = swipeDelta.x / Screen.width;
                moveVertical = swipeDelta.y / Screen.height;

                moveHorizontal *= magnificationHorizontal;
                moveVertical *= magnificationVertical;

                Debug.Log($"Swipe Delta: {swipeDelta}, Move Vector: {moveHorizontal}, {moveVertical}");

                swipeDelta = Vector2.zero; // スワイプ処理後、デルタをリセット
            }
        }
        else
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        }

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

        // 特定のタグ"Collectible"を持つオブジェクトを確認
        if (checkTransform.CompareTag("Collectible"))
        {
            // タグが"Collectible"であれば、そのオブジェクトを球体の子オブジェクトにする
            checkTransform.SetParent(transform);

            // 衝突したオブジェクト及びその親の全てのコライダーを取得してisTriggerをtrueに設定
            Collider[] colliders = checkTransform.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.isTrigger = true;
            }

            // カウンターを増やす
            attachedObjectsCount++;

            audioSource.PlayOneShot(audioSource.clip);

            // 10個ごとに許容されるsize.yの最大値を増やし、スフィアコライダーを大きくする
            if (attachedObjectsCount % 10 == 0)
            {
                currentMaxSizeY++;
                sphereCollider.radius += colliderGrowthRate;
            }
        }
    }
}
