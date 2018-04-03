﻿using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;

namespace UnityEditor.AddressableAssets
{
    public struct WriteVirtualBundleDataTask : IBuildTask
    {
        const int k_Version = 1;
        public int Version { get { return k_Version; } }

        static readonly Type[] k_RequiredTypes = { typeof(IAddressableAssetsBuildContext), typeof(IBundleWriteData) };
        public Type[] RequiredContextTypes { get { return k_RequiredTypes; } }

        public ReturnCode Run(IBuildContext context)
        {
            var aaContext = context.GetContextObject<IAddressableAssetsBuildContext>() as AddressableAssetsBuildContext;
            return Run(aaContext.m_settings, aaContext.m_runtimeData, aaContext.m_contentCatalog, context.GetContextObject<IBundleWriteData>());
        }

        public static ReturnCode Run(AddressableAssetSettings aaSettings, ResourceManagerRuntimeData runtimeData, ResourceLocationList contentCatalog, IBundleWriteData writeData)
        {
            var virtualBundleData = new VirtualAssetBundleRuntimeData(aaSettings.buildSettings.localLoadSpeed, aaSettings.buildSettings.remoteLoadSpeed);
            var bundledAssets = new Dictionary<string, List<string>>();
            foreach (var loc in contentCatalog.locations)
            {
                if (loc.m_provider == typeof(BundledAssetProvider).FullName)
                {
                    if (loc.m_dependencies == null || loc.m_dependencies.Length == 0)
                        continue;
                    foreach (var dep in loc.m_dependencies)
                    {
                        List<string> assetsInBundle = null;
                        if (!bundledAssets.TryGetValue(dep, out assetsInBundle))
                            bundledAssets.Add(dep, assetsInBundle = new List<string>());
                        assetsInBundle.Add(loc.m_internalId);
                    }
                }
            }

            foreach (var bd in bundledAssets)
            {
                var bundleLocData = contentCatalog.locations.Find(a => a.m_address == bd.Key);
                uint size = (uint)(bd.Value.Count * 1024 * 1024); //for now estimate 1MB per entry
                virtualBundleData.AssetBundles.Add(new VirtualAssetBundle(bundleLocData.m_internalId, bundleLocData.m_provider == typeof(LocalAssetBundleProvider).FullName, size, bd.Value));
            }
            virtualBundleData.Save();
            return ReturnCode.Success;
        }
    }
}