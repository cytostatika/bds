using System.Threading.Tasks;
using Orleans;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IWindowJoin : IGrainWithIntegerKey
    {
        Task Process();
        Task Init(string in1, string in2, string out1, long wdSize);
    }
}