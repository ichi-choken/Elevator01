using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Door doorR; // ドア1への参照
    public Door doorL; // ドア2への参照
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
