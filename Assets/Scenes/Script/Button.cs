using UnityEngine;

public class Button : MonoBehaviour
{
    enum ButtonType
    {
        Open,
        Close
    }
    
    [SerializeField] private ButtonType buttonType; // ボタンの種類をインスペクターで設定
    [SerializeField] private int floor; // ボタンが対応する階をインスペクターで設定

    private GameManager gameManager; // GameManagerへの参照
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    private void OnValidate()
    {
        floor = Mathf.Clamp(floor, 0, 7);
    }

    void OnMouseDown()
    {
        if(buttonType == ButtonType.Open)
        {
            gameManager.OpenDoor(floor);
        }
        else if(buttonType == ButtonType.Close)
        {
            gameManager.CloseDoor(floor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
