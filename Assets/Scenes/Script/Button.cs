using UnityEngine;

public class Button : MonoBehaviour
{
    enum ButtonType
    {
        Open,
        Close
    }
    [SerializeField] private ButtonType buttonType; // ボタンの種類をインスペクターで設定

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        if(buttonType == ButtonType.Open)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().OpenDoor();
        }
        else if(buttonType == ButtonType.Close)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().CloseDoor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
