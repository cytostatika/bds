using System.Threading.Tasks;
using Orleans;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IFlatMap : IGrainWithIntegerKey
    {
        Task Process(object e);
        Task Init();
    }
}