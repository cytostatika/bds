using System.Threading.Tasks;
using GrainStreamProcessing.Model;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IWindowJoin : IGrainWithIntegerKey
    {
        Task Process();
        Task Init(string in1, string in2, string out1, long wdSize);
        public Task OnNextMessage1((string, DataTuple, long) message, StreamSequenceToken sequenceToken);
        public Task OnNextMessage2((string, DataTuple, long) message, StreamSequenceToken sequenceToken);
    }
}