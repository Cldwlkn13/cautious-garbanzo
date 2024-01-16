using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Interfaces;

namespace Betfair.ExchangeComparison.ExpectedValueModel
{
    public class FlatModel : IExpectedValueModelFlat
    {
        public FlatModel() 
        { 
        }

        public static Dictionary<string, double> Coefficients
        {
            get
            {
                return new Dictionary<string, double>
                {
                    { "Intercept", -0.1640 },
                    { "IsTurf", 0.0228 },
                    { "IsSprintDistance", -0.0144 },
                    { "IsMileDistance", 0.0446 },
                    { "IsMidDistance", 0.0531 },
                    { "IsHandicap", 0.0382 },
                    { "PlacedTime", 0.0486 },
                    { "IsMaidenOrNovice", 0.1160 },
                    { "IsPlacedBeforeShow", 0.1364 },
                    { "TimeToRace", -0.1413 },
                    { "RaceRunnerCount", -0.0002 },
                    { "IsShortPrice", -0.0508 },
                    { "IsMidPrice", 0.0030 },
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
            if (parameters is not FlatParams flatParams)
            {
                return -100;
            }

            double result = 0;
            var coefficients = Coefficients;

            result += coefficients["Intercept"];
            result += coefficients["IsTurf"] * Convert.ToInt32(flatParams.IsTurf);
            result += coefficients["IsSprintDistance"] * Convert.ToInt32(flatParams.IsSprintDistance);
            result += coefficients["IsMileDistance"] * Convert.ToInt32(flatParams.IsMileDistance);
            result += coefficients["IsMidDistance"] * Convert.ToInt32(flatParams.IsMidDistance);
            result += coefficients["IsHandicap"] * Convert.ToInt32(flatParams.IsHandicap);
            result += coefficients["IsMaidenOrNovice"] * Convert.ToInt32(flatParams.IsMaidenOrNovice);
            result += coefficients["PlacedTime"] * flatParams.PlacedTime;
            result += coefficients["TimeToRace"] * flatParams.TimeToRace;
            result += coefficients["IsPlacedBeforeShow"] * Convert.ToInt32(flatParams.IsPlacedBeforeShow);
            result += coefficients["IsShortPrice"] * Convert.ToInt32(flatParams.IsShortPrice);
            result += coefficients["RaceRunnerCount"] * Convert.ToInt32(flatParams.RaceRunnerCount);
            result += coefficients["IsMidPrice"] * Convert.ToInt32(flatParams.IsMidPrice);

            return result;
        }

        private static double CalculateOutcome(double expectedValue, double price)
        {
            return ((-expectedValue) * price) + price;
        }
    }
}
