using System.Reflection;

namespace DSRemapper.Core
{
    /// <summary>
    /// A class containing all the important folder paths for DSRemapper
    /// </summary>
    public static class DSRPaths
    {
        /// <summary>
        /// Executing folder of the DSRemapper app
        /// </summary>
        public readonly static DirectoryInfo ProgramPath = new(AppContext.BaseDirectory);// Path.GetDirectoryName(ExePath) ?? "";
        /// <summary>
        /// Folder containing all the DSRemapper plugins
        /// </summary>
        public readonly static DirectoryInfo PluginsPath = ProgramPath.CreateSubdirectory("Plugins");
        /// <summary>
        /// DSRemapper folder located inside users document folder (contains app and plugins configurations and remap profiles)
        /// </summary>
        public readonly static DirectoryInfo FolderPath = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DSRemapper"));
        /// <summary>
        /// Folder containing all remap profiles recogniced by the app.
        /// </summary>
        public readonly static DirectoryInfo ProfilesPath = FolderPath.CreateSubdirectory("Profiles");
        /// <summary>
        /// Folder containing DSRemapper app settings.
        /// </summary>
        public readonly static DirectoryInfo ConfigPath = FolderPath.CreateSubdirectory("Configs");
        /// <summary>
        /// Gets a <see cref="DirectoryInfo"/> relative to another <see cref="DirectoryInfo"/> object.
        /// </summary>
        /// <param name="parent">The base directory that will be used for the relative path</param>
        /// <param name="name">The relative path for the sub-directory</param>
        /// <returns>A <see cref="FileInfo"/> representing the file</returns>
        public static DirectoryInfo GetSubdirectory(this DirectoryInfo parent, string name) => new(Path.Combine(parent.FullName, name));
        /// <summary>
        /// Gets a <see cref="FileInfo"/> relative to a <see cref="DirectoryInfo"/> object.
        /// </summary>
        /// <param name="dir">The base directory that will be used for the relative path</param>
        /// <param name="fileName">The relative path for the file</param>
        /// <returns>A <see cref="FileInfo"/> representing the file</returns>
        public static FileInfo GetFile(this DirectoryInfo dir, string fileName) => new(Path.Combine(dir.FullName, fileName));
    }
}