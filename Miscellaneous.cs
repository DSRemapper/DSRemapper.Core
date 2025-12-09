using System.Text.Json;

namespace DSRemapper.Core
{
    /// <summary>
    /// Represents the manifest of a DSRemapper plugin, containing metadata about the plugin.
    /// </summary>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="pluginVer">The version of the plugin.</param>
    /// <param name="coreVer">The required version of the DSRemapper.Core library.</param>
    /// <param name="frameVer">The required version of the DSRemapper.Framework library.</param>
    public struct PluginManifest(string name, Version pluginVer, Version? coreVer = null, Version? frameVer = null){
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// The version of the plugin.
        /// </summary>
        public Version PluginVersion { get => pluginVer; }
        /// <summary>
        /// The required version of the DSRemapper.Core library.
        /// </summary>
        public Version? CoreVersion { get => coreVer; }
        /// <summary>
        /// The required version of the DSRemapper.Framework library.
        /// </summary>
        public Version? FrameworkVersion { get => frameVer; }

        /// <summary>
        /// Checks if the plugin is supported by the specified version of DSRemapper.Core.
        /// </summary>
        /// <param name="coreVer">The version of DSRemapper.Core to check against.</param>
        /// <returns>True if the plugin is supported, otherwise false.</returns>
        public bool IsSupportedByCore(Version coreVer){
            return CoreVersion == null || coreVer >= CoreVersion;
        }

        /// <summary>
        /// Checks if the plugin is supported by the specified version of DSRemapper.Framework.
        /// </summary>
        /// <param name="frameVer">The version of DSRemapper.Framework to check against.</param>
        /// <returns>True if the plugin is supported, otherwise false.</returns>
        public bool IsSupportedByFramework(Version frameVer){
            return FrameworkVersion == null || frameVer >= FrameworkVersion;
        }

        /// <summary>
        /// Serializes the current <see cref="PluginManifest"/> instance to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the manifest.</returns>
        public string SerializeToJson() => JsonSerializer.Serialize(this);
        /// <summary>
        /// Deserializes a JSON string into a <see cref="PluginManifest"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A new <see cref="PluginManifest"/> instance.</returns>
        public static PluginManifest FromJson(string json) => JsonSerializer.Deserialize<PluginManifest>(json);
        /// <summary>
        /// Deserializes a JSON stream into a <see cref="PluginManifest"/> instance.
        /// </summary>
        /// <param name="json">The JSON stream to deserialize.</param>
        /// <returns>A new <see cref="PluginManifest"/> instance.</returns>
        public static PluginManifest FromJson(Stream json) => JsonSerializer.Deserialize<PluginManifest>(json);
    }
}