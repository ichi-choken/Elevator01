using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Sample : MonoBehaviour
{
    private Rigidbody rb;
    private PhysicsMaterial physicMaterial;

    float friction;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ColliderからPhysicMaterialを取得
        physicMaterial = GetComponent<Collider>().material;
        friction = CalculateFrictionForce();
    }

    /// <summary>
    /// 移動時の摩擦力を計算する
    /// </summary>
    float CalculateFrictionForce()
    {
        // 動摩擦係数
        float dynamicFriction = physicMaterial.dynamicFriction;

        // 法線力 = 質量 × 重力加速度（水平面の場合）
        float normalForce = rb.mass * Physics.gravity.magnitude;
        // 摩擦力 = 動摩擦係数 × 法線力
        float frictionForce = dynamicFriction * normalForce;

        return frictionForce;
    }

    void Update()
    {
        float dynamicFriction = physicMaterial.dynamicFriction;
        float staticFriction = physicMaterial.staticFriction;
        float normalForce = rb.mass * Physics.gravity.magnitude;
        // 摩擦力 = 動摩擦係数 × 法線力
        float frictionForce = dynamicFriction * normalForce;
        if (rb.linearVelocity.magnitude < 0.01f)
        {
            // 静止している場合は静摩擦力を考慮
            frictionForce = staticFriction * normalForce+0.1f; // 少し余裕を持たせる
        }
        rb.AddForce(Vector3.right * frictionForce); // 右方向に力を加える
    }
}