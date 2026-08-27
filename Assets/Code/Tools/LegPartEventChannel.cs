using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LegPartEventChannel", menuName = "Events/LegPartEventChannel")]
public class LegPartEventChannel : ScriptableObject
{
    public UnityAction<LegPart> OnEventTriggered;

    public void RaiseEvent(LegPart arg0)
    {
        OnEventTriggered?.Invoke(arg0);
    }
}
