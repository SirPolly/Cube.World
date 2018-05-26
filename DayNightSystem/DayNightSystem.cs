using Cube.Gameplay;
using UnityEngine;

namespace Cube.World {
    public class DayNightSystem : MonoBehaviour, IDayNightSystem {
        public DayNightSettings settings;

        public bool isNight {
            get { return _dayPercentage > 0.5f; }
        }

        [Range(0f, 1f)]
        [SerializeField]
        float _dayPercentage = 0f;
        public float dayPercentage {
            get { return _dayPercentage; }
        }

        TimeSystem _timeSystem;

        void Awake() {
            gameObject.SetSystem<IDayNightSystem>(this);

#if UNITY_EDITOR
            _dayPercentage = 0.3f;
#endif
        }

        void Update() {
#if UNITY_EDITOR
            return;
#endif

            if (_timeSystem == null) {
                _timeSystem = SystemProvider.GetSystem<TimeSystem>(gameObject);
                return;
            }

            var initialTimeOffset = settings.dayLengthInSeconds;
            var time = initialTimeOffset + _timeSystem.time;
            var localTime = time - Mathf.Floor(time / settings.dayLengthInSeconds) * settings.dayLengthInSeconds;
            _dayPercentage = localTime / settings.dayLengthInSeconds;
        }
    }
}