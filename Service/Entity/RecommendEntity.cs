using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    ///推荐实体类
    /// </summary>
    public class RecommendEntity : BaseEntity
    {
        public long UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public string RecommendMobile { get; set; }
        public long RecommendId { get; set; }
        public UserEntity Recommend { get; set; }
        public string RecommendPath { get; set; }
        public int RecommendGenera { get; set; } = 1;
        public bool IsNull { get; set; } = false;
    }
}
