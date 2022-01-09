using System.Threading.Tasks;
using Orleans;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IAggregate : IGrainWithIntegerKey
    {
        Task Process(object e);
        Task Init();
    }
}