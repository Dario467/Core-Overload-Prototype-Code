using UnityEngine;

public class RoomManager : MonoBehaviour
{
   public static RoomManager Instance { get; private set; }
   public GameObject[] rooms;
   public GameObject ActualRooms;
   private RoomController ActualRoomController;

    void Awake()
    {
       if (Instance == null)
        {
            ActualRoomController = ActualRooms.GetComponent<RoomController>();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RestartRooms()
    {
        foreach(var room in rooms)
        {
           RoomController roomControllerroomController = room.GetComponent<RoomController>();
           roomControllerroomController.RestartLevel();
        }
    }

}
