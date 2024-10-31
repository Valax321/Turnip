using Radish.Rendering.PassData;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public readonly struct TurnipRenderPassContext
    {
        public readonly TurnipRenderPipeline Pipeline;
        public RenderPassManager PassManager => Pipeline.RenderPassManager;
        public RenderGraph Graph => Pipeline.Graph;
        public readonly PassDataManager DataManager;

        public TurnipRenderPassContext(TurnipRenderPipeline pipeline, PassDataManager dataManager)
        {
            Pipeline = pipeline;
            DataManager = dataManager;
        }
    }
}