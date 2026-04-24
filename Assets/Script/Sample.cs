using UnityEngine;

public class Smple : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private bool hasCollided = false; // 衝突フラグ
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Door"))
        {
            hasCollided = true; // 衝突したことを記録
            Debug.Log("衝突しました");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hasCollided)
        {
            rb.linearVelocity = new Vector3(0, 0, 1); // 毎フレーム、右方向に一定の速度を与える
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // 衝突後は速度をゼロにする
        }
    }
}
