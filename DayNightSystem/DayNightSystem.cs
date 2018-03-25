using Cube.Gameplay;
using UnityEngine;

namespace Cube.World
{
    public class DayNightSystem : MonoBehaviour, IDayNightSystem
    {
        public DayNightSettings settings;

        [Range(0f, 1f)]
        [SerializeField]
        float _dayPercentage = 0f;
        public float dayPercentage {
            get { return _dayPercentage; }
        }
        
        ITimeSystem _timeSystem;

        void Awake()
        {
            gameObject.SetSystem<IDayNightSystem>(this);
        }

        void Update()
        {
            if (_timeSystem == null) {
                _timeSystem = SystemProvider.GetSystem<ITimeSystem>(gameObject);
                return;
            }

            var initialTimeOffset = settings.dayLengthInSeconds;
            var time = initialTimeOffset + _timeSystem.time;
            var localTime = time - Mathf.Floor(time / settings.dayLengthInSeconds) * settings.dayLengthInSeconds;
            _dayPercentage = localTime / settings.dayLengthInSeconds;
        }
    }
}