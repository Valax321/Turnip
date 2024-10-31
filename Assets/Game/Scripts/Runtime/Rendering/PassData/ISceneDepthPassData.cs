using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.PassData
{
    [PassDataProvider]
    public interface ISceneDepthPassData
    {
        TextureHandle SceneDepth { get; }
    }
    
    public sealed class InputSceneDepthData : ISceneDepthPassData
    {
        public TextureHandle SceneDepth { get; }

        public InputSceneDepthData(TextureHandle handle)
        {
            SceneDepth = handle;
        }
    }
}