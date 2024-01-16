namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class FlatParams
    {
        public bool IsTurf { get; set; }
        public bool IsSprintDistance { get; set; }
        public bool IsMileDistance { get; set; }
        public bool IsMidDistance { get; set; }
        public bool IsHandicap { get; set; }
        public bool IsMaidenOrNovice { get; set; }
        public double PlacedTime { get; set; }
        public double TimeToRace { get; set; }
        public bool IsPlacedBeforeShow { get; set; }
        public int RaceRunnerCount { get; set; }
        public bool IsShortPrice { get; set; }
        public bool IsMidPrice { get; set; }

        public FlatParams(RunnerPriceOverview rpo)
        {
            var furlongs = Furlongs(rpo.MarketCatalogue.MarketName);

            IsTurf = false;
            IsSprintDistance = GetIsSprintDistance(furlongs);
            IsMileDistance = GetIsMileDistance(furlongs);
            IsMidDistance = GetIsMidDistance(furlongs);
            IsHandicap = GetIsHandicap(rpo.MarketCatalogue.MarketName);
            IsMaidenOrNovice = GetIsMaidenOrNovice(rpo.MarketCatalogue.MarketName);
            PlacedTime = GetPlacedTime();
            TimeToRace = GetTimeToRace(rpo.MarketDetail.marketStartTime);
            IsPlacedBeforeShow = GetIsPlacedBeforeShow(TimeToRace);
            RaceRunnerCount = rpo.MarketDetail.runnerDetails.Count(r => r.runnerStatus == "ACTIVE" && 
                                                                        r.runnerOrder < 98);
            IsShortPrice = GetIsShortPrice(rpo.SportsbookRunner.winRunnerOdds.@decimal);
            IsMidPrice = GetIsMidPrice(rpo.SportsbookRunner.winRunnerOdds.@decimal);
        }

        private static bool GetIsSprintDistance(int furlongs)
        {
            return furlongs <= 6;
        }

        private static bool GetIsMileDistance(int furlongs)
        {
            return furlongs > 6 && furlongs <= 9;
        }

        private static bool GetIsMidDistance(int furlongs)
        {
            return furlongs > 9 && furlongs <= 12;
        }

        private static bool GetIsHandicap(string marketName)
        {
            return marketName.Contains("Hcap");
        }

        private static bool GetIsMaidenOrNovice(string marketName)
        {
            return marketName.Contains("Mdn") || marketName.Contains("Nov");
        }

        private static double GetPlacedTime()
        {
            return DateTime.Now.TimeOfDay.TotalMinutes / (double)(24 * 60);
        }

        private static double GetTimeToRace(DateTime raceTime)
        {
            return DateTime.Now.TimeOfDay > raceTime.TimeOfDay ?
                0 : (raceTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes / (double)(24 * 60);
        }

        private static bool GetIsPlacedBeforeShow(double timeToRace)
        {
            return timeToRace > 45 / (double)(24 * 60);
        }

        private static bool GetIsShortPrice(double price)
        {
            return price <= 3;
        }
        private static bool GetIsMidPrice(double price)
        {
            return price > 3 && price <= 11;
        }

        private static int Furlongs(string marketName)
        {
            var stub = marketName.Substring(0, marketName.IndexOf(" "));
            var milesInFurlongs = MilesInFurlongs(stub);
            var partFurlongs = PartFurlongs(stub);
            
            return milesInFurlongs + partFurlongs;
        }

        private static int MilesInFurlongs(string stub)
        {
            if (stub.Contains("m"))
            {
                var miles = stub.Substring(stub.IndexOf("m") - 1, 1);
                int.TryParse(miles, out var result);
                return result * 8;
            }
            return 0;
        }

        private static int PartFurlongs(string stub)
        {
            if (stub.Contains("f"))
            {
                var furlongs = stub.Substring(stub.IndexOf("f") - 1, 1);
                int.TryParse(furlongs, out var result);
                return result;
            }
            return 0;
        }
    }
}
