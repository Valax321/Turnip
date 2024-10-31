using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    public readonly struct CameraContext
    {
        public readonly Camera Camera;
        public readonly CullingResults CullResults;

        public CameraContext(Camera camera, in CullingResults cullResults)
        {
            this.Camera = camera;
            this.CullResults = cullResults;
        }
    }
}