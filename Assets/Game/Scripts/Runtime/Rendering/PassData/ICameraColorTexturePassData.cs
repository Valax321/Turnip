using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.PassData
{
    /// <summary>
    /// Import this to have access to the intermediate camera target color data.
    /// </summary>
    [PassDataProvider]
    public interface ICameraColorTexturePassData
    {
        public TextureHandle CameraColorTexture { get; set; }
    }
    
    public sealed class InputCameraColorTextureData : ICameraColorTexturePassData
    {
        public InputCameraColorTextureData(TextureHandle handle)
        {
            CameraColorTexture = handle;
        }

        public TextureHandle CameraColorTexture { get; set; }
    }
}