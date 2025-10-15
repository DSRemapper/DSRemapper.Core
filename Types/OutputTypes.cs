using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSRemapper.Types
{
    /// <summary>
    /// Standardization of output report for DSRemapper plugins framework. Is used for DSRemapper remap profiles as well.
    /// </summary>
    public interface IDSROutputReport
    {
        /// <summary>
        /// Motor rumble array for controller vibration. It's functionality may vary depending on how the plugins use it.
        /// </summary>
        public float[] Rumble { get; set; }
        /// <summary>
        /// DSRLight class representing DualShock 4 light bar
        /// </summary>
        public DSRLight Led { get; set; }
        /// <summary>
        /// Extension values for leds. It's functionality may vary depending on how the plugins use it.
        /// </summary>
        public float[] ExtLeds { get; set; }
        /// <summary>
        /// The feedback effects to be send to the device.
        /// </summary>
        /// <returns>An array of <see cref="IDSRFeedback"/> objects.</returns>
        public IDSRFeedback[] Feedbacks { get; set; }
        /// <summary>
        /// Right rumble motor of the controller (index on rumble array: 0)
        /// </summary>
        virtual public float Right { get { return Rumble[0]; } set { Rumble[0] = value; } }
        /// <summary>
        /// Left rumble motor of the controller (index on rumble array: 1)
        /// </summary>
        virtual public float Left { get { return Rumble[1]; } set { Rumble[1] = value; } }

        #region DS4Layout
        /// <summary>
        /// Weak/right rumble motor of a DualShock 4 controller (index on rumble array: 0)
        /// </summary>
        virtual public float Weak { get { return Rumble[0]; } set { Rumble[0] = value; } }
        /// <summary>
        /// Strong/left rumble motor of a DualShock 4 controller (index on rumble array: 1)
        /// </summary>
        virtual public float Strong { get { return Rumble[1]; } set { Rumble[1] = value; } }
        /// <summary>
        /// Red led value for the DualShock 4 light bar (range: 0-1)
        /// </summary>
        virtual public float Red { get { return Led.Red; } set { Led.Red = value; } }
        /// <summary>
        /// Green led value for the DualShock 4 light bar (range: 0-1)
        /// </summary>
        virtual public float Green { get { return Led.Green; } set { Led.Green = value; } }
        /// <summary>
        /// Blue led value for the DualShock 4 light bar (range: 0-1)
        /// </summary>
        virtual public float Blue { get { return Led.Blue; } set { Led.Blue = value; } }
        /// <summary>
        /// On time percentage for the DualShock 4 light bar (range: 0-1)
        /// If this property and 'OffTime' property are both 0, light bar is always on.
        /// </summary>
        virtual public float OnTime { get { return Led.OnTime; } set { Led.OnTime = value; } }
        /// <summary>
        /// Off time percentage for the DualShock 4 light bar (range: 0-1)
        /// If this property and 'OnTime' property are both 0, light bar is always on.
        /// </summary>
        virtual public float OffTime { get { return Led.OffTime; } set { Led.OffTime = value; } }
        #endregion DS4Layout
    }
    /// <summary>
    /// Standardization of output report for DSRemapper plugins framework. Is used for DSRemapper remap profiles as well.
    /// </summary>
    public class DefaultDSROutputReport : IDSROutputReport
    {
        /// <summary>
        /// Constant for default DualShock 4 ligth bar intensity
        /// </summary>
        private const float defaulLedIntensity = 0.125f;
        /// <inheritdoc/>
        public float[] Rumble { get; set; } = new float[6];
        /// <inheritdoc/>
        public DSRLight Led { get; set; } = new();
        /// <inheritdoc/>
        public float[] ExtLeds { get; set; } = new float[6];
        /// <inheritdoc/>
        public IDSRFeedback[] Feedbacks { get; set; } = [];

        /// <summary>
        /// DefaultDSROutputReport class constructor
        /// </summary>
        public DefaultDSROutputReport()
        {
            Led.Player = 1;
            Led.Red = 0.4f * defaulLedIntensity;
            Led.Green = 0.8f * defaulLedIntensity;
            Led.Blue = 1f * defaulLedIntensity;
        }
    }
    /// <summary>
    /// Interface for feedback like data.
    /// (aka DualSense Adaptative Triggers or Stearing Wheel Force Feedback)
    /// </summary>
    public interface IDSRFeedback
    {
        /// <summary>
        /// The type of feedback effect.
        /// </summary>
        /// <returns>The feedback effect.</returns>
        ushort EffectType { get; set; }
        /// <summary>
        /// Flags for the effect. For the DualSense the active zones for the effects.
        /// </summary>
        /// <returns>A byte array with the effect flags.</returns>
        byte[] EffectFlags { get; set; }
        /// <summary>
        /// The frequency for the vibration or periodic effects.
        /// </summary>
        /// <returns>The frequency in Hz.</returns>
        float Frequency { get; set; }
        /// <summary>
        /// The arguments for the effect. For the DualSense the strengths applyed on each zone of the trigger.
        /// </summary>
        /// <returns>An Array with the arguments for the effect.</returns>
        float[] Strengths { get; set; }
        /// <inheritdoc/>
        virtual float[] Args { get => Strengths; set => Strengths = value; }
        /// <summary>
        /// An enumeration for the different predefined effects, if there are any.
        /// Usually the 0 value should be used for custom effects.
        /// If this value is gretear than 0 a preset should be applied and the other values in the inteface should be ignored.
        /// </summary>
        /// <returns>The index of the effect preset.</returns>
        public byte Presets { get; set; }
    }
    /// <summary>
    /// Default store class for force feedback effects data.
    /// </summary>
    public class DefaultDSRFFBData : IDSRFeedback
    {
        /// <inheritdoc/>
        public ushort EffectType { get; set; } = 0;
        /// <inheritdoc/>
        public byte[] EffectFlags { get; set; } = [];
        /// <inheritdoc/>
        public float Frequency { get; set; } = 0;
        /// <inheritdoc/>
        public float[] Strengths { get; set; } = [];
        /// <inheritdoc/>
        public byte Presets { get; set; } = 0;
    }
}
