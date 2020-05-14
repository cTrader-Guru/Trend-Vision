/*  CTRADER GURU --> Indicator Template 1.0.6

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TrendVision : Indicator
    {

        #region Enums

        // --> Eventuali enumeratori li mettiamo qui

        #endregion

        #region Identity

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Trend Vision";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.1";

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/trend-vision/")]
        public string ProductInfo { get; set; }

        [Parameter("MA Type", Group = "Params", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        [Parameter("Deviation", Group = "Params", DefaultValue = 5, MinValue = 3)]
        public int Deviation { get; set; }

        [Parameter("Channel", Group = "Params", DefaultValue = 5, MinValue = 0)]
        public int Channel { get; set; }

        [Output("Bullish", LineColor = "LimeGreen", IsHistogram = true, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries Bullish { get; set; }

        [Output("Bearish", LineColor = "Red", IsHistogram = true, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries Bearish { get; set; }

        [Output("Flat", LineColor = "Gray", IsHistogram = true, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries Flat { get; set; }

        #endregion

        #region Property

        private MovingAverage _MA200;
        private MovingAverage _MA500;


        private Random _random = new Random();

        private int CandleWidth = 7;

        #endregion

        #region Indicator Events

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            _MA200 = Indicators.MovingAverage(Bars.ClosePrices, 200, MAType);
            _MA500 = Indicators.MovingAverage(Bars.ClosePrices, 500, MAType);

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {

            double k = (Channel * Symbol.PipSize);

            double Tchannel = Math.Round(_MA500.Result.LastValue + k, Symbol.Digits);
            double Bchannel = Math.Round(_MA500.Result.LastValue - k, Symbol.Digits);

            bool InChannel = (_MA200.Result.LastValue >= Bchannel && _MA200.Result.LastValue <= Tchannel);

            bool U = (_MA200.Result.LastValue < _MA500.Result.LastValue);
            bool F = (_MA200.Result.LastValue == _MA500.Result.LastValue);
            bool O = (_MA200.Result.LastValue > _MA500.Result.LastValue);

            bool FlatUnder = (_MA200.Result.LastValue >= _MA200.Result.Last(Deviation));
            bool FlatOver = (_MA200.Result.LastValue <= _MA200.Result.Last(Deviation));

            if (InChannel || (U && FlatUnder) || (O && FlatOver) || F)
            {

                Flat[index] = 2;
                Bullish[index] = 0;
                Bearish[index] = 0;

            }
            else if (O)
            {

                Bullish[index] = 2;
                Flat[index] = 0;
                Bearish[index] = 0;

            }
            else
            {

                Bearish[index] = 2;
                Bullish[index] = 0;
                Flat[index] = 0;

            }



        }

        #endregion

        #region Private Methods

        // --> Seguiamo la signature con underscore "_mioMetodo()"

        #endregion

    }

}
