using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace UnityEditor.AddressableAssets.Build
{
    /// <summary>
    /// Contains information about the status of the build.
    /// </summary>
    public class AddressableAssetBuildResult : IDataBuilderResult
    {
        /// <summary>
        /// Duration of build, in seconds.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// The number of addressable assets contained in the build.
        /// </summary>
        public int LocationCount { get; set; }

        /// <summary>
        /// Error that caused the build to fail.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Path of runtime settings file
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Registry of files created during the build
        /// </summary>
        public FileRegistry FileRegistry { get; set; }

        /// <summary>
        /// Helper method to create the desired result of a data builder.  This should always be used to create the build result
        ///  with additional details added as needed.  The Result.Duration should always be set at the end of the build
        ///  script in the non-error scenario.
        /// </summary>
        /// <param name="settingsPath">Path to the settings.json file (name may not always match that exactly) generated by this build</param>
        /// <param name="locCount">Number of locations created by this build</param>
        /// <param name="err">Error string if there were problems with the build.  Defaults to empty</param>
        /// <typeparam name="TResult">The actual build result created</typeparam>
        /// <returns></returns>
        public static TResult CreateResult<TResult>(string settingsPath, int locCount, string err = "") where TResult : IDataBuilderResult
        {
            var opResult = Activator.CreateInstance<TResult>();
            opResult.OutputPath = settingsPath;
            opResult.Duration = 0;
            opResult.Error = err;
            opResult.LocationCount = locCount;
            return opResult;
        }

        /// <summary>
        /// Helper method to create the desired result of a data builder.  This should always be used to create the build result
        ///  with additional details added as needed.  The Result.Duration should always be set at the end of the build
        ///  script in the non-error scenario.  Other results should be set as available.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult CreateResult<TResult>() where TResult : IDataBuilderResult
        {
            var opResult = Activator.CreateInstance<TResult>();
            return opResult;
        }
    }

    /// <summary>
    /// Build result for entering play mode in the editor.
    /// </summary>
    [Serializable]
    public class AddressablesPlayModeBuildResult : AddressableAssetBuildResult
    {
    }

    /// <summary>
    /// Build result for building the Addressables content.
    /// </summary>
    public class AddressablesPlayerBuildResult : AddressableAssetBuildResult
    {
        internal List<BundleBuildResult> m_AssetBundleBuildResults = new List<BundleBuildResult>();

        /// <summary>
        /// Information about a bundle build results
        /// </summary>
        [System.Serializable]
        public class BundleBuildResult
        {
            /// <summary>
            /// The file path of the bundle.
            /// </summary>
            public string FilePath;
            /// <summary>
            /// The internal AssetBundle name, this is not always the same as the file name.
            /// </summary>
            public string InternalBundleName;
            /// <summary>
            /// The Addressable Group that was responsible for generating a given AssetBundle
            /// </summary>
            public AddressableAssetGroup SourceAssetGroup;
            /// <summary>
            /// The calculated value used for security during cyclic redundancy checks
            /// </summary>
            public uint Crc;
            /// <summary>
            /// The asset hash of the assets included inside the AssetBundle
            /// </summary>
            public string Hash;
        }

        /// <summary>
        /// True if the build was doing an update to a previous build, else false.
        /// </summary>
        public bool IsUpdateContentBuild { get; internal set; }

        /// <summary>
        /// Build results for AssetBundles created during the build.
        /// </summary>
        public List<BundleBuildResult> AssetBundleBuildResults => m_AssetBundleBuildResults;

        /// <summary>
        /// File path to the generated remote catalog hash file.
        /// </summary>
        public string RemoteCatalogHashFilePath { get; internal set; }

        /// <summary>
        /// File path to the generated remote catalog json file.
        /// </summary>
        public string RemoteCatalogJsonFilePath { get; internal set; }

        /// <summary>
        /// File path to the generate content state file
        /// </summary>
        public string ContentStateFilePath { get; internal set; }
    }
}
