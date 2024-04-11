using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Interfaces;

namespace Betfair.ExchangeComparison.ExpectedValueModel
{
    public class JumpsModel : IExpectedValueModelJumps
    {
        public static Dictionary<string, double> Coefficients
        {
            get
            {
                return new Dictionary<string, double>
                {
                    { "Intercept", -0.1571 },
                    { "IsShortDistance", 0.0239 },
                    { "IsMidDistance", 0.0022 },
                    { "IsChase", 0.00559 },
                    { "IsHurdle", 0.0264 },
                    { "IsHandicap", -0.0079 },
                    { "IsMaidenOrNovice", -0.0243 },
                    { "PlacedTime", 0.0647 },
                    { "IsPlacedBeforeShow", 0.0471 },
                    { "TimeToRace", 0.2352 },
                    { "RaceRunnerCount", 0.00056 },
                    { "IsShortPrice", 0.0527 },
                    { "IsMidPrice", 0.07036 },
                };
            }
        }

        public double ModelOutcome<T>(T parameters, RunnerPriceOverview rpo)
        {
            var raceAndName = rpo.ToString();
            var expectedValue = CalculateExpectedValue(parameters);
            var outcome = CalculateOutcome(expectedValue, rpo.SportsbookRunner.winRunnerOdds.@decimal);

            return outcome;
        }

        public static double CalculateExpectedValue<T>(T parameters)
        {
            if (parameters is not JumpsParams jumpsParams)
            {
                return -100;
            }

            double result = 0;
            var coefficients = Coefficients;

            result += coefficients["Intercept"];
            result += coefficients["IsShortDistance"] * Convert.ToInt32(jumpsParams.IsShortDistance);
            result += coefficients["IsMidDistance"] * Convert.ToInt32(jumpsParams.IsMidDistance);
            result += coefficients["IsChase"] * Convert.ToInt32(jumpsParams.IsChase);
            result += coefficients["IsHurdle"] * Convert.ToInt32(jumpsParams.IsHurdle);
            result += coefficients["IsHandicap"] * Convert.ToInt32(jumpsParams.IsHandicap);
            result += coefficients["IsMaidenOrNovice"] * Convert.ToInt32(jumpsParams.IsMaidenOrNovice);
            result += coefficients["PlacedTime"] * jumpsParams.PlacedTime;
            result += coefficients["TimeToRace"] * jumpsParams.TimeToRace;
            result += coefficients["IsPlacedBeforeShow"] * Convert.ToInt32(jumpsParams.IsPlacedBeforeShow);
            result += coefficients["RaceRunnerCount"] * Convert.ToInt32(jumpsParams.RaceRunnerCount);
            result += coefficients["IsShortPrice"] * Convert.ToInt32(jumpsParams.IsShortPrice);
            result += coefficients["IsMidPrice"] * Convert.ToInt32(jumpsParams.IsMidPrice);

            return result;
        }

        private static double CalculateOutcome(double expectedValue, double price)
        {
            return ((-expectedValue) * price) + price;
        }
    }
}
