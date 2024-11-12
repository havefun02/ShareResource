namespace ShareResource.Interfaces
{
    public interface ITokenService<T>
    {
        Task<T> GetTokenInfo(string token);
        Task<T> UpdateTokenAsync(T token);
    }
}
