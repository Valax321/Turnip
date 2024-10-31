using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class SetupPassData :
        ISceneColorPassData,
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public Color ClearColor { get; set; }
        public bool ShouldClearColor { get; set; }
        public bool ShouldClearDepth { get; set; }
    }
    
    public class CameraSetupPass : TurnipRenderPass<SetupPassData>
    {
        protected override void SetupPass(SetupPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.SceneColor = builder.WriteTexture(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor);
            data.SceneDepth = builder.WriteTexture(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth);
            data.ClearColor = cameraContext.Camera.backgroundColor;
            data.ShouldClearColor = cameraContext.Camera.clearFlags is CameraClearFlags.SolidColor or CameraClearFlags.Skybox;
            data.ShouldClearDepth = data.ShouldClearColor || cameraContext.Camera.clearFlags is CameraClearFlags.Depth;
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc<SetupPassData>(static (data, ctx) =>
            {
                ctx.cmd.SetRenderTarget(
                    data.SceneColor, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                    data.SceneDepth, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
                );
                ctx.cmd.ClearRenderTarget(data.ShouldClearDepth, data.ShouldClearDepth, data.ClearColor);
            });
        }
    }
}