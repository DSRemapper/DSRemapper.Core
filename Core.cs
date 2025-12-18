using DSRemapper.Types;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Linq.Expressions;

namespace DSRemapper.Core
{

    /// <summary>
    /// Delegate for Remappers message events
    /// </summary>
    /// <param name="sender">The object that sends the message to the device console</param>
    /// <param name="message">A string containing the message sent by the Remapper object</param>
    /// <param name="level">The level of the current message</param>
    public delegate void DeviceConsoleEventArgs(object sender, string message, LogLevel level);
    /*/// <summary>
    /// Delegate for Remappers device console message events.
    /// </summary>
    /// <param name="sender">The object that sends the message to the device console</param>
    /// <param name="message">A string containing the message sent to the device console</param>
    public delegate void ControllerConsoleEventArgs(object sender, string message);*/

    /// <summary>
    /// Attribute to bind Remappers to their corresponding file extensions
    /// </summary>
    /// <param name="fileExts">The file extensions (without the dot) binded to the Remapper class</param>
    [AttributeUsage(AttributeTargets.Class)]
    public class RemapperAttribute(string[] fileExts) : Attribute
    {
        /// <summary>
        /// The extensions of the file referenced by this attribute
        /// </summary>
        public string[] FileExts { get; } = fileExts;
    }

    /// <summary>
    /// An attribute to reference a emulated controller with a unique id/path to instance it.
    /// </summary>
    /// <param name="path">The id/path for the emulated controller</param>
    /// <param name="isGlobal">If is true the emulated controller will not be referenced to a specific Remapper (default value: false) [still not implemented]</param>
    [AttributeUsage(AttributeTargets.Class)]
    public class EmulatedControllerAttribute(string path, bool isGlobal = false) : Attribute
    {
        /// <summary>
        /// Gets/Sets the virtual device id/path
        /// </summary>
        public string DevicePath { get; } = path;
        /// <summary>
        /// Gets/Sets if the controller is global or not
        /// </summary>
        public bool IsGlobal { get; } = isGlobal;
    }

    /// <summary>
    /// Mark a method as a Custom Method (not included on the <see cref="IDSRInputController"/> interface) to be accessed from DSRemapper.Framework
    /// </summary>
    /// <param name="internalName">Friendly name for the method</param>
    /// <param name="scriptOnly">Mark the function to be accessed only from code. In the case of true, for example, the function will not be showed in user graphical interfaces like DSRemapperApp, but it can be accessed from a remapper plugin.</param>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomMethodAttribute(string internalName, bool scriptOnly = false) : Attribute
    {
        /// <summary>
        /// Gets/Sets the method friendly name
        /// </summary>
        public string InternalName { get; } = internalName;
        /// <summary>
        /// Gets/Sets if the function will be visible on GUIs
        /// </summary>
        public bool ScriptOnly { get; } = scriptOnly;
    }


    #region Plugins Interfaces
    /// <summary>
    /// A standard interface for device informations handled by DSRemapper
    /// </summary>
    public interface IDSRInputDeviceInfo
    {
        /// <summary>
        /// Unique id for the device referenced by this interface.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Friendly name for the device referenced by this interface.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Creates a IDSRInputController that references the physical controller referenced by this interface.
        /// Works as a shortcut to the corresponding controller constructor to simplify the program code.
        /// </summary>
        /// <returns>A initialized IDSRInputController object</returns>
        public IDSRInputController CreateController();
        /// <summary>
        /// Returns de device name and id as a string with a default format.
        /// </summary>
        /// <returns>A string with device name and id</returns>
        public virtual string? ToString() => $"Device {Name} [{Id}]";
    }
    
    /// <summary>
    /// A standard Device Scanner interface for DSRemapper
    /// </summary>
    public interface IDSRDeviceScanner
    {
        /// <summary>
        /// Returns an array with the information of connected devices for a specific plugin or controller type.
        /// </summary>
        /// <returns>An array of IDSRInputDeviceInfo objects</returns>
        public IDSRInputDeviceInfo[] ScanDevices();
        /// <summary>
        /// Makes some plugins able to clear any unmanaged data from the memory or other finalizing options.
        /// </summary>
        public static virtual void PluginFree() { }
    }
    
    /// <summary>
    /// Standard input controller of DSRemapper
    /// </summary>
    public interface IDSRInputController : IDisposable
    {
        /// <summary>
        /// Gets the controller Id
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Gets the controller name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the controller type
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// Gets the controller info string to show relevant information of it (for example, battery level of Dualshock 4).
        /// Try to keep it on ONE or TWO lines, otherwise information container can expand more than expected.
        /// </summary>
        virtual public string Info { get => ""; }
        /// <summary>
        /// Gets the relative path (from the plugin dll) to the image that represent the physical controller
        /// </summary>
        virtual public string ImgPath { get => "UnknownController.png"; }
        /// <summary>
        /// Gets if the controller is currently connected
        /// </summary>
        public bool IsConnected { get; }
        /// <summary>
        /// Connects the controller to start reading and writing from/to it
        /// </summary>
        public void Connect();
        /// <summary>
        /// Disconnects the controller from DSRemapper
        /// </summary>
        public void Disconnect();
        /// <summary>
        /// Gets the input state of the controller, which includes axes positions, buttons, etc.
        /// </summary>
        /// <returns>A standard DSRemapper input report</returns>
        public IDSRInputReport GetInputReport();
        /// <summary>
        /// Sets the output state of a controller sending information about vibration, force feedback, etc.
        /// </summary>
        /// <param name="report">A standard DSRemapper output report with the information for the controller</param>
        public void SendOutputReport(IDSROutputReport report);
    }
    
