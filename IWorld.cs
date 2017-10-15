
public interface IWorld
{
    ITimeSystem timeSystem { get; }
    IWaterSystem waterSystem { get; }
    IDayNightSystem dayNightSystem { get; set; }
}
