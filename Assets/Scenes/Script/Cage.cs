using UnityEngine;

public class Cage : MonoBehaviour
{
    public float floorHeight = 3.0f; // 各階の高さ [m]
    public float maxSpeed = 1.0f; // 最大速度 [m/s]
    public float maxAcceleration = 1.0f; // 最大加速度 [m/s^2]
    public float jark = 2.0f; // 起動・停止時の衝撃（ジャーク） [m/s^3]
    public float doorOpenTime = 2.0f; // ドアが開いている時間 [s]
    public float doorCloseTime = 2.0f; // ドアが閉まる時間 [s]
    public float movingError = 0.01f; // 移動誤差 [m]

    private int currentFloor; // 現在の階
    enum CurrentMode
    {
        Idle,
        MovingUp,
        MovingDown,
        DoorOpening,
        DoorClosing
    }
    private CurrentMode currentMode;

    private Rigidbody rb;
    private GameManager gameManager; // GameManagerへの参照
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentFloor = 1; // 初期階を1に設定
        currentMode = CurrentMode.Idle; // 初期状態をIdleに設定
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.SetCageFloor(currentFloor); // GameManagerに現在の階を通知
        rb.useGravity = false; // 重力を無効にする
    }

    int _targetFloor; // 目標階
    float targetFloorY; // 目標階の高さ [m]
    public void MoveToFloor(int targetFloor)
    {
        _targetFloor = targetFloor;
        targetFloorY = (_targetFloor - 1) * floorHeight; // 目標階の高さを計算
        Debug.Log("目標高さ: " + targetFloorY);
        rb.constraints &= ~RigidbodyConstraints.FreezePositionY; // Y解除
        currentAcceleration = 0; // 加速度をリセット
        if(currentFloor < targetFloor){
            currentMode = CurrentMode.MovingUp;
        } else if(currentFloor > targetFloor){
            currentMode = CurrentMode.MovingDown;
        }
    }

    float currentVelocity; // 現在の速度 [m/s]
    float currentAcceleration=0; // 現在の加速度 [m/s^2]
    private void MoveElevator(string direction)
    {
        int _direction = direction == "up" ? 1 : -1;
        // 方向に応じて加速度を設定
        currentAcceleration += jark * Time.fixedDeltaTime; // ジャークを加算して加速度を増加
        currentAcceleration = Mathf.Min(currentAcceleration, maxAcceleration); // 最大加速度を超えないようにする
        
        currentVelocity = Mathf.Abs(rb.linearVelocity.y) + currentAcceleration * Time.fixedDeltaTime; // 加速度を適用して速度を更新
        currentVelocity = Mathf.Min(currentVelocity, maxSpeed); // 最大速度を超えないようにする

        rb.linearVelocity = Vector3.up * currentVelocity * _direction; // 速度を適用してエレベーターを移動    
        Debug.Log("現在の速度: " + currentVelocity + "," +rb.linearVelocity.y+ " m/s, 加速度: " + currentAcceleration + " m/s^2");

        // 目標階に到着したら停止する
        if ((transform.localPosition.y - targetFloorY)*_direction > -movingError)
        {
            StopElevator();
        }
    }

    private void StopElevator()
    {
        rb.MovePosition(new Vector3(transform.position.x, targetFloorY, transform.position.z)); // 目標階の位置に移動
        rb.linearVelocity = Vector3.zero; // 速度をゼロにする
        rb.constraints |= RigidbodyConstraints.FreezePositionY; // Y固定
        currentFloor = _targetFloor; // 現在の階を更新
        currentMode = CurrentMode.Idle; // 状態をIdleに設定
        gameManager.SetCageFloor(currentFloor); // GameManagerに現在の階を通知
        Debug.Log("階 " + currentFloor + " に到着しました。");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentMode)
        {
            case CurrentMode.MovingUp:
                MoveElevator("up");
                break;
            case CurrentMode.MovingDown:
                MoveElevator("down");
                break;
            case CurrentMode.DoorOpening:
                // ドアを開ける処理
                break;
            case CurrentMode.DoorClosing:
                // ドアを閉める処理
                break;
            case CurrentMode.Idle:
                // 待機状態の処理
                break;
        }
    }
}
