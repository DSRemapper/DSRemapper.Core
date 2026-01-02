using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DSRemapper.Core.CDN
{
    /// <summary>
    /// Represents information about a download, including the link and hash.
    /// </summary>
    /// <param name="downloadLink">The URL to download the file.</param>
    /// <param name="hash">The hash of the file for verification.</param>
    public struct DownloadInfo(string downloadLink, string hash)
    {
        /// <summary>
        /// The URL to download the file.
        /// </summary>
        public string DownloadLink { get; init; } = downloadLink;
        /// <summary>
        /// The hash of the file for verification.
        /// </summary>
        public string Hash { get; init; } = hash;
    }
    /// <summary>
    /// Represents metadata about a DSRemapper component, which can be either a plugin or the main program.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="version">The version of the component.</param>
    /// <param name="coreVersion">The required version of the DSRemapper.Core library.</param>
    /// <param name="frameworkVersion">The required version of the DSRemapper.Framework library.</param>
    /// <param name="description">A description of the component.</param>
    public class Manifest(string name, Version version, Version? coreVersion = null, Version? frameworkVersion = null, string description = "") : IComparable<Manifest>, IEquatable<Manifest>
    {
        /// <summary>
        /// Gets the OSPlatform instance representing all platforms.
        /// </summary>
        public static OSPlatform AllPlatforms { get => OSPlatform.Create("ALL"); }
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Name { get; init; } = name;
        /// <summary>
        /// A description of the component.
        /// </summary>
        public string Description { get; init; } = description;
        /// <summary>
        /// The version of the component.
        /// </summary>
        public Version Version { get; init; } = version;
        /// <summary>
        /// The required version of the DSRemapper.Core library.
        /// </summary>
        public Version? CoreVersion { get; init; } = coreVersion;
        /// <summary>
        /// The required version of the DSRemapper.Framework library.
        /// </summary>
        public Version? FrameworkVersion { get; init; } = frameworkVersion;
        /// <inheritdoc/>
        public int CompareTo(Manifest? other) => Version.CompareTo(other?.Version);
        /// <inheritdoc/>
        public bool Equals(Manifest? other) => Name == other?.Name && Version == other.Version;
        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Manifest manifest && Equals(manifest);
        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();
        /// <summary>
        /// Gets a dictionary with the download links using the OS as a key
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(OSPlatformDictionaryConverter))]
        public Dictionary<OSPlatform, DownloadInfo> DownloadLinks { get; private set; } = [];
        /// <summary>
        /// Sets the download links for the component.
        /// </summary>
        /// <param name="links">A dictionary containing the download links for different OS platforms.</param>
        public void SetDownloadLinks(Dictionary<OSPlatform, DownloadInfo> links) => DownloadLinks = links;
        /// <summary>
        /// Clears all OS-specific download links.
        /// </summary>
        public void ClearOSDownloadLinks() => DownloadLinks.Clear();
        /// <summary>
        /// Removes the download link for a specific OS platform.
        /// </summary>
        /// <param name="platform">The OS platform to remove.</param>
        /// <returns>True if the element is successfully found and removed; otherwise, false.</returns>
        public bool RemoveOSDownloadLink(OSPlatform platform) => DownloadLinks.Remove(platform);
        /// <summary>
        /// Sets or updates the download link for a specific OS platform.
        /// </summary>
        /// <param name="platform">The OS platform.</param>
        /// <param name="info">The download information containing the url and hash.</param>
        public void SetOSDownloadLink(OSPlatform platform, DownloadInfo info)
        {
            if (!DownloadLinks.TryAdd(platform, info))
                DownloadLinks[platform] = info;
        }
        /// <summary>
        /// Gets the download link for a specific OS platform, falling back to the "ALL" platform if not found.
        /// </summary>
        /// <param name="platform">The OS platform.</param>
        /// <returns>The download URL, or an empty string if not found.</returns>
        public DownloadInfo GetOSDownloadLink(OSPlatform platform)
        {
            if (DownloadLinks.TryGetValue(platform, out DownloadInfo info))
                return info;
            if (DownloadLinks.TryGetValue(AllPlatforms, out DownloadInfo allInfo))
                return allInfo;
            return new("","");
        }
        /// <summary>
        /// Gets the current OS platform based on runtime information.
        /// </summary>
        /// <returns>The <see cref="OSPlatform"/> corresponding to the current operating system, or <see cref="AllPlatforms"/> if no specific platform link is found.</returns>
        [JsonIgnore]
        public OSPlatform CurrentPlatform => DownloadLinks.Keys.FirstOrDefault(RuntimeInformation.IsOSPlatform, AllPlatforms);
        /// <summary>
        /// Gets the download link for the current OS.
        /// </summary>
        [JsonIgnore]
        public DownloadInfo CurrentOSDownloadLink => GetOSDownloadLink(CurrentPlatform);

        /// <summary>
        /// Checks if the component is supported by the specified version of DSRemapper.Core.
        /// </summary>
        /// <param name="coreVer">The version of DSRemapper.Core to check against.</param>
        /// <returns>True if the plugin is supported, otherwise false.</returns>
        public bool IsSupportedByCore(Version coreVer)
        {
            return CoreVersion == null || (coreVer >= CoreVersion &&
                coreVer.Major == CoreVersion.Major && coreVer.Minor == CoreVersion.Minor);
        }

        /// <summary>
        /// Checks if the component is supported by the specified version of DSRemapper.Framework.
        /// </summary>
        /// <param name="frameVer">The version of DSRemapper.Framework to check against.</param>
        /// <returns>True if the plugin is supported, otherwise false.</returns>
        public bool IsSupportedByFramework(Version frameVer)
        {
            return FrameworkVersion == null || (frameVer >= FrameworkVersion &&
                frameVer.Major == FrameworkVersion.Major && frameVer.Minor == FrameworkVersion.Minor);
        }

        /// <summary>
        /// Serializes the current <see cref="Manifest"/> instance to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the manifest.</returns>
        public string SerializeToJson(bool simpleMode = false) => simpleMode ? JsonSerializer.Serialize(this, GetSimpleManifestOptions()) : JsonSerializer.Serialize(this);
        /// <summary>
        /// Deserializes a JSON string into a <see cref="Manifest"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A new <see cref="Manifest"/> instance.</returns>
        public static Manifest FromJson(string json) => JsonSerializer.Deserialize<Manifest>(json)!;
        /// <summary>
        /// Deserializes a JSON stream into a <see cref="Manifest"/> instance.
        /// </summary>
        /// <param name="json">The JSON stream to deserialize.</param>
        /// <returns>A new <see cref="Manifest"/> instance.</returns>
        public static Manifest FromJson(Stream json) => JsonSerializer.Deserialize<Manifest>(json)!;

        /// <summary>
        /// Saves the current manifest to a specified file as a JSON string.
        /// </summary>
        /// <param name="file">The path to the file where the manifest will be saved.</param>
        public void SaveManifestToFile(string file) => File.WriteAllText(file, SerializeToJson());
        /// <inheritdoc/>
        public static bool operator ==(Manifest? left, Manifest? right) => left?.Equals(right) ?? false;
        /// <inheritdoc/>
        public static bool operator !=(Manifest? left, Manifest? right) => !(left == right);
        /// <inheritdoc/>
        public override string ToString() => $"{Name}\n{Description}\nVersion: {Version}\nCore: {CoreVersion}\nFramework: {FrameworkVersion}";

        internal static JsonSerializerOptions GetSimpleManifestOptions()
        {
            return new()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = {
                        typeInfo => {
                            foreach (var property in typeInfo.Properties)
                                if (property.PropertyType == typeof(Dictionary<OSPlatform, DownloadInfo>))
                                    property.ShouldSerialize = (obj, val) => false;
                        }
                    }
                }
            };
        }
    }

    /// <summary>
    /// A sorted collection of <see cref="Manifest"/> objects.
    /// </summary>
    public class ManifestCollection : SortedSet<Manifest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestCollection"/> class.
        /// </summary>
        public ManifestCollection() { }
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
        public Manifest? GetLatestCompatibleVersion(Version core, Version framework) =>
            this.LastOrDefault(man => man.IsSupportedByCore(core) && man.IsSupportedByFramework(framework));
        /// <summary>
        /// Saves the current manifest collection to a specified file as a JSON string.
        /// </summary>
        /// <param name="file">The path to the file where the manifest collection will be saved.</param>
        public void SaveManifestCollection(string file) => File.WriteAllText(file, SerializeToJson());
    }

    /// <summary>
    /// Custom converter class for the Manifest class links dictionary
    /// </summary>
    internal class OSPlatformDictionaryConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType || typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                return false;

            return typeToConvert.GetGenericArguments()[0] == typeof(OSPlatform);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
            (JsonConverter)Activator.CreateInstance(typeof(OSPlatformDictionaryInternal<>)
                .MakeGenericType(typeToConvert.GetGenericArguments()[1]))!;

        private class OSPlatformDictionaryInternal<TValue> : JsonConverter<Dictionary<OSPlatform, TValue>>
        {
            public OSPlatformDictionaryInternal() { }
            public override Dictionary<OSPlatform, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dict = new Dictionary<OSPlatform, TValue>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject) return dict;

                    // Get the Key
                    var key = OSPlatform.Create((reader.GetString() ?? throw new JsonException()).ToUpper());

                    // Get the Value
                    reader.Read();
                    var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
                    if (value != null) dict.Add(key, value);
                }
                return dict;
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<OSPlatform, TValue> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                foreach (var kvp in value)
                {
                    writer.WritePropertyName(kvp.Key.ToString());
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
            }
        }
    }
}
