using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface INavBarService:IServiceSupport
    {
        Task<long> AddAsync(long menuId, string menuName,string url,long sort);
        Task<bool> UpdateAsync(long id, string menuName, string url, long sort);
        Task<long> AddChildAsync(string menuName, string url, long sort, long parentId);
        Task<NavBarDTO> GetByIdAsync(long id);
        Task<NavBarDTO[]> GetByParentIdAsync(long id);
    }
}
