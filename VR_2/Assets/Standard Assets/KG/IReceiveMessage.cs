using UnityEngine.EventSystems;

public interface IRecieveMessage :  IEventSystemHandler {
    void OnReceive(int count);
}