using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Radish.Rendering.PassData
{
    public class PassDataManager
    {
        private Dictionary<Type, object> m_Data = new();
        private TurnipRenderPipeline m_Pipeline;
        
        public void BeginSetup(in TurnipRenderPassContext context)
        {
            m_Pipeline = context.Pipeline;
        }
        
        public void EndSetup()
        {
            foreach (var (type, data) in m_Data)
                m_Pipeline.RenderPassManager.SetData(type, data);
            
            m_Data.Clear();
        }
        
        [PublicAPI]
        public TPassData Use<TPassData>()
        {
            if (!typeof(TPassData).IsInterface)
                throw new ArgumentException($"{nameof(TPassData)} must be an interface");
            
            var data = m_Pipeline.RenderPassManager.GetData<TPassData>();
            Debug.Assert(data is not null);
            
            m_Data[typeof(TPassData)] = data;
            return data;
        }
    }
}