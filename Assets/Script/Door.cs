using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        RightDoor, // 右に開くドア
        LeftDoor   // 左に開くドア
    }
    [Header("ドアの種類")]
    
    public DoorType doorType = DoorType.RightDoor;

    [Header("位置設定")]
    public float damperDistance = 0.2f;     // ダンパー作動開始距離 [m]

    [Header("速度設定")]
    public float targetAccelationVelocity = 0.5f;          // 目標速度 [m/s]
    public float accelerationTime = 0.2f;        // 目標速度に達するまでの時間 [s]
    public float targetDecelerationVelocity = 0.1f; // 減速開始前の目標速度 [m/s]
    public float decelerationTime = 0.2f;        // 目標速度から減速するまでの時間 [s]

    [Header("ダンパー設定")]
    public float damperCoefficient = 30f;        // 空気ダンパー係数

    // ── 状態変数 ──────────────────────────────
    private int dir = 1; // 開ける方向: 1=右に開く, -1=左に開く
    private string currentMode = ""; // 現在のモード: "open" or "close" or ""(停止中)
    private float maxDoorPositionX; // ドアの最大位置 (全開位置)
    private float minDoorPositionX; // ドアの最小位置 (全閉位置

    // 物理パラメータを保持する構造体(CalculateForceの引数としてまとめるため)
    public struct PhysicsParams
    {
        public float Vd;   // 目標速度 [m/s]
        public float Td;   // 加速時間 [s]
        public float Vl;   // 目標速度 [m/s](完全に開閉する前の目標減速速度)
        public float Tl;   // 減速時間 [s](完全に開閉する前の減速時間)
        public float MuP;  // 静摩擦係数 (μ')
        public float Mu;   // 動摩擦係数 (μ)
        public float m;    // 質量 [m]
        public float g;    // 重力加速度 [g]
        public float Xa;   // ダンパa境界位置 [Xa]
        public float Xb;   // ダンパb境界位置 [Xb]
        public float c;    // ダンパ減衰係数 [c]
    }
    private PhysicsParams p; // 物理パラメータのインスタンス

    // ── 内部状態 ──────────────────────────────
    private Rigidbody rb;
    private PhysicsMaterial pm;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Collider>().material;

        // ドアの種類に応じて開ける方向を設定
        dir = (doorType == DoorType.RightDoor) ? 1 : -1;
        Debug.Log($"dir: {dir}");
        float sizeX = transform.localScale.x; // ドアの幅

        // 物理パラメータの初期化
        p = new PhysicsParams
        {
            Vd = targetAccelationVelocity,
            Td = accelerationTime,
            Vl = targetDecelerationVelocity,
            Tl = decelerationTime,
            Mu = pm.staticFriction, // 静摩擦係数
            MuP = pm.dynamicFriction,  // 動摩擦係数
            m = rb.mass,
            g = Physics.gravity.magnitude,
            // ダンパーの境界位置をドアのサイズに基づいて計算
            Xb = sizeX *2 - damperDistance - sizeX/2f,
            Xa = damperDistance + sizeX/2f,
            c = damperCoefficient
        };

        maxDoorPositionX = sizeX / 2f + sizeX;
        minDoorPositionX = sizeX / 2f;

        Close(); // 初期状態は閉じるモードに設定
        //Open(); // 初期状態は開くモードに設定
    }

    public void Open()
    {
        currentMode = "open";
    }

    public void Close()
    {
        currentMode = "close";
    }

    /// <summary>
    /// 現在の状態から必要な外力Fを計算します
    /// </summary>
    /// <param name="_v">現在の速度</param>
    /// <param name="_x">現在の位置</param>
    /// <param name="p">物理パラメータ</param>
    /// <returns>外力 F [N]</returns>
    /// 0<Xc<x<Xb<(開位置)
    public float CalculateForce(string mode, float x, float v, PhysicsParams p)
    {
        float N = p.m * p.g; // 垂直抗力
        const float Epsilon = 0.01f; //停止とみなす速度の閾値

        float targetA = 0; // 目標加速度
        float targetV = 0; // 目標速度

        //float _x = (dir>0) ? x : -x; // ドアの位置
        float _x = Mathf.Abs(x); // ドアの位置
        float _v = Mathf.Abs(v);

        
        // A. 目標速度、加速度の設定

        // 1. 開閉の終了前
        if ((_x < p.Xa && mode == "close")
         || (p.Xb < _x && mode == "open"))
        {
            targetV = p.Vl;//減速済み速度
            if(_v > p.Vl + Epsilon)
            {
                // Vlまで減速させるための加速度 (負の値)
                targetA = (p.Vl - p.Vd) / p.Tl;
            }
        }
        // 2. 開閉の開始時
        else if((p.Xa < _x && mode == "close")
             || (_x < p.Xb && mode == "open"))
        {
            targetV = p.Vd;//加速済み速度
            if(_v < p.Vd - Epsilon)
            {
                // Vdまで加速させるための加速度 (正の値)
                targetA = (p.Vd - 0) / p.Td;
            }
        }

        // B. 力fの計算

        // 1. 走行中の計算 (慣性力 + 摩擦力)
        float f = p.m * targetA + p.MuP * N;

        // 2. ダンパの計算: 
        if(_v < Epsilon && targetA > 0)
        {
            f = p.m * targetA + p.Mu * N; // 停止している場合は静摩擦係数を使用
        }
        // 条件：
        // 　　　{開始側(x < Xa) または 終端側(Xb < x)}
        //  かつ {停止時ではない}　で有効
        else if (_x < p.Xa || p.Xb < _x) 
        {
            f += p.c * _v;
        }

        return mode == "open" ? f : -f; // 右の扉を想定して開くときは正、閉じるときは負の力を返す
    }

    private float CalulateDumpingForce(string mode, float x, float v, PhysicsParams p)
    {
        float _v = Mathf.Abs(v);
        //float _x = (dir>0) ? x : -x; // ドアの位置
        float _x = Mathf.Abs(x); // ドアの位置
        float f;
        if (_x < p.Xa || p.Xb < _x)
        {
            f = p.c * _v;
        }
        else
        {
            f = 0;
        }
        return mode == "open" ? f : -f; // 開くときは正、閉じるときは負の力を返す
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            // 停止エリアに衝突した場合、停止とみなす
            Debug.Log("ドア同士が衝突しました。ドアを停止させます。");
            currentMode = "";
            //rb.linearVelocity = Vector3.zero; // ドアを完全に停止させる
            //rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StopSensor"))
        {
            // 停止エリアに入った場合、停止とみなす
            currentMode = "";
            rb.linearVelocity = Vector3.zero; // ドアを完全に停止させる
        }
    }
    /*
     // ドアが完全に開いた/閉じた位置に達している場合、停止とみなす
     // 物理的な衝突が発生しない場合に備えて、位置と速度からも停止を判断する
     
    private void CheckDoorStop(float x, float v, PhysicsParams p)
    {
        const float Epsilon = 0.01f; //停止とみなす位置の閾値
    
        // ドアが完全に開いた/閉じた位置に達している場合、停止とみなす
        if (x < minDoorPositionX +Epsilon || maxDoorPositionX - Epsilon < x)
        {
            currentMode = "";
            rb.velocity = Vector3.zero; // ドアを完全に停止させる
        }
    }
    */

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(currentMode != "")
        {
            float px = transform.localPosition.x;
            float vx = rb.linearVelocity.x;
            float F = CalculateForce(currentMode, px, vx, p);//モーターによる力
            float f = CalulateDumpingForce(currentMode,px, vx, p);//ダンパによる力
            Vector3 force = new Vector3(dir * (F - f), 0, 0);//右の扉が開くときをベースに考える
            rb.AddForce(force, ForceMode.Force);
            //CheckDoorStop(px, vx, p);
            if(doorType == DoorType.RightDoor)
            Debug.Log($"v: {vx:F2}, F: {F:F2}, f: {f:F2}, force: {force.x:F2}");
        }
    }
}
