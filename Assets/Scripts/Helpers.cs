namespace Helpers
{
    public static class Convert
    {
        /// <summary>
         /// Converts a given value in inches to meters.
         /// 1 Unity unit = 1 meter = .0254 inches.
         /// 1 inch = .0254 meters.
         /// </summary>
        public static float InchToMeter( float inch )
        {
            return inch * .0254f;
        }
    }
}