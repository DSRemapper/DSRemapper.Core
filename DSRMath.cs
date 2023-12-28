using DSRemapper.Core;
using System.Numerics;

namespace DSRemapper.DSRMath
{
    /// <summary>
    /// Represents a vector with two single-precision floating-point values.
    /// Used on DSRemapper SixAxis calculations
    /// </summary>
    public class DSRVector2
    {
        /// <inheritdoc cref="Vector2.X"/>
        public float X { get; set; } = 0;
        /// <inheritdoc cref="Vector2.Y"/>
        public float Y { get; set; } = 0;
        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public float Length { get { return MathF.Sqrt(X * X + Y * Y); } }

        /// <summary>
        /// Creates a new DSRVector2 object whose two elements are 0.
        /// </summary>
        public DSRVector2() { }
        /// <summary>
        /// Creates a new <see cref="System.Numerics.Vector2" /> object whose two elements have the same value.
        /// </summary>
        /// <param name="value">The value to assign to both elements.</param>
        public DSRVector2(float value) : this(value, value) { }
        /// <summary>Creates a vector whose elements have the specified values.</summary>
        /// <param name="x">The value to assign to the <see cref="X" /> property.</param>
        /// <param name="y">The value to assign to the <see cref="Y" /> property.</param>
        public DSRVector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Conversion from DSRVector2 to System.Numerics.Vector2
        /// </summary>
        /// <param name="vector">A DSRVector2 to convert to System.Numerics.Vector2</param>
        public static implicit operator Vector2(DSRVector2 vector) => new(vector.X, vector.Y);
        /// <summary>
        /// Conversion from System.Numerics.Vector2 to DSRVector2
        /// </summary>
        /// <param name="vector">A System.Numerics.Vector2 to convert to DSRVector2</param>
        public static implicit operator DSRVector2(Vector2 vector) => new(vector.X, vector.Y);

        /// <inheritdoc cref="Vector2.operator +"/>
        public static DSRVector2 operator +(DSRVector2 left, DSRVector2 right) => new(left.X + right.X, left.Y + right.Y);
        /// <inheritdoc cref="Vector2.operator -"/>
        public static DSRVector2 operator -(DSRVector2 left, DSRVector2 right) => new(left.X - right.X, left.Y - right.Y);
        /// <inheritdoc cref="Vector2.operator *(Vector2,Vector2)"/>
        public static DSRVector2 operator *(DSRVector2 left, DSRVector2 right) => new(left.X * right.X, left.Y * right.Y);
        /// <inheritdoc cref="Vector2.operator /(Vector2,Vector2)"/>
        public static DSRVector2 operator /(DSRVector2 left, DSRVector2 right) => new(left.X / right.X, left.Y / right.Y);
        /// <inheritdoc cref="Vector2.operator *(Vector2,float)"/>
        public static DSRVector2 operator *(DSRVector2 left, float right) => left * new DSRVector2(right);
        /// <inheritdoc cref="Vector2.operator /(Vector2,float)"/>
        public static DSRVector2 operator /(DSRVector2 left, float right) => left / new DSRVector2(right);
        /// <inheritdoc cref="operator *(DSRVector2, float)"/>
        public static DSRVector2 operator *(float right, DSRVector2 left) => left * right;
        /// <inheritdoc cref="operator /(DSRVector2, float)"/>
        public static DSRVector2 operator /(float right, DSRVector2 left) => left / right;

        /// <inheritdoc cref="Vector2.operator -(Vector2)"/>
        public static DSRVector2 operator -(DSRVector2 vec) => -1 * vec;
        /// <inheritdoc cref="Vector2.Dot(Vector2, Vector2)"/>
        /// <param name="left">The first vector</param>
        /// <param name="right">The second vector</param>
        public static float Dot(DSRVector2 left, DSRVector2 right) => left.X * right.X + left.Y * right.Y;
        /// <inheritdoc cref="Dot(DSRVector2, DSRVector2)"/>
        public float Dot(DSRVector2 right) => X * right.X + Y * right.Y;
        /// <inheritdoc cref="Vector2.Normalize(Vector2)"/>
        /// <param name="vector">The vector to normalize.</param>
        public static DSRVector2 Normalize(DSRVector2 vector) => new DSRVector2(vector.X, vector.Y) / vector.Length;
        /// <inheritdoc cref="Normalize(DSRVector2)"/>
        public DSRVector2 Normalize() => this / Length;
        /// <inheritdoc/>
        public override string ToString() => $"X: {X},Y: {Y}";
    }
    /// <summary>
    /// Represents a vector with three single-precision floating-point values.
    /// Used on DSRemapper SixAxis calculations
    /// </summary>
    public class DSRVector3
    {
        /// <inheritdoc cref="Vector3.X"/>
        public float X { get; set; } = 0;
        /// <inheritdoc cref="Vector3.Y"/>
        public float Y { get; set; } = 0;
        /// <inheritdoc cref="Vector3.Z"/>
        public float Z { get; set; } = 0;
        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public float Length { get { return MathF.Sqrt(X * X + Y * Y + Z * Z); } }

