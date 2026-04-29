using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Door doorR; // ドア1への参照
    public Door doorL; // ドア2への参照
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private int floor; // ボタンが対応する階をインスペクターで設定

    private GameManager gameManager; // GameManagerへの参照
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnValidate()
    {
        floor = Mathf.Clamp(floor, 1, 7);
    }


    private string doorState; // ドアの状態を管理する変数
    public void SetDoorState(string state)
    {
        doorState = state;
        if(gameManager == null)gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        gameManager.SetDoorState(doorState); // GameManagerにドアの状態を通知
    }

    public void OpenDoor()
    {
        doorR.Open();
        doorL.Open();
    }

    public void CloseDoor()
    {
        doorR.Close();
        doorL.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
