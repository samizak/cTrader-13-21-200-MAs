using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, AutoRescale = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ThreeMAs : Indicator
    {

        #region Parameters
        [Parameter("Source")]
        public DataSeries DataSource { get; set; }

        [Parameter("Timeframe", DefaultValue = "Daily")]
        public TimeFrame Timeframe { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MaType { get; set; }

        [Parameter(name: "Change Bar Colour?", DefaultValue = true)]
        public bool ChangeBarCol { get; set; }

        [Parameter(name: "MA Period 1", Group = "MA Periods", DefaultValue = 13, MinValue = 1)]
        public int Period1 { get; set; }

        [Parameter(name: "MA Period 2", Group = "MA Periods", DefaultValue = 21, MinValue = 1)]
        public int Period2 { get; set; }

        [Parameter(name: "MA Period 3", Group = "MA Periods", DefaultValue = 200, MinValue = 1)]
        public int Period3 { get; set; }

        [Parameter(name: "Bearish", Group = "Bar Colours", DefaultValue = "OrangeRed")]
        public string BearishColour { get; set; }

        [Parameter(name: "Bullish", Group = "Bar Colours", DefaultValue = "Green")]
        public string BullishColour { get; set; }
        #endregion

        #region Outputs
        [Output("MA 0", LineColor = "Gray", Thickness = 2)]
        public IndicatorDataSeries Ma0 { get; set; }

        [Output("MA 1", LineColor = "Green", Thickness = 2)]
        public IndicatorDataSeries Ma1 { get; set; }

        [Output("MA 2", LineColor = "Yellow", Thickness = 2)]
        public IndicatorDataSeries Ma2 { get; set; }

        [Output("MA 3", LineColor = "Red", Thickness = 2)]
        public IndicatorDataSeries Ma3 { get; set; }
        #endregion

        private Bars _series;
        private MovingAverage _ma1;
        private MovingAverage _ma2;
        private MovingAverage _ma3;

        protected override void Initialize()
        {
            _series = MarketData.GetBars(Timeframe);
            _ma1 = Indicators.MovingAverage(_series.ClosePrices, Period1, MaType);
            _ma2 = Indicators.MovingAverage(_series.ClosePrices, Period2, MaType);
            _ma3 = Indicators.MovingAverage(_series.ClosePrices, Period3, MaType);
        }

        public override void Calculate(int index)
        {
            int timeframeIndex = _series.OpenTimes.GetIndexByTime(Bars.OpenTimes[index]);

            Ma1[index] = _ma1.Result[timeframeIndex];
            Ma2[index] = _ma2.Result[timeframeIndex];
            Ma3[index] = _ma3.Result[timeframeIndex];
            Ma0[index] = 0.5 * (Ma1[index] + Ma2[index]);

            if (!ChangeBarCol)
                return;
            Color barColour = Ma1[index] > Ma2[index] ? BullishColour.ToString() : BearishColour.ToString();
            Chart.SetBarColor(index, barColour);
        }
    }
}
