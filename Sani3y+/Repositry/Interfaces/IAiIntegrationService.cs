namespace Sani3y_.Repositry.Interfaces
{
    public interface IAiIntegrationService
    {
        Task<string> GetPredictedProfessionAsync(string userQuery);
    }
}
