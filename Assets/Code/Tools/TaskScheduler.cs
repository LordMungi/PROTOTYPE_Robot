using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskScheduler : MonoBehaviour, IService
{
    public class ScheduledCall
    {
        public Action Callback;
        public float RemainingTime;

        public ScheduledCall(Action callback, float remainingTime) { this.Callback = callback; this.RemainingTime = remainingTime; }
    }

    public bool IsPersistant => false;

    List<ScheduledCall> _scheduledCalls = new List<ScheduledCall>();

    void Update()
    {
        for (int i = _scheduledCalls.Count - 1; i >= 0; i--)
        {
            ScheduledCall call = _scheduledCalls[i];
            call.RemainingTime -= Time.deltaTime;

            if (call.RemainingTime <= 0f)
            {
                call.Callback.Invoke();
                _scheduledCalls.RemoveAt(i);
            }
        }
    }

    public void Schedule(Action callback, float remainingTime)
    {
        _scheduledCalls.Add(new ScheduledCall(callback, remainingTime));
    }
}
