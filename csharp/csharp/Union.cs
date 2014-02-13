using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Emulate C++ unions, i.e. shared memory space.
    /// </summary>
    [TestClass]
    public class Union
    {
        [StructLayout(LayoutKind.Explicit)]
        public class RGB
        {
            [FieldOffset(2)]
            public byte R;

            [FieldOffset(1)]
            public byte G;

            [FieldOffset(0)]
            public byte B;

            /// <summary>
            /// This spans the other three fields,
            /// but isn't useful to consumers unless converted via AsHex
            /// </summary>
            [FieldOffset(0)]
            private int _Int32;

            public RGB() { }

            public RGB(byte R, byte G, byte B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }

            public string AsHex()
            {
                return _Int32.ToString("X6");
            }

            public RGB FromHex(string hex)
            {
                _Int32 = int.Parse
                    (hex, System.Globalization.NumberStyles.HexNumber);

                return this;
            }
        }

        /// <summary>
        /// Tests a simple use case that hides the overall integer
        /// representation of the individual bytes of an RGB value, but provides
        /// a method that converts that value to the more useful hex code.
        /// Also will convert a hex code to RGB, for completeness.
        /// </summary>
        /// <remarks>
        /// Compare this to the equivalent C++ test!
        /// </remarks>
        [TestMethod]
        public void TestUnion()
        {
            // Byte is a value type, and will default to 0,
            // so the default colour will be black!
            var black = new RGB();
            Assert.AreEqual(black.AsHex(), "000000");

            var coral = new RGB(255, 125, 125);
            Assert.AreEqual(coral.AsHex(), "FF7D7D");

            var coralConverted = new RGB().FromHex("FF7D7D");
            Assert.AreEqual(coralConverted.R, 255);
            Assert.AreEqual(coralConverted.G, 125);
            Assert.AreEqual(coralConverted.B, 125);
        }
    }
}
