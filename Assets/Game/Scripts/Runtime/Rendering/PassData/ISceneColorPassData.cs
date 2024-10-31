using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.PassData
{
    [PassDataProvider]
    public interface ISceneColorPassData
    {
        TextureHandle SceneColor { get; }
    }
    
    public sealed class InputSceneColorData : ISceneColorPassData
    {
        public TextureHandle SceneColor { get; }

        public InputSceneColorData(TextureHandle handle)
        {
            SceneColor = handle;
        }
    }
}