using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.PassData
{
    /// <summary>
    /// Import this to have access to the camera target depth texture.
    /// </summary>
    [PassDataProvider]
    public interface ICameraDepthTexturePassData
    {
        public TextureHandle CameraDepthTexture { get; set; }
    }
    
    public sealed class InputCameraDepthTextureData : ICameraDepthTexturePassData
    {
        public InputCameraDepthTextureData(TextureHandle handle)
        {
            CameraDepthTexture = handle;
        }

        public TextureHandle CameraDepthTexture { get; set; }
    }
}