using Radish.Rendering.PassData;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class CameraTargetBlitPassData :
        ISceneColorPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle CameraTarget { get; set; }
    }
    
    public sealed class CameraTargetBlitPass : TurnipRenderPass<CameraTargetBlitPassData>
    {
        protected override void SetupPass(CameraTargetBlitPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.SceneColor = builder.ReadTexture(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor);
            data.CameraTarget = passContext.Graph.ImportBackbuffer(BuiltinRenderTextureType.CameraTarget);
            
            builder.SetRenderFunc<CameraTargetBlitPassData>(static (data, context) =>
            {
                context.cmd.Blit(data.SceneColor, data.CameraTarget);
            });
        }
    }
}