using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class IdNameService : IIdNameService
    {
        private IdNameDTO ToDTO(IdNameEntity entity)
        {
            IdNameDTO dto = new IdNameDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.Description = entity.Description;
            dto.TypeName = entity.TypeName;
            return dto;
        }
        public async Task<long> GetIdByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entitiy = await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == name);
                if(entitiy==null)
                {
                    return -1;
                }
                return entitiy.Id;
            }
        }
        public async Task<IdNameDTO[]> GetByTypeNameAsync(string typeName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<IdNameEntity>().Where(i => i.TypeName == typeName && i.IsNull == false);
                var result= await entities.OrderBy(i => i.Id).ToListAsync();
                return result.Select(i => ToDTO(i)).ToArray();
            }
        }
    }
}
