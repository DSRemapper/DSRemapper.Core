using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DSRemapper.Core.CDN
{
    /// <summary>
    /// Represents metadata about a DSRemapper component, which can be either a plugin or the main program.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="version">The version of the component.</param>
    /// <param name="coreVersion">The required version of the DSRemapper.Core library.</param>
    /// <param name="frameworkVersion">The required version of the DSRemapper.Framework library.</param>
    /// <param name="description">A description of the component.</param>
    public struct Manifest(string name, Version version, Version? coreVersion = null, Version? frameworkVersion = null, string description = "") : IComparable<Manifest>, IEquatable<Manifest>
    {
        /// <summary>
        /// Gets the OSPlatform instance representing all platforms.
        /// </summary>
        public static OSPlatform AllPlatforms { get => OSPlatform.Create("ALL"); }
        /// <summary>
        /// The name of the component.
        /// </summary>
        public readonly string Name { get => name; }
        /// <summary>
        /// A description of the component.
        /// </summary>
        public readonly string Description { get => description; }
        /// <summary>
        /// The version of the component.
        /// </summary>
        public readonly Version Version { get => version; }
        /// <summary>
        /// The required version of the DSRemapper.Core library.
        /// </summary>
        public readonly Version? CoreVersion { get => coreVersion; }
        /// <summary>
        /// The required version of the DSRemapper.Framework library.
        /// </summary>
        public readonly Version? FrameworkVersion { get => frameworkVersion; }
        /// <inheritdoc/>
        public readonly int CompareTo(Manifest other) => Version.CompareTo(other.Version);
        /// <inheritdoc/>
        public readonly bool Equals(Manifest other) => Name == other.Name && Version == other.Version;
        /// <inheritdoc/>
        public override readonly bool Equals(object? obj) => obj is Manifest manifest && Equals(manifest);
        /// <inheritdoc/>
        public override readonly int GetHashCode() => Version.GetHashCode();
        /// <summary>
        /// Gets a dictionary with the download links using the OS as a key
        /// </summary>        
        public Dictionary<OSPlatform, string> DownloadLinks { get; private set;} = [];
        
        /// <summary>
        /// Clears all OS-specific download links.
        /// </summary>
        public readonly void ClearOSDownloadLinks() => DownloadLinks.Clear();
        /// <summary>
        /// Removes the download link for a specific OS platform.
        /// </summary>
        /// <param name="platform">The OS platform to remove.</param>
        /// <returns>True if the element is successfully found and removed; otherwise, false.</returns>
        public readonly bool RemoveOSDownloadLink(OSPlatform platform) => DownloadLinks.Remove(platform);
        /// <summary>
        /// Sets or updates the download link for a specific OS platform.
        /// </summary>
        /// <param name="platform">The OS platform.</param>
        /// <param name="url">The download URL.</param>
        public readonly void SetOSDownloadLink(OSPlatform platform, string url){
            if (!DownloadLinks.TryAdd(platform, url))
                DownloadLinks[platform] = url;
        }
        /// <summary>
        /// Gets the download link for a specific OS platform, falling back to the "ALL" platform if not found.
        /// </summary>
        /// <param name="platform">The OS platform.</param>
        /// <returns>The download URL, or an empty string if not found.</returns>
        public readonly string GetOSDownloadLink(OSPlatform platform){
            if (DownloadLinks.TryGetValue(platform, out string? url))
                return url;
            if (DownloadLinks.TryGetValue(AllPlatforms, out string? allUrl))
                return allUrl;
            return "";
        }
        /// <summary>
        /// Gets the current OS platform based on runtime information.
        /// </summary>
        /// <returns>The <see cref="OSPlatform"/> corresponding to the current operating system, or <see cref="AllPlatforms"/> if no specific platform link is found.</returns>
        public readonly OSPlatform CurrentPlatform => DownloadLinks.Keys.FirstOrDefault(RuntimeInformation.IsOSPlatform, AllPlatforms);
        /// <summary>
        /// Gets the download link for the current OS.
        /// </summary>
        public readonly string CurrentOSDownloadLink => GetOSDownloadLink(CurrentPlatform);

        /// <summary>
        /// Checks if the component is supported by the specified version of DSRemapper.Core.
        /// </summary>
        /// <param name="coreVer">The version of DSRemapper.Core to check against.</param>
        /// <returns>True if the plugin is supported, otherwise false.</returns>
        public readonly bool IsSupportedByCore(Version coreVer){
            return CoreVersion == null || (coreVer >= CoreVersion &&
                coreVer.Major == CoreVersion.Major && coreVer.Minor == CoreVersion.Minor);
        }

        /// <summary>
        /// Checks if the component is supported by the specified version of DSRemapper.Framework.
        /// </summary>
        /// <param name="frameVer">The version of DSRemapper.Framework to check against.</param>
        /// <returns>True if the plugin is supported, otherwise false.</returns>
        public readonly bool IsSupportedByFramework(Version frameVer){
            return FrameworkVersion == null || (frameVer >= FrameworkVersion &&
                frameVer.Major == FrameworkVersion.Major && frameVer.Minor == FrameworkVersion.Minor);
        }

        /// <summary>
        /// Serializes the current <see cref="Manifest"/> instance to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the manifest.</returns>
        public readonly string SerializeToJson() => JsonSerializer.Serialize(this);
        /// <summary>
        /// Deserializes a JSON string into a <see cref="Manifest"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A new <see cref="Manifest"/> instance.</returns>
        public static Manifest FromJson(string json) => JsonSerializer.Deserialize<Manifest>(json);
        /// <summary>
        /// Deserializes a JSON stream into a <see cref="Manifest"/> instance.
        /// </summary>
        /// <param name="json">The JSON stream to deserialize.</param>
        /// <returns>A new <see cref="Manifest"/> instance.</returns>
        public static Manifest FromJson(Stream json) => JsonSerializer.Deserialize<Manifest>(json);
        
        /// <summary>
        /// Saves the current manifest to a specified file as a JSON string.
        /// </summary>
        /// <param name="file">The path to the file where the manifest will be saved.</param>
        public void SaveManifestToFile(string file) => File.WriteAllText(file, SerializeToJson());
        /// <inheritdoc/>
        public static bool operator ==(Manifest left, Manifest right) => left.Equals(right);
        /// <inheritdoc/>
        public static bool operator !=(Manifest left, Manifest right) => !(left == right);
        /// <inheritdoc/>
        public override readonly string ToString() => $"{Name}\n{Description}\nVersion: {Version}\nCore: {CoreVersion}\nFramework: {FrameworkVersion}";
    }

    /// <summary>
    /// A sorted collection of <see cref="Manifest"/> objects.
    /// </summary>
    public class ManifestCollection : SortedSet<Manifest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestCollection"/> class.
        /// </summary>
        private ManifestCollection() { }
        /// <summary>
        /// Deserializes a JSON string into a <see cref="ManifestCollection"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>        
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A new <see cref="ManifestCollection"/> instance.</returns>
        public static ManifestCollection FromJson(string json) => JsonSerializer.Deserialize<ManifestCollection>(json) ?? throw new NullReferenceException("Invalid JSON data for ManifestCollection");
        /// <summary>
        /// Deserializes a JSON stream into a <see cref="ManifestCollection"/> instance.
        /// </summary>
        /// <param name="json">The JSON stream to deserialize.</param>
        /// <exception cref="NullReferenceException">Thrown if the deserialized object is null.</exception>
        /// <returns>A new <see cref="ManifestCollection"/> instance.</returns>
        public static ManifestCollection FromJson(Stream json) => JsonSerializer.Deserialize<ManifestCollection>(json) ?? throw new NullReferenceException("Invalid JSON data for ManifestCollection");
        /// <summary>
        /// Serializes the current <see cref="ManifestCollection"/> instance to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the manifest.</returns>
        public string SerializeToJson() => JsonSerializer.Serialize(this);
        /// <summary>
        /// Gets the latest manifest that is compatible with the specified core and framework versions.
        /// </summary>
        /// <param name="core">The DSRemapper.Core version.</param>
        /// <param name="framework">The DSRemapper.Framework version.</param>
        /// <returns>The latest compatible <see cref="Manifest"/>.</returns>
        public Manifest GetLatestCompatibleVersion(Version core, Version framework) =>
            this.Last(man => man.IsSupportedByCore(core) && man.IsSupportedByFramework(framework));
        /// <summary>
        /// Saves the current manifest collection to a specified file as a JSON string.
        /// </summary>
        /// <param name="file">The path to the file where the manifest collection will be saved.</param>
        public void SaveManifestCollection(string file) => File.WriteAllText(file, SerializeToJson());
    }
}
