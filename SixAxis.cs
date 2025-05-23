﻿using DSRemapper.DSRMath;
using System.Numerics;

namespace DSRemapper.SixAxis
{
    /// <summary>
    /// A Simple Signal Filter to minimize extreme values from IMU reads
    /// </summary>
    public class SimpleSignalFilter
    {
        private DSRVector3 sample1, y;
        /// <summary>
        /// SimpleSignalFilter class contructor
        /// </summary>
        public SimpleSignalFilter()
        {
            sample1 = y = new();
        }
        /// <summary>
        /// Creates a SimpleSignalFilter. Used for remap profiles to create a signal filter.
        /// </summary>
        /// <returns>A new SimpleSignalFilter</returns>
        public static SimpleSignalFilter CreateSSF() => new();
        /// <summary>
        /// Performs a low pass filter to eliminate high frequency noise from the IMU reads
        /// </summary>
        /// <param name="sample">The new sample value to filter</param>
        /// <param name="x0Strength">The percentage strength for the new sample (range: 0-1)</param>
        /// <returns>An interpolation of the sample without high frequency noise</returns>
        public DSRVector3 LowPass(DSRVector3 sample, float x0Strength)
        {
            x0Strength = Math.Clamp(x0Strength, 0, 1);
            y = (1 - x0Strength) * y + x0Strength * sample;

            return y;
        }
        /// <summary>
        /// Performs a low pass filter to eliminate high frequency noise from the IMU reads.
        /// Includes the last call sample to perform a better interpolation.
        /// </summary>
        /// <param name="sample">The new sample value to filter</param>
        /// <param name="x0Strength">The percentage strength for the new sample (range: 0-1)</param>
        /// <param name="x1Strength">The percentage strength for the previus sample (range: 0-1)</param>
        /// <returns>An interpolation of the sample without high frequency noise</returns>
        public DSRVector3 LowPass(DSRVector3 sample, float x0Strength, float x1Strength)
        {
            x0Strength = Math.Clamp(x0Strength, 0, 1);
            x1Strength = Math.Clamp(x1Strength, 0, 1);
            y = (1 - x0Strength - x1Strength) * y + x0Strength * sample + x1Strength * sample1;

            sample1 = sample;
            return y;
        }
    }
    /// <summary>
    /// Exponential Moving Average class used to calculate an average of a large amount of samples without storing all the values.
    /// </summary>
    public class ExpMovingAverage
    {
        int n = 0;
        /// <summary>
        /// Average stored by this object
        /// </summary>
        public float Average { get; private set; } = 0;
        /// <summary>
        /// ExpMovingAverage class constructor
        /// </summary>
        public ExpMovingAverage() { }
        /// <summary>
        /// Updates/Inserts a new sample to the average
        /// </summary>
        /// <param name="newValue">New sample value for the average</param>
        /// <param name="maxN">Max number of values that are taken into account for the average</param>
        /// <returns>The new average value</returns>
        public float Update(float newValue, int maxN = 0)
        {
            if (n == 0 || n < maxN)
                n++;

            Average += (newValue - Average) / n;
            return Average;
        }
    }
    /// <summary>
    /// Exponential Moving Average class used to calculate an average of a large amount of 3D vector samples without storing all the values.
    /// </summary>
    public class ExpMovingAverageVector3
    {
        int n = 0;

        /// <summary>
        /// Average value stored by this object
        /// </summary>
        public DSRVector3 Average { get; private set; } = new();

        /// <summary>
        /// ExpMovingAverageVector3 class constructor
        /// </summary>
        public ExpMovingAverageVector3() { }
        /// <summary>
        /// Updates/Inserts a new sample to the average
        /// </summary>
        /// <param name="newValue">New sample value for the average</param>
        /// <param name="maxN">Max number of values that are taken into account for the average</param>
        /// <returns>The new average value</returns>
        public DSRVector3 Update(DSRVector3 newValue, int maxN = 0)
        {
            if (n == 0 || n < maxN)
                n++;

            Average += (newValue - Average) / n;
            return Average;
        }
    }
    /// <summary>
    /// IMU data processor class. Performs all the calculation requiered to get all the data related to the IMU measurements
    /// </summary>
    public class SixAxisProcess
    {
        const float accelCorrection = 0.05f;

