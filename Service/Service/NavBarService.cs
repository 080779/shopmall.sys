using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMS.IService;
using IMS.DTO;

namespace IMS.Service.Service
{
    public class NavBarService : INavBarService
    {
        public async Task<long> AddAsync(long menuId, string menuName, string url, long sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NavBarEntity entity = new NavBarEntity();
                entity.MenuId = menuId;
                entity.MenuName = menuName;
                entity.Url = url;
                entity.ParentId = 0;
                entity.Sort = sort;
                dbc.NavBars.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        public async Task<bool> UpdateAsync(long id, string menuName, string url, long sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = dbc.GetAll<NavBarEntity>().SingleOrDefault(n=>n.Id==id);
                if(entity==null)
                {
                    return false;
                }
                entity.MenuName = menuName;
                entity.Url = url;
                entity.Sort = sort;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
        public async Task<long> AddChildAsync(string menuName, string url, long sort, long parentId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NavBarEntity entity = new NavBarEntity();
                entity.MenuId = 0;
                entity.MenuName = menuName;
                entity.Url = url;
                entity.Sort = sort;
                entity.ParentId = parentId;
                dbc.NavBars.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        public async Task<NavBarDTO> GetByIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<NavBarEntity>().SingleOrDefaultAsync(n => n.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }
        public async Task<NavBarDTO[]> GetByParentIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var navbars = dbc.GetAll<NavBarEntity>().Where(n=>n.ParentId==id);

                //Expression<Func<NavBarEntity, NavBarDTO>> func = n => ToDTO(n);

                var entities= await navbars.OrderBy(n => n.Sort).ToArrayAsync();
                return entities.Select(n=>ToDTO(n)).ToArray();
            }
        }
        private NavBarDTO ToDTO(NavBarEntity entity)
        {
            NavBarDTO dto = new NavBarDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.MenuName = entity.MenuName;
            dto.ParentId = entity.ParentId;
            dto.Sort = entity.Sort;
            dto.Url = entity.Url;
            dto.MenuId = entity.MenuId;
            dto.IsLock = entity.IsLock;
            return dto;
        }
    }
}
