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
    public class JournalTypeService : IJournalTypeService
    {
        public JournalTypeDTO ToDTO(JournalTypeEntity entity)
        {
            JournalTypeDTO dto = new JournalTypeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Customer = entity.Customer;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Merchant = entity.Merchant;
            dto.Name = entity.Name;
            dto.Platform = entity.Platform;
            return dto;
        }
        public async Task<long?> GetIdByDescAsync(string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.GetAll<JournalTypeEntity>().SingleOrDefaultAsync(j=>j.Description==description);
                if(type==null)
                {
                    return null;
                }
                return type.Id;
            }
        }

        public async Task<JournalTypeDTO[]> GetModelListAsync(string userTypeName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var types = dbc.GetAll<JournalTypeEntity>();
                List<JournalTypeEntity> typesRes;
                if (userTypeName == "平台")
                {
                    typesRes = await types.ToListAsync();
                }
                else
                {
                    typesRes = await types.Where(j=>j.Name!= "积分增加").ToListAsync();
                }
                return typesRes.Select(j=>ToDTO(j)).ToArray();
            }
        }

        public async Task<JournalTypeDTO[]> GetModelListAsync(bool platform)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var types = dbc.GetAll<JournalTypeEntity>();
                List<JournalTypeEntity> typesRes;
                typesRes = await types.Where(j => j.Name == "平台发放" || j.Name == "平台扣除" || j.Name == "积分增加").ToListAsync();
                return typesRes.Select(j => ToDTO(j)).ToArray();
            }
        }
    }
}
