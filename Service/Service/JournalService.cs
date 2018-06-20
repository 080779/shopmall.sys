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
    public class JournalService : IJournalService
    {
        public JournalDTO ToDTO(JournalEntity entity)
        {
            JournalDTO dto = new JournalDTO();
            dto.Amount = entity.Amount;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;            
            dto.Id = entity.Id;
            dto.InIntegral = entity.InIntegral;
            dto.Integral = entity.Integral;
            dto.IntegralTypeId = entity.IntegralTypeId;
            dto.JournalTypeId = entity.JournalTypeId;
            dto.JournalTypeName = entity.JournalType.Name;
            dto.OutIntegral = entity.OutIntegral;            
            dto.Tip = entity.Tip;
            dto.FromPlatformUserId = entity.FormPlatformUserId;
            dto.FromPlatformUserMobile = entity.FormPlatformUser.Mobile;
            dto.ToPlatformUserId = entity.ToPlatformUserId;
            dto.ToPlatformUserMobile = entity.ToPlatformUser.Mobile;
            dto.IntegralTypeName = entity.IntegralType.Name;
            dto.ToIntegralTypeName = entity.ToIntegralType.Name;
            dto.Journal01 = entity.Journal01;            
            dto.PlatformUserId = entity.PlatformUserId;
            dto.PlatformUserMobile = entity.PlatformUser.Mobile;
            dto.ToPlatformUserCode = entity.ToPlatformUser.Code;
            dto.FormPlatformUserCode = entity.FormPlatformUser.Code;
            return dto;
        }
        public async Task<JournalSearchResult> GetModelListAsync(long? id, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                long journalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "消费").Id;
                var journals = dbc.GetAll<JournalEntity>();
                if (id != null)
                {
                    if (typeId == journalTypeId)
                    {
                        journals = journals.Where(j => j.PlatformUserId != j.ToPlatformUserId && j.ToPlatformUserId == id);
                    }

                }
                journals = journals.Where(j => j.JournalTypeId == journalTypeId && j.InIntegral == null);
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.PlatformUser.Mobile.Contains(mobile));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.PlatformUser.Code.Contains(code));
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                long givingIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "商家积分").Id;
                long useIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "消费积分").Id;
                result.TotalCount = await journals.LongCountAsync();
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == "PlatformUser201805051709360001");
                if (user != null)
                {
                    long platformUserId = user.Id;
                    result.GivingIntegrals = await journals.Where(j => j.ToIntegralTypeId == givingIntegralId && j.PlatformUserId == platformUserId).SumAsync(j => j.OutIntegral);
                    result.UseIntegrals = await journals.Where(j => j.ToIntegralTypeId == useIntegralId && j.PlatformUserId == platformUserId).SumAsync(j => j.OutIntegral);
                }
                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }
        public async Task<JournalSearchResult> GetIntegralModelListAsync(long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                //long id = dbc.GetAll<PlatformUserEntity>().SingleOrDefault(j => j.Mobile == "PlatformUser201805051709360001").Id;
                long jtid = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "积分增加").Id;
                long outId= dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "平台扣除").Id;
                var journals = dbc.GetAll<JournalEntity>().Where(j => j.PlatformUserId != j.ToPlatformUserId && j.IntegralType.Name=="平台积分");
                if (typeId != null)
                {
                    journals = journals.Where(j => j.JournalTypeId == typeId);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Mobile.Contains(mobile) && j.JournalTypeId != jtid);
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Code.Contains(code) && j.JournalTypeId != jtid);
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                long givingIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "商家积分").Id;
                long useIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "消费积分").Id;
                result.TotalCount = await journals.LongCountAsync();
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == "PlatformUser201805051709360001");
                if (user != null)
                {
                    result.PlatformIntegral = user.PlatformIntegral;
                    result.GivingIntegrals = await journals.Where(j => j.ToIntegralTypeId == givingIntegralId && j.JournalTypeId!=outId).SumAsync(j => j.OutIntegral);
                    result.UseIntegrals = await journals.Where(j => j.ToIntegralTypeId == useIntegralId && j.JournalTypeId != outId).SumAsync(j => j.OutIntegral);
                }
                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }
        public async Task<JournalSearchResult> GetGivingModelListAsync(long? id, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                long journalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "赠送").Id;
                var journals = dbc.GetAll<JournalEntity>();
                if (id != null)
                {
                    journals = journals.Where(j => j.PlatformUserId == id);
                }
                journals = journals.Where(j => j.JournalTypeId == journalTypeId && j.InIntegral == null);
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Mobile.Contains(mobile));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Code.Contains(code));
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = await journals.LongCountAsync();
                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }
        public async Task<JournalSearchResult> GetSpendModelListAsync(long? id, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                long journalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "消费").Id;
                var journals = dbc.GetAll<JournalEntity>();
                if (id != null)
                {
                    journals = journals.Where(j => j.PlatformUserId == j.ToPlatformUserId && j.PlatformUserId == id);
                }
                journals = journals.Where(j => j.JournalTypeId == journalTypeId && j.OutIntegral == null);
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.FormPlatformUser.Mobile.Contains(mobile));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.FormPlatformUser.Code.Contains(code));
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = await journals.LongCountAsync();
                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }

        public async Task<JournalSearchResult> GetModelListAsync(string typeName, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var journals = dbc.GetAll<JournalEntity>();
                long useId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "消费").Id;
                long givingId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "赠送").Id;
                if (typeName == "交易")
                {
                    journals = journals.Where(j => j.PlatformUserId != j.ToPlatformUserId).Where(j => j.JournalTypeId == useId || j.JournalTypeId == givingId);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Mobile.Contains(mobile) || j.PlatformUser.Mobile.Contains(mobile));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Code.Contains(code) || j.PlatformUser.Code.Contains(code));
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = await journals.LongCountAsync();

                result.GivingIntegralCount = await journals.Where(j => j.JournalTypeId == givingId).SumAsync(j => j.OutIntegral);
                result.UseIntegralCount = await journals.Where(j => j.JournalTypeId == useId).SumAsync(j => j.OutIntegral);

                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }

        public async Task<JournalSearchResult> GetUserModelListAsync(long id, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var journals = dbc.GetAll<JournalEntity>();
                long useId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "消费").Id;
                journals = journals.Where(j => (j.PlatformUserId == id && j.PlatformUserId == j.ToPlatformUserId) || (j.PlatformUserId == id && j.JournalTypeId == useId) || (j.ToPlatformUserId == id && j.ToPlatformUserId == j.FormPlatformUserId));
                result.TotalCount = await journals.LongCountAsync();
                var res = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals= res.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }

        public async Task<JournalSearchResult> GetMerchantModelListAsync(long? id, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                long useId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == "消费").Id;
                var journals = dbc.GetAll<JournalEntity>();
                journals = journals.Where(j => j.PlatformUserId == id || (j.PlatformUserId == id && j.PlatformUserId == j.ToPlatformUserId) || (j.ToPlatformUserId == id && j.ToPlatformUserId == j.FormPlatformUserId));
                if (typeId != null)
                {
                    journals = journals.Where(j => j.JournalTypeId == typeId);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Mobile.Contains(mobile) || (j.FormPlatformUser.Mobile.Contains(mobile) && j.JournalTypeId == useId));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.ToPlatformUser.Code.Contains(code) || (j.FormPlatformUser.Code.Contains(code) && j.JournalTypeId == useId));
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = await journals.LongCountAsync();
                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }
        public async Task<JournalSearchResult> GetAgencyModelListAsync(long? id,long? typeId, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var journals = dbc.GetAll<JournalEntity>().Where(j => j.PlatformUserId != j.ToPlatformUserId && j.IntegralType.Name == "平台积分");

                if (id!=null)
                {
                    journals = journals.Where(j => j.PlatformUserId == id);
                    long useId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(j => j.Name == "消费积分").Id;
                    long givingId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(j => j.Name == "商家积分").Id;
                    result.GivingIntegralCount = await journals.Where(j => j.ToIntegralTypeId == givingId).SumAsync(j => j.OutIntegral);
                    result.UseIntegralCount = await journals.Where(j => j.ToIntegralTypeId == useId).SumAsync(j => j.OutIntegral);
                }
                if (typeId != null)
                {
                    journals = journals.Where(j => j.JournalTypeId == typeId);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.PlatformUser.Mobile.Contains(mobile));
                }
                if (startTime != null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = await journals.LongCountAsync();
                var journalResult = await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }
    }
}
