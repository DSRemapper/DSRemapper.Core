using DSRemapper.DSRMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSRemapper.Types
{
    /// <summary>
    /// DSRPov class that represent a POV (Point Of View) control (also known as D-Pad)
    /// </summary>
    public class DSRPov
    {
        private static readonly bool[][] buttonArragement = [
            [false, false, false, false],
            [true, false, false, false],
            [true, true, false, false],
            [false, true, false, false],
            [false, true, true, false],
            [false, false, true, false],
            [false, false, true, true],
            [false, false, false, true],
            [true, false, false, true],
        ];

        private float angle = -1;
        /// <summary>
        /// Current angle (in degrees) of the POV control
        /// </summary>
        public float Angle { get { return angle; } set { angle = value < 0 ? -1 : Math.Clamp(value, 0, 360); CalculateButtons(); } }
        private bool[] buts = new bool[4];
        /// <summary>
        /// Gets if the pov is poiting North/Up
        /// </summary>
        public bool Up { get => buts[0]; set { buts[0] = value; angle = -1; } }
        /// <summary>
        /// Gets if the pov is poiting East/Right
        /// </summary>
        public bool Right { get => buts[1]; set { buts[1] = value; angle = -1; } }
        /// <summary>
        /// Gets if the pov is poiting South/Down
        /// </summary>
        public bool Down { get => buts[2]; set { buts[2] = value; angle = -1; } }
        /// <summary>
        /// Gets if the pov is poiting West/Left
        /// </summary>
        public bool Left { get => buts[3]; set { buts[3] = value; angle = -1; } }

        /// <summary>
        /// Auxiliar function to set pov from a 0 to 8 value (8 is 'nothing pressed')
        /// </summary>
        /// <param name="pov">A byte value within 0-8 range</param>
        public void SetDSPov(byte pov)
        {
            Angle = pov != 8 ? pov * 360f / 8f : -1;
            CalculateButtons();
        }
        /// <summary>
        /// Updates the unassigned values of the POV.
        /// If the angle is -1, the function will calculate the angle using the buttons value. Otherwise, it will calculate the buttons using the angle value.
        /// </summary>
        public void Update()
        {
            if (Angle == -1)
                CalculateAngle();
            else
                CalculateButtons();
        }
        /// <summary>
        /// Calculate the angle using the buttons value
        /// </summary>
        public void CalculateAngle()
        {
            int angle = -1;
            for (int i = 0; i < buttonArragement.Length; i++)
            {
                if (buttonArragement[i][0] == buts[0] && buttonArragement[i][1] == buts[1] &&
                    buttonArragement[i][2] == buts[2] && buttonArragement[i][3] == buts[3])
                {
                    angle = i - 1;
                    break;
                }
            }

            if (angle < 0)
                this.angle = -1;
            else
                this.angle = angle * 45f;
        }
        /// <summary>
        /// Calculate the buttons using the angle value
        /// </summary>
        public void CalculateButtons()
        {
            int dPad = (int)(Angle / 45);
            if (Angle >= 0)
                buts = (bool[])buttonArragement[dPad + 1].Clone();
            else
                buts = (bool[])buttonArragement[0].Clone();
        }
        /// <summary>
        /// String representation of the pressed buttons of the POV/D-Pad
        /// </summary>
        /// <returns>A string representing the buttons values of the POV</returns>
        public override string ToString() => $"U:{Up},D:{Down},L:{Left},R:{Right}";
        /// <summary>
        /// String representation of the angle of the POV/D-Pad
        /// </summary>
        /// <returns>A string containing the angle in degrees</returns>
        public string ToStringAngle() => $"Ang: {Angle}";
    }
    /// <summary>
    /// DSRLight class that represents the DualShock 4 light bar
    /// </summary>
    public class DSRLight
    {
        private readonly float[] led = [0f, 0f, 0f];
        private readonly float[] OnOff = [0f, 0f];
        /// <summary>
        /// Player number for Xbox controller compatibility, to get or set the player number of the controller.
        /// </summary>
        public float Player { get; set; }
        /// <summary>
        /// Red value of the RGB of the light bar (range: 0-1)
        /// </summary>
        public float Red { get { return led[0]; } set { led[0] = Math.Clamp(value, 0, 1); } }
        /// <summary>
        /// Green value of the RGB of the light bar (range: 0-1)
        /// </summary>
        public float Green { get { return led[1]; } set { led[1] = Math.Clamp(value, 0, 1); } }
        /// <summary>
        /// Blue value of the RGB of the light bar (range: 0-1)
        /// </summary>
        public float Blue { get { return led[2]; } set { led[2] = Math.Clamp(value, 0, 1); } }
        /// <summary>
        /// On time percentage for the DualShock 4 light bar (range: 0-1)
        /// If this property and 'OffTime' property are both 0, light bar is always on.
        /// </summary>
        public float OnTime { get { return OnOff[0]; } set { OnOff[0] = Math.Clamp(value, 0, 1); } }
        /// <summary>
        /// Off time percentage for the DualShock 4 light bar (range: 0-1)
        /// If this property and 'OnTime' property are both 0, light bar is always on.
        /// </summary>
        public float OffTime { get { return OnOff[1]; } set { OnOff[1] = Math.Clamp(value, 0, 1); } }
        /// <summary>
        /// DSRLight class contructor
        /// </summary>
        public DSRLight() { }
        /// <summary>
        /// DSRLight class contructor
        /// </summary>
        /// <param name="red">Red value of the light bar (range: 0-1)</param>
        /// <param name="green">Green value of the light bar (range: 0-1)</param>
        /// <param name="blue">Blue value of the light bar (range: 0-1)</param>
        /// <param name="intensity">Global multiplier of intensity/brightness for all the color channels (RGB values)</param>
        public DSRLight(float red, float green, float blue, float intensity = 1) => SetRGB(red, green, blue, intensity);
        /// <summary>
        /// Sets all the color channels of the light bar, and applies an intesity/brightness value
        /// </summary>
        /// <param name="red">Red value of the light bar (range: 0-1)</param>
        /// <param name="green">Green value of the light bar (range: 0-1)</param>
        /// <param name="blue">Blue value of the light bar (range: 0-1)</param>
        /// <param name="intensity">Global multiplier of intensity for all the color channels (RGB values)</param>
        public void SetRGB(float red, float green, float blue, float intensity = 1)
        {
            Red = red * intensity;
            Green = green * intensity;
            Blue = blue * intensity;
        }
        /// <summary>
        /// Sets all the color channels of the light bar, and applies an intesity/brightness value
        /// </summary>
        /// <param name="leds">An array, of at least, three values representing the RGB channels (each array value range: 0-1)</param>
        /// <param name="intensity">Global multiplier of intensity for all the color channels (RGB values)</param>
        public void SetRGB(float[] leds, float intensity = 1)
        {
            if (leds.Length >= 3)
            {
                Red = leds[0] * intensity;
                Green = leds[1] * intensity;
                Blue = leds[2] * intensity;
            }
        }
        /// <summary>
        /// Multiplies the current led color channels by an intensity/brightness value
        /// </summary>
        /// <param name="light">The DSRLight class to apply the intensity</param>
        /// <param name="intensity">The intensity value</param>
        /// <returns>A new DSRLight object with the intensity/brightness applied</returns>
        public static DSRLight operator *(DSRLight light, float intensity) => new(light.Red, light.Green, light.Blue, intensity);
        /// <inheritdoc cref="operator *(DSRLight, float)"/>
        public static DSRLight operator *(float intensity, DSRLight light) => light * intensity;
        /// <summary>
        /// Adds the value to the current led color channels values
        /// </summary>
        /// <param name="light">The DSRLight class to apply the intensity</param>
        /// <param name="add">The value to add to all color channels</param>
        /// <returns>A new DSRLight object with the value added to all color channels</returns>
        public static DSRLight operator +(DSRLight light, float add) => new(light.Red + add, light.Green + add, light.Blue + add);
        /// <inheritdoc cref="operator +(DSRLight, float)"/>
        public static DSRLight operator +(float add, DSRLight light) => light + add;
        /// <summary>
        /// Subtract the value to the current led color channels values
        /// </summary>
        /// <param name="light">The DSRLight class to apply the intensity</param>
        /// <param name="sub">The value to subtracted to all color channels</param>
        /// <returns>A new DSRLight object with the value subtracted to all color channels</returns>
        public static DSRLight operator -(DSRLight light, float sub) => light + (-sub);
        /// <inheritdoc cref="operator -(DSRLight, float)"/>
        public static DSRLight operator -(float sub, DSRLight light) => light - sub;
    }
    /// <summary>
    /// DSRTouch class that represents a finger touch on a touchpad (intended for the DualShock 4 touchpad)
    /// </summary>
    public class DSRTouch
    {
        /// <summary>
        /// Current id of the touch
        /// </summary>
        public int Id { get; set; } = 0;
        /// <summary>
        /// Gets if the finger is touching the touchpad
        /// </summary>
        public bool Pressed { get; set; } = false;
        /// <summary>
        /// A 2D vector representing the position of the finger in a range of 0-1 in both axis
        /// </summary>
        public DSRVector2 Pos { get; set; } = new();
        /// <summary>
        /// Gets a String representation of the DSRTouch class
        /// </summary>
        /// <returns>A string containing the id, if is pressed and the position of the touch</returns>
        public override string ToString() => $"Id:{Id},P:{Pressed},{Pos}";
    }
}
