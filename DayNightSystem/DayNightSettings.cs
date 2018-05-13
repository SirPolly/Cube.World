using UnityEngine;

namespace Cube.World {
    [CreateAssetMenu(menuName = "Cube.World/DayNightSettings")]
    public class DayNightSettings : ScriptableObject {
        [Range(1f, 1000f)]
        public float dayLengthInSeconds;
    }
}
