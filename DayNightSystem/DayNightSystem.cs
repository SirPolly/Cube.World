using UnityEngine;

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
    
    IWorld _world;

    void Start()
    {
        _world = GetComponentInParent<IWorld>();
        _world.dayNightSystem = this;
    }

    void Update()
    {
        if (_world.timeSystem == null || fixedDayTime)
            return;

        var initialTimeOffset = DayLengthTime;
        var time = initialTimeOffset + _world.timeSystem.time;
        var localTime = time - Mathf.Floor(time / DayLengthTime) * DayLengthTime;
        _dayPercentage = localTime / DayLengthTime;
    }
}
