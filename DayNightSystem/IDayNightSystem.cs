namespace Cube.World {
    public interface IDayNightSystem {
        bool isNight {
            get;
        }

        /// <summary>
        /// Current percentage of the day in the range [0,1].
        /// </summary>
        float dayPercentage { get; }
    }
}