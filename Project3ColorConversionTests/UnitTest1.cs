using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project3.MyColorSpaces;

namespace Project3ColorConversionTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RgbToXyz()
        {
            foreach (var rgb in GenerateAllRgb())
            {
                Assert.IsTrue(rgb.Equals(rgb.ToXyz().ToRgb()));
            }
        }

        [TestMethod]
        public void RgbToCmyk()
        {
            foreach (var rgb in GenerateAllRgb())
            {
                Assert.IsTrue(rgb.Equals(rgb.ToCmyk().ToRgb()));
            }
        }

        [TestMethod]
        public void RgbToHsl()
        {
            foreach (var rgb in GenerateAllRgb())
                Assert.IsTrue(rgb.Equals(rgb.ToHsl().ToRgb()));
        }

        [TestMethod]
        public void RgbToHsv()
        {
            foreach (var rgb in GenerateAllRgb())
                Assert.IsTrue(rgb.Equals(rgb.ToHsv().ToRgb()));
        }

        [TestMethod]
        public void RgbToYuv()
        {
            foreach (var rgb in GenerateAllRgb())
                Assert.IsTrue(rgb.Equals(rgb.ToYuv().ToRgb()));
        }

        [TestMethod]
        public void RgbToYCbCr()
        {
            foreach (var rgb in GenerateAllRgb())
                Assert.IsTrue(rgb.Equals(rgb.ToYCbCr().ToRgb()));
        }

        [TestMethod]
        public void XyzToLab()
        {
            foreach (var xyz in GenerateAllXyz())
                Assert.IsTrue(xyz.Equals(xyz.ToLab().ToXyz()));
        }

        [TestMethod]
        public void XyzToRgb()
        {
            foreach (var xyz in GenerateAllXyz())
            {
                var rgb = xyz.ToRgb();
                if (rgb.IsVisible == false)
                    continue;
                Assert.IsTrue(xyz.Equals(xyz.ToRgb().ToXyz()));
            }
        }

        public IEnumerable<MyRgb> GenerateAllRgb()
        {
            for (var r = 0; r < 255; r++)
            for (var g = 0; g < 255; g++)
            for (var b = 0; b < 255; b++)
                yield return new MyRgb(r, g, b);
        }
        public IEnumerable<MyXyz> GenerateAllXyz()
        {
            for (var x = 0.0; x < MyXyz.Xr; x+=0.5)
            for (var y = 0.0; y < MyXyz.Yr; y+=0.5)
            for (var z = 0.0; z < MyXyz.Yr; z+=0.5)
                yield return new MyXyz(x, y, z);
        }
    }
}