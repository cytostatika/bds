using System;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IWindowJoin : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
        Task Init(string in1, string in2, string out1);
    }
}
