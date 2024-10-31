using System;
using Radish.Rendering.PassData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Scripting;

namespace Radish.Rendering
{
    [Serializable]
    public abstract class TurnipRenderPassBase
    {
        public abstract void AddToGraph(TurnipRenderPipeline pipeline, in CameraContext cameraContext);
    }
 
    [Serializable, RequireDerived]
    public abstract class TurnipRenderPass<TPassData> : TurnipRenderPassBase
        where TPassData : class, new()
    {
        private class AttachmentCopyPassData :
            ISceneColorPassData,
            ISceneDepthPassData,
            ICameraColorTexturePassData,
            ICameraDepthTexturePassData
        {
            public TextureHandle SceneColor { get; set; }
            public TextureHandle SceneDepth { get; set; }
            public TextureHandle CameraColorTexture { get; set; }
            public TextureHandle CameraDepthTexture { get; set; }
        }

        private static readonly int s_CameraColorTextureID = Shader.PropertyToID("_CameraColorTexture");
        private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");
        
        [LabelText("Name")]
        [SerializeField] private string m_PassName = "My Render Pass";

        private ProfilingSampler m_Sampler;
        private PassDataManager m_DataManager;

        protected virtual bool NeedsColorTexture => false;
        protected virtual bool NeedsDepthTexture => false;

        public override void AddToGraph(TurnipRenderPipeline pipeline, in CameraContext cameraContext)
        {
            m_Sampler ??= new ProfilingSampler(m_PassName);
            m_DataManager ??= new PassDataManager();

            var passContext = new TurnipRenderPassContext(pipeline, m_DataManager);

            if (ShouldCullPass(in passContext, in cameraContext))
                return;
            
            m_DataManager.BeginSetup(in passContext);
            
            if (NeedsColorTexture || NeedsDepthTexture)
                InjectAttachmentCopyPass(in passContext, in cameraContext);

            var builder = passContext.Pipeline.Graph.AddRenderPass<TPassData>(m_PassName, out var passData, m_Sampler);
            try
            {
                if (NeedsColorTexture && passData is ICameraColorTexturePassData colorTex)
                {
                    colorTex.CameraColorTexture = builder.ReadTexture(passContext.PassManager
                        .GetData<ICameraColorTexturePassData>().CameraColorTexture);
                }

                if (NeedsDepthTexture && passData is ICameraDepthTexturePassData depthTex)
                {
                    depthTex.CameraDepthTexture = builder.ReadTexture(passContext.PassManager
                        .GetData<ICameraDepthTexturePassData>().CameraDepthTexture);
                }
                
                SetupPass(passData, in passContext, in cameraContext, ref builder);
                passContext.PassManager.CollectDataInterfacesForData(passData);
            }
            finally
            {
                builder.Dispose();
            }
            
            m_DataManager.EndSetup();
        }
        
        

        private void InjectAttachmentCopyPass(in TurnipRenderPassContext passContext, in CameraContext cameraContext)
        {
            using var builder =
                passContext.Pipeline.Graph.AddRenderPass<AttachmentCopyPassData>("Attachment Copy", out var passData,
                    m_Sampler);

            passData.SceneColor =
                builder.ReadTexture(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor);
            passData.SceneDepth =
                builder.ReadTexture(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth);

            passData.CameraColorTexture =
                builder.WriteTexture(passContext.PassManager.GetData<ICameraColorTexturePassData>().CameraColorTexture);
            passData.CameraDepthTexture =
                builder.WriteTexture(passContext.PassManager.GetData<ICameraDepthTexturePassData>().CameraDepthTexture);
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc<AttachmentCopyPassData>(static (data, ctx) =>
            {
                ctx.cmd.CopyTexture(data.SceneColor, data.CameraColorTexture);
                ctx.cmd.CopyTexture(data.SceneDepth, data.CameraDepthTexture);
                
                ctx.cmd.SetGlobalTexture(s_CameraColorTextureID, data.CameraColorTexture);
                ctx.cmd.SetGlobalTexture(s_CameraDepthTextureID, data.CameraDepthTexture);
            });
            
            passContext.PassManager.CollectDataInterfacesForData(passData);
        }

        protected abstract void SetupPass(TPassData data, in TurnipRenderPassContext passContext,
            in CameraContext cameraContext, ref RenderGraphBuilder builder);

        protected virtual bool ShouldCullPass(in TurnipRenderPassContext passContext,
            in CameraContext cameraContext)
        {
            return false;
        }
    }
}