        /// <summary>
        /// Creates a new DSRVector3 object whose three elements are 0.
        /// </summary>
        public DSRVector3() { }
        /// <summary>
        /// Creates a new DSRVector3 object whose three elements have the same value.
        /// </summary>
        /// <param name="value">The value to assign to all three elements.</param>
        public DSRVector3(float value) : this(value, value, value) { }
        /// <summary>
        /// Creates a vector whose elements have the specified values.
        /// </summary>
        /// <param name="x">The value to assign to the <see cref="X" /> property.</param>
        /// <param name="y">The value to assign to the <see cref="Y" /> property.</param>
        /// <param name="z">The value to assign to the <see cref="Z" /> property.</param>
        public DSRVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        /// <summary>
        /// Conversion from DSRVector3 to System.Numerics.Vector3
        /// </summary>
        /// <param name="vector">A DSRVector3 to convert to System.Numerics.Vector3</param>
        public static implicit operator Vector3(DSRVector3 vector) => new(vector.X, vector.Y, vector.Z);
        /// <summary>
        /// Conversion from System.Numerics.Vector3 to DSRVector3
        /// </summary>
        /// <param name="vector">A System.Numerics.Vector3 to convert to DSRVector3</param>
        public static implicit operator DSRVector3(Vector3 vector) => new(vector.X, vector.Y, vector.Z);
        /// <inheritdoc cref="Vector3.operator +"/>
        public static DSRVector3 operator +(DSRVector3 left, DSRVector3 right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        /// <inheritdoc cref="Vector3.operator -"/>
        public static DSRVector3 operator -(DSRVector3 left, DSRVector3 right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        /// <inheritdoc cref="Vector3.operator *(Vector3,Vector3)"/>
        public static DSRVector3 operator *(DSRVector3 left, DSRVector3 right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        /// <inheritdoc cref="Vector3.operator /(Vector3,Vector3)"/>
        public static DSRVector3 operator /(DSRVector3 left, DSRVector3 right) => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        /// <inheritdoc cref="Vector3.operator *(Vector3,float)"/>
        public static DSRVector3 operator *(DSRVector3 left, float right) => left * new DSRVector3(right);
        /// <inheritdoc cref="Vector3.operator /(Vector3,float)"/>
        public static DSRVector3 operator /(DSRVector3 left, float right) => left / new DSRVector3(right);
        /// <inheritdoc cref="operator *(DSRVector3,float)"/>
        public static DSRVector3 operator *(float right, DSRVector3 left) => left * right;
        /// <inheritdoc cref="operator /(DSRVector3,float)"/>
        public static DSRVector3 operator /(float right, DSRVector3 left) => left / right;
        /// <inheritdoc cref="Vector3.operator -(Vector3)"/>
        public static DSRVector3 operator -(DSRVector3 vec) => -1 * vec;

        /// <inheritdoc cref="Vector3.Dot(Vector3, Vector3)"/>
        /// <param name="left">The first vector</param>
        /// <param name="right">The second vector</param>
        public static float Dot(DSRVector3 left, DSRVector3 right) => left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        /// <inheritdoc cref="Dot(DSRVector3, DSRVector3)"/>
        public float Dot(DSRVector3 right) => X * right.X + Y * right.Y + Z * right.Z;
        /// <inheritdoc cref="Vector3.Cross(Vector3, Vector3)"/>
        /// <param name="left">The first vector</param>
        /// <param name="right">The second vector</param>
        public static DSRVector3 Cross(DSRVector3 left, DSRVector3 right) => new((left.Y * right.Z) - (left.Z * right.Y), (left.Z * right.X) - (left.X * right.Z), (left.X * right.Y) - (left.Y * right.X));
        /// <inheritdoc cref="Cross(DSRVector3, DSRVector3)"/>
        public DSRVector3 Cross(DSRVector3 right) => new((Y * right.Z) - (Z * right.Y), (Z * right.X) - (X * right.Z), (X * right.Y) - (Y * right.X));
        /// <inheritdoc cref="Vector3.Normalize(Vector3)"/>
        /// <param name="vector">The vector to normalize.</param>
        public static DSRVector3 Normalize(DSRVector3 vector) => vector / vector.Length;
        /// <inheritdoc cref="Normalize(DSRVector3)"/>
        public DSRVector3 Normalize() => this / Length;
        /// <inheritdoc/>
        public override string ToString() => $"X: {X},Y: {Y},Z: {Z}";
    }
    /// <summary>
    /// Represents a vector that is used to encode three-dimensional physical rotations.
    /// Used on DSRemapper SixAxis calculations
    /// </summary>
    public class DSRQuaternion
    {
        /// <inheritdoc cref="Quaternion.X"/>
        public float X { get; set; } = 0;
        /// <inheritdoc cref="Quaternion.Y"/>
        public float Y { get; set; } = 0;
        /// <inheritdoc cref="Quaternion.Z"/>
        public float Z { get; set; } = 0;
        /// <inheritdoc cref="Quaternion.W"/>
        public float W { get; set; } = 1;
        /// <summary>
        /// Constructs an identity quaternion.
        /// </summary>
        public DSRQuaternion() { }
        /// <summary>
        /// Creates a quaternion from the specified vector and rotation parts.
        /// </summary>
        /// <param name="vec">The vector part of the quaternion.</param>
        /// <param name="w">The rotation part of the quaternion.</param>
        public DSRQuaternion(DSRVector3 vec, float w) : this(vec.X, vec.Y, vec.Z, w) { }
        /// <summary>
        /// Constructs a quaternion from the specified components.
        /// </summary>
        /// <param name="x">The value to assign to the X component of the quaternion.</param>
        /// <param name="y">The value to assign to the Y component of the quaternion.</param>
        /// <param name="z">The value to assign to the Z component of the quaternion.</param>
        /// <param name="w">The value to assign to the W component of the quaternion.</param>
        public DSRQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Conversion from DSRQuaternion to System.Numerics.Quaternion
        /// </summary>
        /// <param name="vector">A DSRQuaternion to convert to System.Numerics.Quaternion</param>
        public static implicit operator Quaternion(DSRQuaternion vector) => new(vector.X, vector.Y, vector.Z, vector.W);
        /// <summary>
        /// Conversion from System.Numerics.Quaternion to DSRQuaternion
        /// </summary>
        /// <param name="vector">A System.Numerics.Quaternion to convert to DSRQuaternion</param>
        public static implicit operator DSRQuaternion(Quaternion vector) => new(vector.X, vector.Y, vector.Z, vector.W);
        /// <inheritdoc cref="Quaternion.Dot(Quaternion, Quaternion)"/>
        public static float Dot(DSRQuaternion left, DSRQuaternion right) => left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        /// <inheritdoc cref="Dot(DSRQuaternion, DSRQuaternion)"/>
        public float Dot(DSRQuaternion right) => X * right.X + Y * right.Y + Z * right.Z + W * right.W;
        /// <inheritdoc cref="Quaternion.Inverse(Quaternion)"/>
        public static DSRQuaternion Inverse(DSRQuaternion value)
        {
            float ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            float invNorm = 1.0f / ls;

            return new(-value.X * invNorm, -value.Y * invNorm, -value.Z * invNorm, value.W * invNorm);
        }
        /// <inheritdoc cref="Inverse(DSRQuaternion)"/>
        public DSRQuaternion Inverse() => Inverse(this);
        /// <inheritdoc cref="Quaternion.operator *(Quaternion,Quaternion)"/>
        /// <param name="left">The first quaternion.</param>
        /// <param name="right">The second quaternion.</param>
        public static DSRQuaternion operator *(DSRQuaternion left, DSRQuaternion right)
        {
            float cx = left.Y * right.Z - left.Z * right.Y;
            float cy = left.Z * right.X - left.X * right.Z;
            float cz = left.X * right.Y - left.Y * right.X;

            float dot = left.X * right.X + left.Y * right.Y + left.Z * right.Z;

            return new(
                left.X * right.W + right.X * left.W + cx,
                left.Y * right.W + right.Y * left.W + cy,
                left.Z * right.W + right.Z * left.W + cz,
                left.W * right.W - dot);

        }
        /// <summary>
        /// Rotates a DSRVector3 using a DSRQuaternion
        /// </summary>
        /// <param name="quat">The quaternion used for the rotation</param>
        /// <param name="vec">The 3D vector to rotate</param>
        /// <returns>A DSRVector3 rotated by the rotation specified by the quaternion</returns>
        public static DSRVector3 operator *(DSRQuaternion quat, DSRVector3 vec)
        {
            DSRQuaternion vecQuat = new(vec, 0);
            vecQuat = quat * vecQuat * quat.Inverse();
            return new DSRVector3(vecQuat.X, vecQuat.Y, vecQuat.Z);
        }
        /// <inheritdoc cref="operator *(DSRQuaternion, DSRVector3)"/>
        public static DSRVector3 operator *(DSRVector3 vec, DSRQuaternion quat) => quat * vec;
        /// <inheritdoc/>
        public override string ToString() => $"X: {X},Y: {Y},Z: {Z},W: {W}";
    }
}
