using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IUserService : IServiceSupport
    {
        Task<long> AddAsync();
    }
    public class UserSearchResult
    {
        public AdminDTO[] Admins { get; set; }
        public long TotalCount { get; set; }
    }
}
