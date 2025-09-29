using Cheting.Dtos;
using Cheting.Models;

namespace Cheting.Services
{
    public interface IAuthServices
    {
        User Register(RegisterRequestDto registerRequestDto);
    }
}