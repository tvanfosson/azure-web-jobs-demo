using System.Data;
using System.Threading.Tasks;

namespace WebJobDemo.Core.Data
{
    public interface IUpdateStatisticCommands
    {
        Task AddOrUpdateDomainCount(string domain, IDbConnection connection, IDbTransaction transaction);
        Task AddConfirmation(string domain);
        Task Recalculate();
    }
}