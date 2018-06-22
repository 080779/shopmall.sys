using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface ILevelTypeService : IServiceSupport
    {
        Task<LevelTypeDTO[]> GetModelListAsync();
        Task<bool> UpdateAsync(KeyValuePair<long,string>[] keyValues);
    }
}
