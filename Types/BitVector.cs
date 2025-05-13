using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSRemapper.Core.Types
{
    /// <summary>
    /// A structure to create simple bitfields for specific hardware communications
    /// </summary>
    /// <typeparam name="T">Type to mimic (byte: 1 byte, ushort: 2 bytes, uint: 4 bytes, ulong: 8 bytes)</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitVector<T> where T : IBinaryInteger<T>, IUnsignedNumber<T>
    {
        T data;

        /// <summary>
        /// Gets or sets the raw value of the BitVector
        /// </summary>
        public T Data { get { return data; } set { data = value; } }

        /// <summary>
        /// BitVector constructor
        /// </summary>
        public BitVector():this(default!){}
        /// <summary>
        /// BitVector constructor
        /// </summary>
        /// <param name="data">The initial value of the field</param>
        public BitVector(T data)
        {
            this.data = data;
        }
        /// <summary>
        /// Applies the given mask to the BitVector and:
        /// Returns true if all bits the set bits of the mask are set, false otherwise.
        /// If is set to true or false, all the set bits of the mask will be set or clear respectively.
        /// </summary>
        /// <param name="mask">The mask to apply</param>
        /// <returns>True if all set bits of the mask are set, false otherwise</returns>
        public bool this[T mask]
        {
            get => (dynamic)(Data & mask) == mask;
            set => Data = value ? Data | mask : Data & ~mask;
        }
        /// <summary>
        /// Applies the given mask to the BitVector and:
        /// Returns or sets the stored value for said mask.
        /// </summary>
        /// <param name="mask">The mask to apply</param>
        /// <param name="offset">The bit offset for the mask</param>
        /// <returns>The value stored for said mask</returns>
        public T this[T mask, byte offset]
        {
            get => (Data & (mask << offset)) >> offset;
            set => Data = (Data & ~(mask << offset)) | ((value & mask) << offset);
        }
        /// <summary>
        /// Returns or sets the stored value for the section provided.
        /// </summary>
        /// <param name="section">A BitField Section declared for this BitField</param>
        /// <returns>The value stored for the section</returns>
        public T this[Section section]
        {
            get => this[section.Mask, section.Offset];
            set => this[section.Mask, section.Offset] = value;

            /*get => (Data & section.RawMask) >> section.Offset;
            set => Data = (Data & ~section.RawMask) | ((value << section.Offset) & section.RawMask);*/
        }
        /// <summary>
        /// Creates a section using a mask
        /// </summary>
        /// <param name="mask">The mask to be stored as a Section</param>
        /// <returns>A new section based on the provided mask</returns>
        public static Section CreateSection(T mask) => new(mask, 0);
        /// <summary>
        /// Creates a section using a mask and a offset
        /// </summary>
        /// <param name="mask">The mask to be stored as a Section</param>
        /// <param name="offset">The bit offset for the mask</param>
        /// <returns>A new section based on the provided mask and offset</returns>
        public static Section CreateSection(T mask, byte offset) => new(mask, offset);
        /// <summary>
        /// Creates a section using a mask and a section as a offset
        /// </summary>
        /// <param name="mask">The mask to be stored as a Section</param>
        /// <param name="previous">The previous section in the bitfield as an offset</param>
        /// <returns>A new section based on the provided mask and offset</returns>
        public static Section CreateSection(T mask, Section previous) => new(mask, previous);

        /// <summary>
        /// A BitField section which stores a mask and a offset to be applied to a BitField structure
        /// </summary>
        public struct Section
        {
            T mask;
            byte offset;

            /// <summary>
            /// The stored mask
            /// </summary>
            public T Mask => mask;
            /// <summary>
            /// The stored mask with the offset applied.
            /// If the mask is 0b11 and the offset is 3, will return 0b11000
            /// </summary>
            public T RawMask => mask << offset;
            /// <summary>
            /// The offset of the section
            /// </summary>
            public byte Offset => offset;

            /// <summary>
            /// A constructor for the Section structure
            /// </summary>
            /// <param name="mask">The mask to be stored</param>
            /// <param name="offset">The offset to apply to the mask</param>
            /// <exception cref="InvalidOperationException">If a mask set bit have a offset greater than the BitField type size an exception will be raised</exception>
            public Section(T mask, byte offset)
            {
                this.offset = offset;
                this.mask = (T)(dynamic)(BitOperations.RoundUpToPowerOf2((ulong)(dynamic)mask + 1) - 1);
                if (BitOperations.PopCount((dynamic)mask) + offset > mask.GetByteCount() * 8)
                    throw new InvalidOperationException("BitVector overflow. Mask bits and the offset exceeds the mask size.");
            }
            /// <summary>
            /// A constructor for the Section structure
            /// </summary>
            /// <param name="mask">The mask to be stored</param>
            /// <param name="previous">The previous section as an offset for the mask</param>
            public Section(T mask, Section previous) : this(mask, previous.NextSectionOffset()) { }

            /// <summary>
            /// Gives the offset of all set bits of the mask plus its offset
            /// </summary>
            /// <returns>The bit offset for the next bit to the left of the Section</returns>
            public byte NextSectionOffset() => (byte)(BitOperations.PopCount((dynamic)Mask) + Offset);
        }
    }
}