    /// <summary>
    /// Standard interface for DSRemapper emulated controllers
    /// </summary>
    public interface IDSROutputController : IDisposable
    {
        /// <summary>
        /// Custom methods outside the interface that are defined by the controller
        /// </summary>
        /// <returns>a dictionary with the delegates of the custom methods</returns>
        virtual public Dictionary<string, Delegate> CustomMethods { get {
            var customMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, Delegate> delegates = new(customMethods
                .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(CustomMethodAttribute)))
                .Select(m =>
                {
                    CustomMethodAttribute? attr = m.GetCustomAttribute<CustomMethodAttribute>();
                    List<Type> parameterTypes = [.. m.GetParameters().Select(p => p.ParameterType)];
                    parameterTypes.Add(m.ReturnType);
                    Type delegateType = Expression.GetDelegateType([.. parameterTypes]);
                    return new KeyValuePair<string, Delegate>(attr?.InternalName ?? m.Name, m.CreateDelegate(delegateType, this));
                }));
            //DSRLogger.StaticLogInformation($"Custom methods found: {string.Join(", ",delegates.Keys)}");
            return delegates;
        } }
        /// <summary>
        /// Gets if the emulated controller is connected and updating it's data
        /// </summary>
        public bool IsConnected { get; }
        /// <summary>
        /// Gets the state structure of the emulated controller
        /// </summary>
        public IDSRInputReport State { get; set; }
        /// <summary>
        /// Default Connect function to connect the emulated controller
        /// </summary>
        public void Connect();
        /// <summary>
        /// Disconnects the emulated controller
        /// </summary>
        public void Disconnect();
        /// <summary>
        /// Updates the emulated controller values using the state property values
        /// </summary>
        public void Update();
        /// <summary>
        /// Gets the current state of the feedback sended to the emulated controller from the computer
        /// </summary>
        /// <returns>A standard DSRemapper output report</returns>
        public IDSROutputReport GetFeedbackReport();
                
        /*int IReadOnlyCollection<KeyValuePair<string, Delegate>>.Count => CustomMethods.Count;
        virtual int Count => CustomMethods.Count;
        IEnumerable<string> IReadOnlyDictionary<string, Delegate>.Keys => CustomMethods.Keys;
        virtual IEnumerable<string> Keys => CustomMethods.Keys;
        IEnumerable<Delegate> IReadOnlyDictionary<string, Delegate>.Values => CustomMethods.Values;
        virtual IEnumerable<Delegate> Values => CustomMethods.Values;

        bool IReadOnlyDictionary<string, Delegate>.ContainsKey(string key) => 
            CustomMethods.ContainsKey(key);
        virtual bool ContainsKey(string key) => 
            CustomMethods.ContainsKey(key);
        Delegate IReadOnlyDictionary<string, Delegate>.this[string key] { get=>CustomMethods[key]; }
        virtual Delegate this[string key] { get=>CustomMethods[key]; }
        bool IReadOnlyDictionary<string, Delegate>.TryGetValue(string key, out Delegate value) =>
            CustomMethods.TryGetValue(key, out value);
        virtual bool TryGetValue(string key, out Delegate value) =>
            CustomMethods.TryGetValue(key, out value);
        IEnumerator<KeyValuePair<string, Delegate>> IEnumerable<KeyValuePair<string, Delegate>>.GetEnumerator() =>
            CustomMethods.GetEnumerator();
        virtual IEnumerator<KeyValuePair<string, Delegate>> GetEnumerator() =>
            CustomMethods.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            CustomMethods.GetEnumerator();*/
            
        /// <summary>
        /// Implementation for custom user defined functions.
        /// Created to implement needed functions not suported by the interface.
        /// Override for custom implementation, default implementation use reflection.
        /// </summary>
        /// <param name="funcName">Name of custom function</param>
        /// <param name="args">Optional arguments for the custom function</param>
        public virtual void CustomFunc(string funcName, params object[]? args)
        {
            MethodInfo? method = GetType().GetMethod(funcName,
                args == null ? Type.EmptyTypes : args.Select(a => a.GetType()).ToArray());
            method?.Invoke(this, args);
        }
        /// <summary>
        /// Makes some plugins able to clear any unmanaged data from the memory or other finalizing options.
        /// </summary>
        public static virtual void PluginFree() { }
    }
    
    /// <summary>
    /// Standard interface for DSRemapper remappers
    /// </summary>
    public interface IDSRemapper : IDisposable
    {
        /// <summary>
        /// Occurs when the remapper raise a message to the device console.
        /// </summary>
        public event DeviceConsoleEventArgs? OnDeviceConsole;
        /// <summary>
        /// Sets the script for the remapper setting it up to start remapping the controller
        /// </summary>
        /// <param name="file">File path to the Remap Profile file</param>
        /// <param name="customMethods">A dictionary of custom methods from the controller that can be used by the script.</param>
        public void SetScript(string file, Dictionary<string, Delegate> customMethods);
        /// <summary>
        /// Main remap function of a Remapper class. This funciton is called every time the program needs to update the emulated controllers.
        /// </summary>
        /// <param name="report">Standard DSRemapper input report with the state of physical controller</param>
        /// <param name="deltaTime">The elapsed time between executions of the <see cref="Remap"/> function</param> 
        /// <returns>Standard DSRemapper output report with the feedback state for the physical controller</returns>
        public IDSROutputReport Remap(IDSRInputReport report, double deltaTime);
        /// <summary>
        /// Makes some plugins able to clear any unmanaged data from the memory or other finalizing options.
        /// </summary>
        public static virtual void PluginFree() { }
    }
    #endregion Plugins Interfaces
}