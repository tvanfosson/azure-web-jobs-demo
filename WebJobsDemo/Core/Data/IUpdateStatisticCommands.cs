using System.Threading.Tasks;

namespace WebJobDemo.Core.Data
{
    public interface IUpdateStatisticCommands
    {
        Task AddConfirmation(string domain);
        Task Recalculate();
    }
}