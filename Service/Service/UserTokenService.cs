using IMS.Common;
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
    public class UserTokenService : IUserTokenService
    {
        public long CheckToken(long userId, string token)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var userToken= dbc.GetAll<UserTokenEntity>().SingleOrDefault(u=>u.UserId==userId);
                if(userToken==null)
                {
                    return -1;
                }
                if(string.IsNullOrEmpty(userToken.Token))
                {
                    return -2;
                }
                if(userToken.Token!=token)
                {
                    return -3;
                }
                return 1;
            }
        }
    }
}