        private DateTime now = DateTime.Now, lastUpdate = DateTime.Now;
        /// <summary>
        /// Delta time used for the SixAxisProcess object to operate
        /// </summary>
        public float DeltaTime { get; private set; } = 0;
        /// <summary>
        /// Delta/diference rotation from the last update
        /// </summary>
        public DSRQuaternion deltaRotation = Quaternion.Identity;
        /// <summary>
        /// Total rotation of the IMU
        /// </summary>
        public DSRQuaternion rotation = Quaternion.Identity;
        /// <summary>
        /// Gravity vector pointing to planet center of gravity
        /// </summary>
        public DSRVector3 Grav = new(0, -1, 0);
        /// <summary>
        /// Acceleration vector without gravity component
        /// </summary>
        //of the IMU relative to it's starting position
        public DSRVector3 Accel = new();
        /// <summary>
        /// Acceleration vector with gravity component and without input error
        /// </summary>
        public DSRVector3 RawAccel = new();
        /// <summary>
        /// Gyroscope vector without input error
        /// </summary>
        public DSRVector3 RawGyro = new();

        /// <summary>
        /// SixAxisProcess class constructor
        /// </summary>
        public SixAxisProcess() { }
        /// <summary>
        /// SixAxisProcess update function, updates all the values using IMU accelerometer and gyroscope
        /// </summary>
        /// <param name="accel">3D vector representing device acceleration</param>
        /// <param name="gyro">3D vector representing device angular velocity</param>
        public void Update(DSRVector3 accel, DSRVector3 gyro)
        {
            now = DateTime.Now;
            DeltaTime = (now - lastUpdate).Ticks / (float)TimeSpan.TicksPerSecond;
            lastUpdate = now;

            float angleSpeed = gyro.Length * MathF.PI / 180f;
            float angle = angleSpeed * DeltaTime;
            DSRVector3 unitAccel = accel.Length > 0 ? accel.Normalize() : new();

            if (angle != 0)
                deltaRotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(Vector3.Normalize(gyro), angle));

            Grav = (DSRQuaternion)Quaternion.Inverse(deltaRotation) * Grav;

            Grav = Vector3.Normalize((1 - accelCorrection) * Grav + accelCorrection * unitAccel);

            rotation *= deltaRotation;
            //Console.WriteLine(accel);
            //Console.WriteLine(accel + Grav);
            Accel = (accel - Grav);//rotation * 
        }

        DSRVector3 lastGyroRead = new();
        DSRVector3 lastAccelRead = new();
        ExpMovingAverageVector3 accelError = new();
        ExpMovingAverageVector3 gyroError = new();

        /// <summary>
        /// IMU processing function. Update the IMU values and corrects the input errors.
        /// </summary>
        /// <param name="accel">Accelerometer raw input value</param>
        /// <param name="gyro">Gyroscope raw input value</param>
        /// <param name="maxCount">Indicates how much reads will be considerated for the error</param>
        // <param name="autoThesshold">If true the class will auto recalculate the thresshold to an optimal one</param>
        public void ProcessRawIMU(DSRVector3 accel, DSRVector3 gyro, int maxCount = 200, float gyroMag=1f, float accelMag = 0.1f)//, bool autoThesshold = false
        {
            DSRVector3 temp = gyro - lastGyroRead;
            if (temp.Length < gyroMag)
                gyroError.Update(gyro, 200);

            lastGyroRead = new(gyro.X, gyro.Y, gyro.Z);

            DSRVector3 fixedAccel = accel - accelError.Average;
            DSRVector3 fixedGyro = gyro - gyroError.Average;

            RawAccel = fixedAccel;
            RawGyro = fixedGyro;

            Update(fixedAccel, fixedGyro);

            //DSRVector3 AccelError = Accel + accelError.Average;
            temp = Accel - lastAccelRead;


            if (temp.Length < accelMag)
            {
                //Console.WriteLine($"{temp.Length} | {Accel}");
                accelError.Update(Accel, 200);
            }
            

            lastAccelRead = new(Accel.X, Accel.Y, Accel.Z);
        }
    }
}
