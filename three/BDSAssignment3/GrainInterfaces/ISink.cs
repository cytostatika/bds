using System.Threading.Tasks;
using Orleans;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface ISink : IGrainWithIntegerKey
    {
        Task Process(object e);
        Task Init();
    }
}