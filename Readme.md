# DSRemapper.Core - DSR/SDK

`DSRemapper.Core` is the foundational SDK package for the DSRemapper application. It contains all the essential interfaces, attributes, and data types necessary for creating any type of plugin.

By providing a standardized set of contracts and structures, it ensures that input, remapper, and output plugins can communicate effectively with each other and with the main `DSRemapper.Framework`.

**If you are a developer looking to extend DSRemapper's functionality, this is the primary library you will be working with. Also, check the DSRPackager app to package your plugin.**

## Key Components

Breakdown of the main parts of the assembly and their responsibilities:

1.  **Core Interfaces**: These define the fundamental contracts that plugins must implement.
    -   `IDSRDeviceScanner`: For discovering physical controllers.
    -   `IDSRInputController`: For representing and interacting with a physical controller.
    -   `IDSRemapper`: For implementing the input translation logic.
    -   `IDSROutputController`: For emulating virtual controllers.

2.  **Plugin Attributes**: Used to provide metadata about your plugin classes, allowing the framework to discover and manage them.
    -   `RemapperAttribute`: Binds a remapper plugin to profile file extensions.
    -   `EmulatedControllerAttribute`: Identifies an output plugin as a virtual controller.
    -   `CustomMethodAttribute`: Exposes custom functions from a controller to be used in scripts or a GUI.

3.  **Standardized Data Types**: Found in the `DSRemapper.Types` namespace, these ensure consistent data flow.
    -   `IDSRInputReport`: A standard structure for all controller input data (axes, buttons, motion sensors).
    -   `IDSROutputReport`: A standard structure for sending feedback (rumble, LEDs) to a physical controller.

4.  **Six-Axis and Math Helpers**: The `DSRemapper.DSRMath` and `DSRemapper.SixAxis` namespaces provide tools for processing motion control data, including vectors, quaternions, and filters.

5.  **Utilities**:
    -   `DSRLogger`: A logging facility for plugins.
    -   `DSRPaths`: Provides standardized paths to important application folders (`Plugins`, `Profiles`, etc.).
    -   `PluginManifest`: Allows plugins to declare metadata like name, version, and dependencies. Created automatically by the DSRPackager app.
