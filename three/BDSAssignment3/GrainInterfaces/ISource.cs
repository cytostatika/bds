using System.Threading.Tasks;
using Orleans;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface ISource : IGrainWithGuidCompoundKey
    {
        Task Init();
    }
}