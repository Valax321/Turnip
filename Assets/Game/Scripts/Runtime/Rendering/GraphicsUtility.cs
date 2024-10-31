using UnityEngine;

namespace Radish.Rendering
{
    public static class GraphicsUtility
    {
        public static bool isSRGB => QualitySettings.activeColorSpace == ColorSpace.Linear;
    }
}