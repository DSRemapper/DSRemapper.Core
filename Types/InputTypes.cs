using DSRemapper.Core;
using DSRemapper.DSRMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSRemapper.Types
{
    /// <summary>
    /// Standardization of input reports for DSRemapper Framework. Is used for DSRemapper remap profiles as well.
    /// </summary>
    public interface IDSRInputReport
    {
        /// <summary>
        /// Gets the battery level of the controller (intended for wireless controllers as DualShock 4 or similars)
        /// </summary>
        public virtual float Battery { get => 0; set => _ = value; }
        /// <summary>
        /// Gets if the controller is currently charging (intended for the DualShock 4, which has a value indicating if it is charging/connected to an usb cable)
        /// </summary>
        public virtual bool Charging { get => false; set => _ = value; }
        /// <summary>
        /// Axes array containing all the axes of the controller
        /// </summary>
        public float[] Axes { get; }
        /// <summary>
        /// Sliders array containing all the sliders of the controller
        /// </summary>
        public float[] Sliders { get; }
        /// <summary>
        /// Buttons array containing all the buttons of the controller
        /// </summary>
        public bool[] Buttons { get; }
        /// <summary>
        /// POVs array containing all the POVs/D-Pads of the controller
        /// </summary>
        public DSRPov[] Povs { get; }
        /// <summary>
        /// SixAxis array containing all the IMU data of the controller
        /// </summary>
        public DSRVector3[] SixAxes { get; }
        /// <summary>
        /// Quaternions array containing IMU data quaternions of the controller
        /// </summary>
        public DSRQuaternion[] Quaternions { get; }
        /// <summary>
        /// Touches array containing all the finger touchs on the controller touchpad
        /// </summary>
        public DSRTouch[] Touch { get; }
        /// <summary>
        /// Touchpad resolution as a 2D vector (intended for the DualShock 4, by default is defined as zero vector)
        /// </summary>
        public virtual DSRVector2 TouchPadSize { get => new(); set => _ = value; }
        /// <summary>
        /// Delta time from last report. This value is setted from the controller and is meaning can change depending of the input plugin.
        /// By default this field is 0.
        /// </summary>
        public virtual float DeltaTime { get => 0; set => _ = value; }

        /// <summary>
        /// Sets Axes from index 0 to the array length
        /// </summary>
        /// <param name="axes">Axes array to set to the structure</param>
        public virtual void SetAxes(float[] axes) => SetAxes(axes, 0, axes.Length);
        /// <summary>
        /// Sets Axes from the offset to the specified length
        /// </summary>
        /// <param name="axes">Axes array to set to the structure</param>
        /// <param name="offset">The starting index to update</param>
        /// <param name="length">The maximum index to update</param>
        public virtual void SetAxes(float[] axes, int offset, int length)
        {
            int runLength = Math.Min(Axes.Length - offset, length);

            for (int i = offset; i < runLength; i++)
                Axes[i] = axes[i];
        }
        /// <summary>
        /// Sets Sliders from index 0 to the array length
        /// </summary>
        /// <param name="sliders">Sliders array to set to the structure</param>
        public virtual void SetSliders(float[] sliders) => SetSliders(sliders, 0, sliders.Length);
        /// <summary>
        /// Sets Sliders from the offset to the specified length
        /// </summary>
        /// <param name="sliders">Sliders array to set to the structure</param>
        /// <param name="offset">The starting index to update</param>
        /// <param name="length">The maximum index to update</param>
        public virtual void SetSliders(float[] sliders, int offset, int length)
        {
            int runLength = Math.Min(Sliders.Length - offset, length);

            for (int i = offset; i < runLength; i++)
                Sliders[i] = sliders[i];
        }
        /// <summary>
        /// Sets Buttons from index 0 to the array length
        /// </summary>
        /// <param name="buttons">Buttons array to set to the structure</param>
        public virtual void SetButtons(bool[] buttons) => SetButtons(buttons, 0, buttons.Length);
        /// <summary>
        /// Sets Buttons from the offset to the specified length
        /// </summary>
        /// <param name="buttons">Buttons array to set to the structure</param>
        /// <param name="offset">The starting index to update</param>
        /// <param name="length">The maximum index to update</param>
        public virtual void SetButtons(bool[] buttons, int offset, int length)
        {
            int runLength = Math.Min(Buttons.Length - offset, length);

            for (int i = offset; i < runLength; i++)
                Buttons[i] = buttons[i];
        }
        /// <summary>
        /// Sets POVs from index 0 to the array length
        /// </summary>
        /// <param name="povs">POVs array to set to the structure</param>
        public virtual void SetPovs(DSRPov[] povs) => SetPovs(povs, 0, povs.Length);
        /// <summary>
        /// Sets POVs from the offset to the specified length
        /// </summary>
        /// <param name="povs">POVs array to set to the structure</param>
        /// <param name="offset">The starting index to update</param>
        /// <param name="length">The maximum index to update</param>
        public virtual void SetPovs(DSRPov[] povs, int offset, int length)
        {
            int runLength = Math.Min(Povs.Length - offset, length);

            for (int i = 0; i < runLength; i++)
                Povs[i] = povs[i];
        }
        /// <summary>
        /// Sets all Touch Pads to the shortest length between controller touches or input array
        /// </summary>
        /// <param name="touch">Touches array to set to the structure</param>
        public virtual void SetTouchPads(DSRTouch[] touch) => SetTouchPads(touch, 0, touch.Length);

        /// <summary>
        /// Sets Touchpads from the offset to the specified length
        /// </summary>
        /// <param name="touch">Touches array to set to the structure</param>
        /// <param name="offset">The starting index to update</param>
        /// <param name="length">The maximum index to update</param>
        public virtual void SetTouchPads(DSRTouch[] touch, int offset, int length)
        {
            int runLength = Math.Min(Touch.Length - offset, length);

            for (int i = 0; i < runLength; i++)
                Touch[i] = touch[i];
        }

        #region Axes
        /// <summary>
        /// Left Stick X Axis (for DS4 and Xbox)
        /// </summary>
        public virtual float LX { get { return Axes[0]; } set { Axes[0] = value; } }
        /// <summary>
        /// Left Stick Y Axis (for DS4 and Xbox)
        /// </summary>
        public virtual float LY { get { return Axes[1]; } set { Axes[1] = value; } }
        /// <summary>
        /// Right Stick X Axis (for DS4 and Xbox)
        /// </summary>
        public virtual float RX { get { return Axes[2]; } set { Axes[2] = value; } }
        /// <summary>
        /// Right Stick Y Axis (for DS4 and Xbox)
        /// </summary>
        public virtual float RY { get { return Axes[3]; } set { Axes[3] = value; } }
        /// <summary>
        /// Left Trigger Axis (for DS4 and Xbox)
        /// </summary>
        public virtual float LTrigger { get { return Axes[4]; } set { Axes[4] = value; } }
        /// <summary>
        /// Right Trigger Axis (for DS4 and Xbox)
        /// </summary>
        public virtual float RTrigger { get { return Axes[5]; } set { Axes[5] = value; } }
        #endregion Axes

        #region POV1
        /// <summary>
        /// Controller First Pov
        /// </summary>
        public virtual DSRPov Pov { get => Povs[0]; set => Povs[0] = value; }
        /// <summary>
        /// Controller First Pov Up Button
        /// </summary>
        public virtual bool Up { get { return Povs[0].Up; } set { Povs[0].Up = value; } }
        /// <summary>
        /// Controller First Pov Right Button
        /// </summary>
        public virtual bool Right { get { return Povs[0].Right; } set { Povs[0].Right = value; } }
        /// <summary>
        /// Controller First Pov Down Button
        /// </summary>
        public virtual bool Down { get { return Povs[0].Down; } set { Povs[0].Down = value; } }
        /// <summary>
        /// Controller First Pov Left Button
        /// </summary>
        public virtual bool Left { get { return Povs[0].Left; } set { Povs[0].Left = value; } }
        #endregion POV1

        #region DS4Layout
        /// <summary>
        /// DualShock 4 Square button (index on the buttons array: 0)
        /// </summary>
        public virtual bool Square { get { return Buttons[0]; } set { Buttons[0] = value; } }
        /// <summary>
        /// DualShock 4 Cross button (index on the buttons array: 1)
        /// </summary>
        public virtual bool Cross { get { return Buttons[1]; } set { Buttons[1] = value; } }
        /// <summary>
        /// DualShock 4 Circle button (index on the buttons array: 2)
        /// </summary>
        public virtual bool Circle { get { return Buttons[2]; } set { Buttons[2] = value; } }
        /// <summary>
        /// DualShock 4 Triangle button (index on the buttons array: 3)
        /// </summary>
        public virtual bool Triangle { get { return Buttons[3]; } set { Buttons[3] = value; } }
        /// <summary>
        /// DualShock 4 L1 button (index on the buttons array: 4)
        /// </summary>
        public virtual bool L1 { get { return Buttons[4]; } set { Buttons[4] = value; } }
        /// <summary>
        /// DualShock 4 R1 button (index on the buttons array: 5)
        /// </summary>
        public virtual bool R1 { get { return Buttons[5]; } set { Buttons[5] = value; } }
        /// <summary>
        /// DualShock 4 L2 button (index on the buttons array: 6)
        /// </summary>
        public virtual bool L2 { get { return Buttons[6]; } set { Buttons[6] = value; } }
        /// <summary>
        /// DualShock 4 R2 button (index on the buttons array: 7)
        /// </summary>
        public virtual bool R2 { get { return Buttons[7]; } set { Buttons[7] = value; } }
        /// <summary>
        /// DualShock 4 Share button (index on the buttons array: 8)
        /// </summary>
        public virtual bool Share { get { return Buttons[8]; } set { Buttons[8] = value; } }
        /// <summary>
        /// DualShock 4 Options button (index on the buttons array: 9)
        /// </summary>
        public virtual bool Options { get { return Buttons[9]; } set { Buttons[9] = value; } }
        /// <summary>
        /// DualShock 4 L3 button (index on the buttons array: 10)
        /// </summary>
        public virtual bool L3 { get { return Buttons[10]; } set { Buttons[10] = value; } }
        /// <summary>
        /// DualShock 4 R3 button (index on the buttons array: 11)
        /// </summary>
        public virtual bool R3 { get { return Buttons[11]; } set { Buttons[11] = value; } }
        /// <summary>
        /// DualShock 4 PS button (index on the buttons array: 12)
        /// Be careful, if the button is pressed for 10 seconds the DualShock 4 controller will shutdown.
        /// </summary>
        public virtual bool PS { get { return Buttons[12]; } set { Buttons[12] = value; } }
        /// <summary>
        /// DualShock 4 Touch Pad button (index on the buttons array: 13)
        /// </summary>
        public virtual bool TouchPad { get { return Buttons[13]; } set { Buttons[13] = value; } }
        #endregion DS4Layout

        #region XboxLayout
        /// <summary>
        /// Xbox X button (index on the buttons array: 0)
        /// </summary>
        public virtual bool X { get { return Buttons[0]; } set { Buttons[0] = value; } }
        /// <summary>
        /// Xbox A button (index on the buttons array: 1)
        /// </summary>
        public virtual bool A { get { return Buttons[1]; } set { Buttons[1] = value; } }
        /// <summary>
        /// Xbox B button (index on the buttons array: 2)
        /// </summary>
        public virtual bool B { get { return Buttons[2]; } set { Buttons[2] = value; } }
        /// <summary>
        /// Xbox Left Button button (index on the buttons array: 4)
        /// </summary>
        public virtual bool Y { get { return Buttons[3]; } set { Buttons[3] = value; } }
        /// <summary>
        /// Xbox Left Button button (index on the buttons array: 4)
        /// </summary>
        public virtual bool LButton { get { return Buttons[4]; } set { Buttons[4] = value; } }
        /// <summary>
        /// Xbox Right Button button (index on the buttons array: 5)
        /// </summary>
        public virtual bool RButton { get { return Buttons[5]; } set { Buttons[5] = value; } }
        /// <summary>
        /// Xbox Back button (index on the buttons array: 8)
        /// </summary>
        public virtual bool Back { get { return Buttons[8]; } set { Buttons[8] = value; } }
        /// <summary>
        /// Xbox Start button (index on the buttons array: 9)
        /// </summary>
        public virtual bool Start { get { return Buttons[9]; } set { Buttons[9] = value; } }
        /// <summary>
        /// Xbox Left Thumb button (index on the buttons array: 10)
        /// </summary>
        public virtual bool LThumb { get { return Buttons[10]; } set { Buttons[10] = value; } }
        /// <summary>
        /// Xbox Right Thumb button (index on the buttons array: 11)
        /// </summary>
        public virtual bool RThumb { get { return Buttons[11]; } set { Buttons[11] = value; } }
        /// <summary>
        /// Xbox Guide button (index on the buttons array: 12)
        /// </summary>
        public virtual bool Guide { get { return Buttons[12]; } set { Buttons[12] = value; } }
        #endregion XboxLayout

        #region SixAxes
        /// <summary>
        /// Raw Acceleration data (with gravity) from IMU data (index on SixAxis array: 0)
        /// </summary>
        public virtual DSRVector3 RawAccel { get { return SixAxes[0]; } set { SixAxes[0] = value; } }
        /// <summary>
        /// Raw Gyro data from IMU data (index on SixAxis array: 1)
        /// </summary>
        public virtual DSRVector3 Gyro { get { return SixAxes[1]; } set { SixAxes[1] = value; } }
        /// <summary>
        /// Gravity vector from IMU data (index on SixAxis array: 2)
        /// Points to the "global/planet" gravity relative to the controller. Can detect pitch and roll of the controller.
        /// </summary>
        public virtual DSRVector3 Grav { get { return SixAxes[2]; } set { SixAxes[2] = value; } }
        /// <summary>
        /// Processed Acceleration data (without gravity and fixed) from IMU data (index on SixAxis array: 3)
        /// Rotation of the controller is irrelevant to this value. If the controller is turned 90 degrees, and is moved left to right, in this vector it will still be on the X axis.
        /// </summary>
        public virtual DSRVector3 Accel { get { return SixAxes[3]; } set { SixAxes[3] = value; } }
        /// <summary>
        /// Gets the delta/diference of quaternion rotation from the last report
        /// </summary>
        public virtual DSRQuaternion DeltaRotation { get { return Quaternions[0]; } set { Quaternions[0] = value; } }
        /// <summary>
        /// Gets the current total rotation of the controller as a quaternion
        /// </summary>
        public virtual DSRQuaternion Rotation { get { return Quaternions[1]; } set { Quaternions[1] = value; } }
        #endregion SixAxes
    }
    /// <summary>
    /// A default implementation of the IDSRInputReport.
    /// Is recomended to implement a class using the IDSRInputReport interface.
    /// </summary>
    public class DefaultDSRInputReport : IDSRInputReport
    {
        /// <inheritdoc/>
        public float Battery { get; set; } = 0;
        /// <inheritdoc/>
        public bool Charging { get; set; } = false;
        /// <inheritdoc/>
        public float DeltaTime { get; set; } = 0;
        /// <inheritdoc/>
        public float[] Axes { get; } = new float[6];
        /// <inheritdoc/>
        public float[] Sliders { get; } = new float[6];
        /// <inheritdoc/>
        public bool[] Buttons { get; } = new bool[32];
        /// <inheritdoc/>
        public DSRPov[] Povs { get; } = [new()];
        /// <inheritdoc/>
        public DSRVector3[] SixAxes { get; } = [];
        /// <inheritdoc/>
        public DSRQuaternion[] Quaternions { get; } = [];
        /// <inheritdoc/>
        public DSRTouch[] Touch { get; } = [];
        /// <inheritdoc/>
        public DSRVector2 TouchPadSize { get; set; } = new();

        /// <summary>
        /// Creates a DefaultDSRInputReport with the specified values.
        /// If no arguments are specified, creates a DefaultDSRInputReport with default values, which are compatibles with DirectInput.
        /// </summary>
        /// <param name="axes">Number of axes for the structure</param>
        /// <param name="sliders">Number of sliders for the structure</param>
        /// <param name="buttons">Number of buttons for the structure</param>
        /// <param name="povs">Number of povs for the structure (value between 0 and 4)</param>
        /// <param name="touchs">Number of touchpad structures for the structure</param>
        /// <param name="sixAxes">Number of sixAxes for the structure (each sixAxis represents a 3D vector)</param>
        /// <param name="quaternions">Number of quaternions for the structure</param>
        public DefaultDSRInputReport(byte axes = 6, byte sliders = 6, byte buttons = 32, byte povs = 1, byte touchs = 0, byte sixAxes = 0, byte quaternions = 0)
        {
            Axes = new float[axes];
            Sliders = new float[sliders];
            Buttons = new bool[buttons];

            povs = Math.Clamp(povs, (byte)0, (byte)4);
            Povs = new DSRPov[povs];
            for (int i = 0; i < Povs.Length; i++)
                Povs[i] = new DSRPov();

            Touch = new DSRTouch[touchs];
            for (int i = 0; i < Touch.Length; i++)
                Touch[i] = new DSRTouch();

            SixAxes = new DSRVector3[sixAxes];
            for (int i = 0; i < SixAxes.Length; i++)
                SixAxes[i] = new DSRVector3();

            Quaternions = new DSRQuaternion[quaternions];
            for (int i = 0; i < Quaternions.Length; i++)
                Quaternions[i] = new DSRQuaternion();
        }
    }
}
