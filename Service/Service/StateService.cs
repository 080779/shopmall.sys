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
    public class StateService : IStateService
    {
        public StateDTO ToDTO(StateEntity entity)
        {
            StateDTO dto = new StateDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            return dto;
        }
        public async Task<StateDTO[]> GetModelListAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var states = await dbc.GetAll<StateEntity>().ToListAsync();
                return states.Select(s => ToDTO(s)).ToArray();
            }
        }
    }
}
