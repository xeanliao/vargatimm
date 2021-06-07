using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Area {
    public class MapColor : IGPSColor {
        private int _r;
        private int _g;
        private int _b;
        private double _a;

        public MapColor(int r, int g, int b, double a) {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public int R {
            get { return _r; }
            set { _r = value; }
        }
        public int G {
            get { return _g; }
            set { _g = value; }
        }
        public int B {
            get { return _b; }
            set { _b = value; }
        }
        public double A {
            get { return _a; }
            set { _a = value; }
        }


        /// <summary>
        /// Gets the pre-defined color Red.
        /// </summary>
        public static MapColor Red {
            get { return new MapColor(187, 0, 0, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Green.
        /// </summary>
        public static MapColor Green {
            get { return new MapColor(0, 255, 0, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Blue.
        /// </summary>
        public static MapColor Blue {
            get { return new MapColor(0, 0, 255, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Orange.
        /// </summary>
        public static MapColor Orange {
            get { return new MapColor(247, 86, 0, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Yellow.
        /// </summary>
        public static MapColor Yellow {
            get { return new MapColor(255, 255, 0, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Purple.
        /// </summary>
        public static MapColor Purple {
            get { return new MapColor(160, 32, 240, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Gray.
        /// </summary>
        public static MapColor Gray {
            get { return new MapColor(150, 150, 150, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color DarkGray.
        /// </summary>
        public static MapColor DarkGray {
            get {
                return new MapColor(50, 50, 50, 0.2);
            }
        }

        /// <summary>
        /// Gets the pre-defined color DarkGreen.
        /// </summary>
        public static MapColor DarkGreen {
            get {
                return new MapColor(0, 100, 0, 0.2);
            }
        }

        /// <summary>
        /// Gets the pre-defined color Pink.
        /// </summary>
        public static MapColor Pink {
            get {
                return new MapColor(249, 150, 246, 0.2);
            }
        }

        /// <summary>
        /// Gets the pre-defined color Brown.
        /// </summary>
        public static MapColor Brown {
            get { return new MapColor(165, 42, 42, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Azure.
        /// </summary>
        public static MapColor Azure {
            get { return new MapColor(240, 255, 255, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Cyan.
        /// </summary>
        public static MapColor Cyan {
            get { return new MapColor(0, 255, 255, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Aquamarine.
        /// </summary>
        public static MapColor Aquamarine {
            get { return new MapColor(127, 255, 212, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Maroon.
        /// </summary>
        public static MapColor Maroon {
            get { return new MapColor(176, 48, 96, 0.2); }
        }

        /// <summary>
        /// Gets the pre-defined color Chartreuse.
        /// </summary>
        public static MapColor Chartreuse {
            get { return new MapColor(127, 255, 0, 0.2); }
        }

        public static MapColor GetFillColor(Classifications classification) {
            MapColor color;
            switch (classification) {
                case Classifications.Z3:
                    color = GetColorByClassiValue(MapColorSettings.Z3FillColor);
                    break;
                case Classifications.Z5:
                    color = GetColorByClassiValue(MapColorSettings.Z5FillColor);
                    break;
                case Classifications.TRK:
                    color = GetColorByClassiValue(MapColorSettings.TractFillColor);
                    break;
                case Classifications.BG:
                    color = GetColorByClassiValue(MapColorSettings.BlockGroupFillColor);
                    break;
                case Classifications.CBSA:
                    color = GetColorByClassiValue(MapColorSettings.CbsaFillColor);
                    break;
                case Classifications.County:
                    color = GetColorByClassiValue(MapColorSettings.CountyFillColor);
                    break;
                case Classifications.SD_Elem:
                    color = GetColorByClassiValue(MapColorSettings.SdElemFillColor);
                    break;
                case Classifications.SD_Secondary:
                    color = GetColorByClassiValue(MapColorSettings.SdSecondaryFillColor);
                    break;
                case Classifications.SD_Unified:
                    color = GetColorByClassiValue(MapColorSettings.SdUnifiedFillColor);
                    break;
                case Classifications.SLD_House:
                    color = GetColorByClassiValue(MapColorSettings.SldHouseFillColor);
                    break;
                case Classifications.SLD_Senate:
                    color = GetColorByClassiValue(MapColorSettings.SldSenateFillColor);
                    break;
                case Classifications.Urban:
                    color = GetColorByClassiValue(MapColorSettings.UrbanFillColor);
                    break;
                default:
                    color = MapColor.Red;
                    break;
            }
            return color;
        }

        public static MapColor GetLineColor(Classifications classification) {

            MapColor color;
            switch (classification) {
                case Classifications.Z3:
                    color = GetColorByClassiValue(MapColorSettings.Z3OutlineColor);
                    break;
                case Classifications.Z5:
                    color = GetColorByClassiValue(MapColorSettings.Z5OutlineColor);
                    break;
                case Classifications.TRK:
                    color = GetColorByClassiValue(MapColorSettings.TractOutlineColor);
                    break;
                case Classifications.BG:
                    color = GetColorByClassiValue(MapColorSettings.BlockGroupOutlineColor);
                    break;
                case Classifications.CBSA:
                    color = GetColorByClassiValue(MapColorSettings.CbsaOutlineColor);
                    break;
                case Classifications.County:
                    color = GetColorByClassiValue(MapColorSettings.CountyOutlineColor);
                    break;
                case Classifications.SD_Elem:
                    color = GetColorByClassiValue(MapColorSettings.SdElemOutlineColor);
                    break;
                case Classifications.SD_Secondary:
                    color = GetColorByClassiValue(MapColorSettings.SdSecondaryOutlineColor);
                    break;
                case Classifications.SD_Unified:
                    color = GetColorByClassiValue(MapColorSettings.SdUnifiedOutlineColor);
                    break;
                case Classifications.SLD_House:
                    color = GetColorByClassiValue(MapColorSettings.SldHouseOutlineColor);
                    break;
                case Classifications.SLD_Senate:
                    color = GetColorByClassiValue(MapColorSettings.SldSenateOutlineColor);
                    break;
                case Classifications.Urban:
                    color = GetColorByClassiValue(MapColorSettings.UrbanOutlineColor);
                    break;
                default:
                    color = MapColor.Red;
                    break;
            }
            return color;
        }

        /// <summary>
        /// Set fill color and outline color of shape by classification value
        /// </summary>
        /// <param name="strColor">String color</param>
        /// <returns>GPS.Map.MapColor</returns>
        public static MapColor GetColorByClassiValue(string strColor) {
            MapColor color = null;
            switch (strColor) {
                case "Red":
                    color = MapColor.Red;
                    break;
                case "Green":
                    color = MapColor.Green;
                    break;
                case "Blue":
                    color = MapColor.Blue;
                    break;
                case "Orange":
                    color = MapColor.Orange;
                    break;
                case "Yellow":
                    color = MapColor.Yellow;
                    break;
                case "Purple":
                    color = MapColor.Purple;
                    break;
                case "Gray":
                    color = MapColor.Gray;
                    break;
                case "DarkGray":
                    color = MapColor.DarkGray;
                    break;
                case "DarkGreen":
                    color = MapColor.DarkGreen;
                    break;
                case "Pink":
                    color = MapColor.Pink;
                    break;
                case "Brown":
                    color = MapColor.Brown;
                    break;
                case "Azure":
                    color = MapColor.Azure;
                    break;
                case "Cyan":
                    color = MapColor.Cyan;
                    break;
                case "Aquamarine":
                    color = MapColor.Aquamarine;
                    break;
                case "Maroon":
                    color = MapColor.Maroon;
                    break;
                case "Chartreuse":
                    color = MapColor.Chartreuse;
                    break;
                default:
                    color = MapColor.Red;
                    break;
            }
            return color;
        }
    }
}
