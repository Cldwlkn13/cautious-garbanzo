using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Account
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("user-id")]
        public int UserId { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("date-of-birth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone-number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("free-funds")]
        public double FreeFunds { get; set; }

        [JsonProperty("exposure")]
        public double Exposure { get; set; }

        [JsonProperty("commission-credit")]
        public double CommissionCredit { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("cashier-status")]
        public string CashierStatus { get; set; }

        [JsonProperty("casino-status")]
        public string CasinoStatus { get; set; }

        [JsonProperty("virtuals-status")]
        public string VirtualsStatus { get; set; }

        [JsonProperty("language-id")]
        public int LanguageId { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("currency-id")]
        public int CurrencyId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("odds-type-id")]
        public string OddsTypeId { get; set; }

        [JsonProperty("odds-type")]
        public string OddsType { get; set; }

        [JsonProperty("bet-confirmation")]
        public bool BetConfirmation { get; set; }

        [JsonProperty("display-p-and-l")]
        public bool DisplayPAndL { get; set; }

        [JsonProperty("exchange-type-id")]
        public string ExchangeTypeId { get; set; }

        [JsonProperty("exchange-type")]
        public string ExchangeType { get; set; }

        [JsonProperty("odds-rounding")]
        public bool OddsRounding { get; set; }

        [JsonProperty("bonus-code")]
        public string BonusCode { get; set; }

        [JsonProperty("deposit-terms")]
        public string DepositTerms { get; set; }

        [JsonProperty("user-security-question")]
        public UserSecurityQuestion UserSecurityQuestion { get; set; }

        [JsonProperty("mfa-enabled")]
        public bool MfaEnabled { get; set; }

        [JsonProperty("last-login")]
        public DateTime LastLogin { get; set; }

        [JsonProperty("registration-time")]
        public DateTime RegistrationTime { get; set; }

        [JsonProperty("bet-slip-pinned")]
        public bool BetSlipPinned { get; set; }

        [JsonProperty("marketing-consent")]
        public bool MarketingConsent { get; set; }

        [JsonProperty("first-exchange-bet")]
        public DateTime FirstExchangeBet { get; set; }

        [JsonProperty("last-exchange-bet")]
        public DateTime LastExchangeBet { get; set; }

        [JsonProperty("affordability-info-provided")]
        public bool AffordabilityInfoProvided { get; set; }

        [JsonProperty("employment-status")]
        public string EmploymentStatus { get; set; }

        [JsonProperty("proof-of-address-status")]
        public string ProofOfAddressStatus { get; set; }

        [JsonProperty("proof-of-identity-status")]
        public string ProofOfIdentityStatus { get; set; }

        [JsonProperty("edd-status")]
        public string EddStatus { get; set; }

        [JsonProperty("responsible-gambling-interaction")]
        public string ResponsibleGamblingInteraction { get; set; }

        [JsonProperty("terms-and-conditions-accepted")]
        public bool TermsAndConditionsAccepted { get; set; }
    }
}
