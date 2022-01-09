using System.Threading.Tasks;
using Orleans;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IFilter : IGrainWithIntegerKey
    {
        Task Process(object e);
        Task Init();
    }
}