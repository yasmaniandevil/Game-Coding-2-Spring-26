using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject door;
    public void Open()
    {
        door.SetActive(false);
    }

    public void Close()
    {
        door.SetActive(true);
    }
}
