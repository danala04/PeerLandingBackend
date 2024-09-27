using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);
        Task<List<ResUserDto>> GetAllUser();
        Task<ResUserByIdDto> GetUserById(string userId);
        Task<ResLoginDto> Login(ReqLoginDto reqLogin);
        Task<string> Update(string userId, ReqUpdateUserDto reqUpdate);
        Task<string> Delete(string userId);
    }
}
