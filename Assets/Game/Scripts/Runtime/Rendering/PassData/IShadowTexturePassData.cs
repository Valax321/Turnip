using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.PassData
{
    [PassDataProvider]
    public interface IShadowTexturePassData
    {
        public TextureHandle ShadowTexture { get; }
    }
    
    public sealed class InputShadowTextureData : IShadowTexturePassData
    {
        public TextureHandle ShadowTexture { get; }

        public InputShadowTextureData(TextureHandle handle)
        {
            ShadowTexture = handle;
        }
    }
}