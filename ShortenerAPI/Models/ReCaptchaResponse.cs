namespace ShortenerAPI.Models
{
    public class ReCaptchaResponse
    {
        public bool Success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string apk_package_name { get; set; }
        public string HostName { get; set; }
        public string[] ErrorCodes { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }
        public override string ToString()
        {
            string result = "reCaptcehaResponse\n";
            result += $"Success={Success}\n";
            result += $"ErrorCodes={ErrorCodes}\n";
            result += $"Score={Score}\n";
            result += $"Action={Action}\n";
            result += $"challenge_ts={challenge_ts}\n";
            result += $"apk_package_name={apk_package_name}\n";
            result += $"HostName={HostName}\n";
            return result;
        }
    }
}
