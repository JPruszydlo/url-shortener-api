namespace ShortenerAPI.Services.Interfaces
{
    public interface IRecaptchaService
    {
        Task<bool> VerifyRecaptcha(string token);
    }
}