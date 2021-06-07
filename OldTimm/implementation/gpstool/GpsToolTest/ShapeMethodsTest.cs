using GPS.Tool.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GPS.Tool.Data;
using System.Collections.Generic;

namespace GpsToolTest
{


    /// <summary>
    ///This is a test class for ShapeMethodsTest and is intended
    ///to contain all ShapeMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ShapeMethodsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion



        /// <summary>
        ///A test for BoxInBox
        ///</summary>
        [TestMethod()]
        [DeploymentItem("GPSTool.exe")]
        public void BoxInBoxTest()
        {
            double mMinLat = 0F; // TODO: Initialize to an appropriate value
            double mMaxLat = 0F; // TODO: Initialize to an appropriate value
            double mMinLon = 0F; // TODO: Initialize to an appropriate value
            double mMaxLon = 0F; // TODO: Initialize to an appropriate value
            double iMinLat = 0F; // TODO: Initialize to an appropriate value
            double iMaxLat = 0F; // TODO: Initialize to an appropriate value
            double iMinLon = 0F; // TODO: Initialize to an appropriate value
            double iMaxLon = 0F; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = ShapeMethods_Accessor.BoxInBox(mMinLat, mMaxLat, mMinLon, mMaxLon, iMinLat, iMaxLat, iMinLon, iMaxLon);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PolygonInPolygon
        ///</summary>
        [TestMethod()]
        public void PolygonInPolygonTest()
        {
            List<ICoordinate> masterCoordinates = new List<ICoordinate>() {
                new TestCoordinate(0, 0),
                new TestCoordinate(0, 10),
                new TestCoordinate(10, 10),
                new TestCoordinate(10, 0),
                new TestCoordinate(0, 0)
            };
            List<ICoordinate> innerCoordinates = new List<ICoordinate>() {
                new TestCoordinate(1, 1),
                new TestCoordinate(1, 20),
                new TestCoordinate(20, 20),
                new TestCoordinate(20, 1),
                new TestCoordinate(1, 1)
            };
            bool expected = true;
            bool actual;
            actual = ShapeMethods.PolygonInPolygon(innerCoordinates, masterCoordinates);
            Assert.AreEqual(expected, actual);

        }

        class TestCoordinate : ICoordinate
        {
            private double _latitude;
            private double _longitude;

            public TestCoordinate(double lat, double lon)
            {
                _latitude = lat;
                _longitude = lon;
            }

            #region ICoordinate Members

            public double Latitude
            {
                get
                {
                    return _latitude;
                }
            }

            public double Longitude
            {
                get
                {
                    return _longitude;
                }
            }

            #endregion
        }

        class TestCoordinateArea : ICoordinateArea
        {
            public double _latitude;
            public double _longitude;
            public double _minLongitude;
            public double _maxLongitude;
            public double _minLatitude;
            public double _maxLatitude;


            #region ICoordinateArea Members

            public double Latitude
            {
                get
                {
                    return _latitude;
                }
            }

            public double Longitude
            {
                get
                {
                    return _longitude;
                }
            }

            public double MinLongitude
            {
                get
                {
                    return _minLongitude;
                }
                set
                {
                    _minLongitude = value;
                }
            }

            public double MaxLongitude
            {
                get
                {
                    return _maxLongitude;
                }
                set
                {
                    _maxLongitude = value;
                }
            }

            public double MinLatitude
            {
                get
                {
                    return _minLatitude;
                }
            }

            public double MaxLatitude
            {
                get
                {
                    return _maxLatitude;
                }
            }

            #endregion
        }



        /// <summary>
        ///A test for BoxInBox
        ///</summary>
        [TestMethod()]
        public void BoxInBoxTest1()
        {
            ICoordinateArea mca = new TestCoordinateArea()
            {
                _maxLatitude = 10,
                _minLatitude = 0,
                _maxLongitude = 10,
                _minLongitude = 0,
            };
            ICoordinateArea ica = new TestCoordinateArea()
            {
                _maxLatitude = 9,
                _minLatitude = 1,
                _maxLongitude = 9,
                _minLongitude = 1,
            };
            bool expected = true;
            bool actual;
            actual = ShapeMethods.BoxInBox(mca, ica);
            Assert.AreEqual(expected, actual);
        }
    }
}
