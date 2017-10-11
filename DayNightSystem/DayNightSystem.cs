using UnityEngine;
using Core;

public class DayNightSystem : MonoBehaviour, IDayNightSystem
{
    [Range(1f, 1000f)]
    public float DayLengthTime;

    [Range(0f, 1f)]
    [SerializeField]
    float _dayPercentage = 0f;
    public float dayPercentage {
        get { return _dayPercentage; }
    }

    public bool fixedDayTime = false;

    ITimeSystem _timeSystem;

    void Awake()
    {
        SystemProvider.SetSystem<IDayNightSystem>(gameObject, this);
    }

    void Update()
    {
        if (fixedDayTime)
            return;

        if (_timeSystem == null) {
            _timeSystem = SystemProvider.GetSystem<ITimeSystem>(gameObject);
            return;
        }

        var initialTimeOffset = DayLengthTime;
        var time = initialTimeOffset + _timeSystem.time;
        var localTime = time - Mathf.Floor(time / DayLengthTime) * DayLengthTime;
        _dayPercentage = localTime / DayLengthTime;
    }
}
