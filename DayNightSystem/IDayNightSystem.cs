namespace Cube.World {
    public interface IDayNightSystem {
        /// <summary>
        /// Current percentage of the day in the range [0,1].
        /// </summary>
        float dayPercentage { get; }
    }
}