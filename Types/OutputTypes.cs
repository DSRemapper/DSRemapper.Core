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
}
