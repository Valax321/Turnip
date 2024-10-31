using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class BackgroundPassData : 
        ISceneColorPassData, 
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public RendererListHandle Renderers { get; set; }
    }
    
    public sealed class BackgroundPass : TurnipRenderPass<BackgroundPassData>
    {
        protected override void SetupPass(BackgroundPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.SceneColor =
                builder.UseColorBuffer(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.Read);

            var renderers = passContext.Graph.CreateSkyboxRendererList(in cameraContext.Camera);
            data.Renderers = builder.UseRendererList(in renderers);
            
            builder.AllowRendererListCulling(false);

            builder.SetRenderFunc<BackgroundPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
        }

        protected override bool ShouldCullPass(in TurnipRenderPassContext passContext, in CameraContext cameraContext)
        {
            return cameraContext.Camera.clearFlags != CameraClearFlags.Skybox;
        }
    }
}