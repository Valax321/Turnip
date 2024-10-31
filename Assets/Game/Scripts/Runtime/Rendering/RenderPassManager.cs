using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public class RenderPassManager
    {
        public struct DataInitContext
        {
            public Camera Camera { get; internal set; }
            public RenderPassManager RenderPassManager { get; internal set; }
            public RenderGraph Graph => RenderPassManager.RenderGraph;
        }
        
        public TurnipRenderPipeline Pipeline { get; }
        public RenderGraph RenderGraph { get; }

        private readonly Dictionary<Type, object> m_LastImplementorOfDataType = new();

        private readonly Dictionary<Type, Func<DataInitContext, object>> m_DataInitializers = new();
        
        private static readonly Dictionary<Type, List<Type>> s_InterfaceMap = new();

        private Camera m_ActiveCamera;
        
        public RenderPassManager(TurnipRenderPipeline pipeline, RenderGraph graph)
        {
            Pipeline = pipeline;
            RenderGraph = graph;
        }

        public void BeginCameraFrame(Camera camera)
        {
            m_ActiveCamera = camera;
        }

        public void EndCameraFrame()
        {
            m_LastImplementorOfDataType.Clear();
            m_ActiveCamera = null;
        }

        [PublicAPI]
        public void SetDataInitializer<TPassData>(Func<DataInitContext, TPassData> initFunc)
        {
            if (!typeof(TPassData).IsInterface)
                throw new ArgumentException($"{nameof(TPassData)} must be an interface");

            m_DataInitializers[typeof(TPassData)] = c => initFunc(c);
        }

        [PublicAPI]
        public TPassData GetData<TPassData>()
        {
            if (m_ActiveCamera is null)
                throw new NullReferenceException("Active camera is null");
            
            if (!typeof(TPassData).IsInterface)
                throw new ArgumentException($"{nameof(TPassData)} must be an interface");
            
            if (!m_LastImplementorOfDataType.TryGetValue(typeof(TPassData), out var data))
            {
                if (m_DataInitializers.TryGetValue(typeof(TPassData), out var initFunc))
                {
                    data = initFunc(new DataInitContext
                    {
                        Camera = m_ActiveCamera,
                        RenderPassManager = this
                    });
                    Debug.Assert(data is TPassData);
                    m_LastImplementorOfDataType[typeof(TPassData)] = data;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }

            return (TPassData)data;
        }

        [PublicAPI]
        public void SetData(Type interfaceType, object data)
        {
            m_LastImplementorOfDataType[interfaceType] = data;
        }

        internal void CollectDataInterfacesForData<TPassData>(TPassData passData) where TPassData : class, new()
        {
            var interfaces = ExtractInterfaces<TPassData>();
            foreach (var i in interfaces)
            {
                m_LastImplementorOfDataType[i] = passData;
            }
        }

        private static List<Type> ExtractInterfaces<TPassData>() where TPassData : class, new()
        {
            if (!s_InterfaceMap.TryGetValue(typeof(TPassData), out var interfaces))
            {
                interfaces = typeof(TPassData).GetInterfaces()
                    .Where(x => x.GetCustomAttribute<PassDataProviderAttribute>() is not null).ToList();
                s_InterfaceMap.Add(typeof(TPassData), interfaces);
            }

            return interfaces;
        }
        
        private static List<Type> ExtractInterfaces(Type t)
        {
            if (!s_InterfaceMap.TryGetValue(t, out var interfaces))
            {
                interfaces = t.GetInterfaces()
                    .Where(x => x.GetCustomAttribute<PassDataProviderAttribute>() is not null).ToList();
                s_InterfaceMap.Add(t, interfaces);
            }

            return interfaces;
        }
    }
}