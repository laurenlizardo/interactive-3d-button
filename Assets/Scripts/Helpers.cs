namespace Helpers
{
    /// <summary>
    /// Converts a given value in inches to meters.
    /// </summary>
    public static class Convert
    {
        // 1 Unity unit = 1 meter = .0254 inches
        // 1 inch = .0254 meters
        public static float InchToMeter( float inch )
        {
            return inch * .0254f;
        }
    }
}