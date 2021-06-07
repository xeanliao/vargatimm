using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TIMM.GPS.ControlCenter.Model
{
    public class PrintOption : ModelBase
    {
        public PrintOption()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                this.DMaps = new ObservableCollection<DistributionMapUI>();
                this.DMaps.Add(new DistributionMapUI()
                {
                    IsChecked = true,
                    Name = "AAA"
                });
            }
        }

        private bool m_IsUseMapRoadMode;
        public bool IsUseMapRoadMode
        {
            get { return m_IsUseMapRoadMode; }
            set
            {
                if (m_IsUseMapRoadMode == value)
                    return;
                m_IsUseMapRoadMode = value;
                RaisePropertyChanged("IsUseMapRoadMode");
                RaisePropertyChanged("IsUseMapAerialMode");
            }
        }
        public bool IsUseMapAerialMode
        {
            get
            {
                return !m_IsUseMapRoadMode;
            }
            set
            {
                m_IsUseMapRoadMode = !value;
                RaisePropertyChanged("IsUseMapRoadMode");
                RaisePropertyChanged("IsUseMapAerialMode");
            }
        }
        
        public bool SuppressCover { get; set; }

        private bool _suppressLocations;

        public bool SuppressLocations
        {
            get { return _suppressLocations; }
            set 
            { 
                _suppressLocations = value;
                RaisePropertyChanged("SuppressLocations");
            }
        }
        
        public bool SuppressRadii { get; set; }
        public bool SuppressNDAreasForCampaignMap { get; set; }
        public bool SuppressNDAreasForDistributionmap { get; set; }
        public bool SuppressCampaignSummary { get; set; }
        public bool SuppressSummaryOfSubmap { get; set; }

        public bool SuppressSubMaps { get; set; }
        public bool SuppressSubMapDetails { get; set; }
        public bool SuppressDMaps { get; set; }
        public bool SuppressGTU { get; set; }

        private int m_GTUDotRadii;
        public int GTUDotRadii
        {
            get
            {
                return m_GTUDotRadii;
            }
            set
            {
                m_GTUDotRadii = value;
                RaisePropertyChanged("DisplayGTUDotRadii");
            }
        }
        public string DisplayGTUDotRadii
        {
            get
            {
                int meters = (int)((double)GTUDotRadii / 1000d * 1609.344d);

                return string.Format("{0} meters", meters);
            }
        }

        public bool SuppressClassificationOutlines { get; set; }
        private bool m_ShowPenetrationColors;
        public bool ShowPenetrationColors
        {
            get { return m_ShowPenetrationColors; }
            set
            {
                if (m_ShowPenetrationColors == value)
                    return;
                m_ShowPenetrationColors = value;
                RaisePropertyChanged("CustomColorsVisibility");
            }
        }

        #region Destribution Maps Option

        private Visibility _isShowDMaps;
        public Visibility IsShowDMaps
        {
            get { return _isShowDMaps; }
            set
            {
                _isShowDMaps = value;
                RaisePropertyChanged("IsShowDMaps");
            }
        }

        private ObservableCollection<DistributionMapUI> _DMaps;
        public ObservableCollection<DistributionMapUI> DMaps
        {
            get { return _DMaps; }
            set
            {
                _DMaps = value;
                RaisePropertyChanged("DMaps");
            }
        }
        
        #endregion

        #region Report Option

        private bool _isClientMap;

        /// <summary>
        /// Control show the client (edit) map or distribute (unedit) map
        /// Ture - client(edit) map: show all dots (modified points) in the boundary
        /// False - distribute(unedit) map: show original raw data (include both inside and outside)
        /// </summary>
        public bool IsClientMap
        {
            get { return _isClientMap; }
            set
            {                
                _isClientMap = value;
                RaisePropertyChanged("IsClientMap");
            }
        }

        #endregion

        private bool m_UseCustomColors;
        public bool UseCustomColors
        {
            get { return m_UseCustomColors; }
            set
            {
                if (m_UseCustomColors == value)
                    return;
                m_UseCustomColors = value;
                RaisePropertyChanged("CustomColorsVisibility");
            }
        }

        public Visibility CustomColorsVisibility
        {
            get
            {
                if (UseCustomColors && ShowPenetrationColors)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public List<PercentItem> CustomColors = new List<PercentItem>();

        #region Blue
        private bool m_IsBlueEnabled;
        public bool IsBlueEnabled
        {
            get
            {
                return m_IsBlueEnabled;
            }
            set
            {
                m_IsBlueEnabled = value;
                if (m_IsBlueEnabled == false)
                {
                    BlueMin = null;
                    BlueMax = null;
                }
                RaisePropertyChanged("BlueMin");
                RaisePropertyChanged("BlueMax");
            }
        }

        private double? m_BlueMin;
        public double? BlueMin
        {
            get
            {
                return m_BlueMin;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_BlueMax = null;
                    RaisePropertyChanged("BlueMin");
                    return;
                }

                if (value != m_BlueMin)
                {
                    m_BlueMin = value;
                    RaisePropertyChanged("BlueMin");
                }
            }
        }
        private double? m_BlueMax;
        public double? BlueMax
        {
            get
            {
                return m_BlueMax;
            }
            set
            {
                if (value.HasValue && value != -1d && value <0d && value > 100d)
                {
                    m_BlueMax = null;
                    RaisePropertyChanged("BlueMax");
                    return;
                }

                if (value != m_BlueMax)
                {
                    m_BlueMax = value;
                    RaisePropertyChanged("BlueMax");
                }
            }
        }
        #endregion

        #region Green
        private bool m_IsGreenEnabled;
        public bool IsGreenEnabled
        {
            get
            {
                return m_IsGreenEnabled;
            }
            set
            {
                m_IsGreenEnabled = value;
                if (m_IsGreenEnabled == false)
                {
                    GreenMin = null;
                    GreenMax = null;
                }
                RaisePropertyChanged("GreenMin");
                RaisePropertyChanged("GreenMax");
            }
        }

        private double? m_GreenMin;
        public double? GreenMin
        {
            get
            {
                return m_GreenMin;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_GreenMax = null;
                    RaisePropertyChanged("GreenMin");
                    return;
                }

                if (value != m_GreenMin)
                {
                    m_GreenMin = value;
                    RaisePropertyChanged("GreenMin");
                }
            }
        }
        private double? m_GreenMax;
        public double? GreenMax
        {
            get
            {
                return m_GreenMax;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_GreenMax = null;
                    RaisePropertyChanged("GreenMax");
                    return;
                }

                if (value != m_GreenMax)
                {
                    m_GreenMax = value;
                    RaisePropertyChanged("GreenMax");
                }
            }
        }
        #endregion

        #region Yellow
        private bool m_IsYellowEnabled;
        public bool IsYellowEnabled
        {
            get
            {
                return m_IsYellowEnabled;
            }
            set
            {
                m_IsYellowEnabled = value;
                if (m_IsYellowEnabled == false)
                {
                    YellowMin = null;
                    YellowMax = null;
                }
                RaisePropertyChanged("YellowMin");
                RaisePropertyChanged("YellowMax");
            }
        }

        private double? m_YellowMin;
        public double? YellowMin
        {
            get
            {
                return m_YellowMin;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_YellowMax = null;
                    RaisePropertyChanged("YellowMin");
                    return;
                }

                if (value != m_YellowMin)
                {
                    m_YellowMin = value;
                    RaisePropertyChanged("YellowMin");
                }
            }
        }
        private double? m_YellowMax;
        public double? YellowMax
        {
            get
            {
                return m_YellowMax;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_YellowMax = null;
                    RaisePropertyChanged("YellowMax");
                    return;
                }

                if (value != m_YellowMax)
                {
                    m_YellowMax = value;
                    RaisePropertyChanged("YellowMax");
                }
            }
        }
        #endregion

        #region Orange
        private bool m_IsOrangeEnabled;
        public bool IsOrangeEnabled
        {
            get
            {
                return m_IsOrangeEnabled;
            }
            set
            {
                m_IsOrangeEnabled = value;
                if (m_IsOrangeEnabled == false)
                {
                    OrangeMin = null;
                    OrangeMax = null;
                }
                RaisePropertyChanged("OrangeMin");
                RaisePropertyChanged("OrangeMax");
            }
        }

        private double? m_OrangeMin;
        public double? OrangeMin
        {
            get
            {
                return m_OrangeMin;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_OrangeMax = null;
                    RaisePropertyChanged("OrangeMin");
                    return;
                }

                if (value != m_OrangeMin)
                {
                    m_OrangeMin = value;
                    RaisePropertyChanged("OrangeMin");
                }
            }
        }
        private double? m_OrangeMax;
        public double? OrangeMax
        {
            get
            {
                return m_OrangeMax;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_OrangeMax = null;
                    RaisePropertyChanged("OrangeMax");
                    return;
                }

                if (value != m_OrangeMax)
                {
                    m_OrangeMax = value;
                    RaisePropertyChanged("OrangeMax");
                }
            }
        }
        #endregion

        #region Red
        private bool m_IsRedEnabled;
        public bool IsRedEnabled
        {
            get
            {
                return m_IsRedEnabled;
            }
            set
            {
                m_IsRedEnabled = value;
                if (m_IsRedEnabled == false)
                {
                    RedMin = null;
                    RedMax = null;
                }
                RaisePropertyChanged("RedMin");
                RaisePropertyChanged("RedMax");
            }
        }

        private double? m_RedMin;
        public double? RedMin
        {
            get
            {
                return m_RedMin;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_RedMax = null;
                    RaisePropertyChanged("RedMin");
                    return;
                }

                if (value != m_RedMin)
                {
                    m_RedMin = value;
                    RaisePropertyChanged("RedMin");
                }
            }
        }
        private double? m_RedMax;
        public double? RedMax
        {
            get
            {
                return m_RedMax;
            }
            set
            {
                if (value.HasValue && value != -1d && value < 0d && value > 100d)
                {
                    m_RedMax = null;
                    RaisePropertyChanged("RedMax");
                    return;
                }

                if (value != m_RedMax)
                {
                    m_RedMax = value;
                    RaisePropertyChanged("RedMax");
                }
            }
        }
        #endregion

        public void SetPercentageColor(List<CampaignPercentageColorUI> colorDefine)
        {
            if (colorDefine != null && colorDefine.Count > 0)
            {
                var blue = colorDefine.FirstOrDefault(i => i.ColorId == 1);
                if (blue != null && blue.Max >= 0 && blue.Min >= 0)
                {
                    IsBlueEnabled = true;
                    BlueMin = blue.Min * 100d;
                    BlueMax = blue.Max * 100d;
                }
                else
                {
                    IsBlueEnabled = false;
                    BlueMin = null;
                    BlueMax = null;
                }

                var green = colorDefine.FirstOrDefault(i => i.ColorId == 2);
                if (green != null && green.Max >= 0 && green.Min >= 0)
                {
                    IsGreenEnabled = true;
                    GreenMin = green.Min * 100d;
                    GreenMax = green.Max * 100d;
                }
                else
                {
                    IsGreenEnabled = false;
                    GreenMin = null;
                    GreenMax = null;
                }

                var yellow = colorDefine.FirstOrDefault(i => i.ColorId == 3);
                if (yellow != null && yellow.Max >= 0 && yellow.Min >= 0)
                {
                    IsYellowEnabled = true;
                    YellowMin = yellow.Min * 100d;
                    YellowMax = yellow.Max * 100d;
                }
                else
                {
                    IsYellowEnabled = false;
                    YellowMin = null;
                    YellowMax = null;
                }

                var orange = colorDefine.FirstOrDefault(i => i.ColorId == 4);
                if (orange != null && orange.Max >= 0 && orange.Min >= 0)
                {
                    IsOrangeEnabled = true;
                    OrangeMin = orange.Min * 100d;
                    OrangeMax = orange.Max * 100d;
                }
                else
                {
                    IsOrangeEnabled = false;
                    OrangeMin = null;
                    OrangeMax = null;
                }

                var red = colorDefine.FirstOrDefault(i => i.ColorId == 5);
                if (red != null && red.Max >= 0 && red.Min >= 0)
                {
                    IsRedEnabled = true;
                    RedMin = red.Min * 100d;
                    RedMax = red.Max * 100d;
                }
                else
                {
                    IsRedEnabled = false;
                    RedMin = null;
                    RedMax = null;
                }
            }
            else
            {
                IsBlueEnabled = true;
                IsGreenEnabled = true;
                IsYellowEnabled = true;
                IsOrangeEnabled = true;
                IsRedEnabled = true;

                BlueMin = 0;
                BlueMax = 20;
                GreenMin = 20;
                GreenMax = 40;
                YellowMin = 40;
                YellowMax = 60;
                OrangeMin = 60;
                OrangeMax = 80;
                RedMin = 80;
                RedMax = 100;
            }
        }

        public List<CampaignPercentageColorUI> GetPercentageColor()
        {
            if (!UseCustomColors)
            {
                return null;
            }
            List<CampaignPercentageColorUI> result = new List<CampaignPercentageColorUI>();

            #region Blue
            if (IsBlueEnabled)
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 1,
                    Max = BlueMax.Value / 100d,
                    Min = BlueMin.Value / 100d
                });
            }
            else
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 1,
                    Max = -1,
                    Min = -1
                });
            }
            #endregion

            #region Green
            if (IsGreenEnabled)
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 2,
                    Max = GreenMax.Value / 100d,
                    Min = GreenMin.Value / 100d
                });
            }
            else
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 2,
                    Max = -1,
                    Min = -1
                });
            }
            #endregion

            #region Yellow
            if (IsYellowEnabled)
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 3,
                    Max = YellowMax.Value / 100d,
                    Min = YellowMin.Value / 100d
                });
            }
            else
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 3,
                    Max = -1,
                    Min = -1
                });
            }
            #endregion

            #region Orange
            if (IsOrangeEnabled)
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 4,
                    Max = OrangeMax.Value / 100d,
                    Min = OrangeMin.Value / 100d
                });
            }
            else
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 4,
                    Max = -1,
                    Min = -1
                });
            }
            #endregion

            #region Red
            if (IsRedEnabled)
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 5,
                    Max = RedMax.Value / 100d,
                    Min = RedMin.Value / 100d
                });
            }
            else
            {
                result.Add(new CampaignPercentageColorUI
                {
                    ColorId = 5,
                    Max = -1,
                    Min = -1
                });
            }
            #endregion

            return result;
        }
    }

    public class PercentItem : ModelBase
    {
        private bool m_IsChecked;
        public bool IsChecked
        {
            get { return m_IsChecked; }
            set
            {
                m_IsChecked = value;
            }
        }

        private SolidColorBrush m_FillColor;
        public SolidColorBrush FillColor
        {
            get { return m_FillColor; }
            set
            {
                m_FillColor = value;
            }
        }

        private string m_DisplayName;
        public string DisplayName
        {
            get { return m_DisplayName; }
            set
            {
                m_DisplayName = value;
            }
        }
        

        private double? m_Min;
        public double? Min
        {
            get { return m_Min; }
            set
            {
                m_Min = value;
            }
        }

        private double? m_Max;
        public double? Max
        {
            get { return m_Max; }
            set
            {
                m_Max = value;
            }
        }
        
    }
}
