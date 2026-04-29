using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private Cage cage; // エレベーターの管理スクリプトへの参照
    public List<DoorManager> doorManagers = new List<DoorManager>(); // ドアの管理スクリプトへの参照
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string doorState; // ドアの状態を管理する変数
    private int nextFloor; // 次の階を管理する変数
    private int cageFloor; // エレベーターの現在の階を管理する変数
    void Start()
    {
        doorState= "";        
        nextFloor = 2;
        cage=GameObject.Find("Cage").GetComponent<Cage>(); // Cageスクリプトへの参照を取得
    }

    public void SetDoorState(string state)
    {
        doorState = state;
        if(nextFloor != 0 && doorState == "closed")
        {
            moveCaage(nextFloor);
        }
    }

    private void moveCaage(int floor)
    {
        if (cageFloor==0 || floor == cageFloor)
        {
            Debug.Log("移動中または同じ階にいるのに移動しようとしています。c,n"+cageFloor+","+floor);
            return;
        }
        else
        {
            Debug.Log("階 " + floor + " に移動します。");
            cage.MoveToFloor(floor);
        }
    }

    public void SetCageFloor(int floor)
    {
        cageFloor = floor;
        if (cageFloor != 0)
        {
            OpenDoor(cageFloor); // エレベーターが到着した階のドアを開ける
        }
        if(nextFloor == 2)
        {
            nextFloor = 1; // 次の階をリセット
        }
        else if(nextFloor == 1)
        {
            nextFloor = 2; // 次の階をリセット
        }
    }

    public void OpenDoor(int floor)
    {
        doorManagers[floor - 1].OpenDoor();
    }

    public void CloseDoor(int floor)
    {
        if(cageFloor == floor)
        {
            doorManagers[floor - 1].CloseDoor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
