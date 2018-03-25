using UnityEngine;

namespace Cube.World
{
    public class DayNightSettings : ScriptableObject
    {
        [Range(1f, 1000f)]
        public float dayLengthInSeconds;
    }
}
