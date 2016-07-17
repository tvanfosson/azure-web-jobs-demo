using System.Threading.Tasks;

namespace WebJobDemo.Core.Data
{
    public interface IUpdateStatisticCommand
    {
        Task AddConfirmation(string domain);
    }
}