using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventChannel", menuName = "Events/EventChannel")]
public class EventChannel : ScriptableObject
{
    public UnityAction OnEventTriggered;

    public void RaiseEvent()
    {
        OnEventTriggered?.Invoke();
    }
}
