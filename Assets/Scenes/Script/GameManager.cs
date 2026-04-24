using UnityEngine;

public class GameManager : MonoBehaviour
{

    public DoorManager doorManager; // ドアの管理スクリプトへの参照
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OpenDoor()
    {
        doorManager.OpenDoor();
    }

    public void CloseDoor()
    {
        doorManager.CloseDoor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
