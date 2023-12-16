using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Abstractions.Services
{
    public interface IAuthService
    {
        public Task<ValidateTokenResponse> TokenValidation(string? token);
    }
}