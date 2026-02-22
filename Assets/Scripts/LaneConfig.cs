public static class LaneConfig
{
    public const int LaneCount = 4;

    public static readonly float[] LaneX = { -2.25f, -0.75f, 0.75f, 2.25f };
    public static float GetLaneX(int laneIndex) => LaneX[laneIndex];
}
