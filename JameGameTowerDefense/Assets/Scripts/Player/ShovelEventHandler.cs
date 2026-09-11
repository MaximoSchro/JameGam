using UnityEngine;

public class ShovelEventHandler : MonoBehaviour
{
   public void SendEventToHead()
    {
        ShovelHead.SwingAnimFinished?.Invoke();
    }
}
