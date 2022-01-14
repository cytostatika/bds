using System;
using System.Threading.Tasks;

using System.Collections.Generic;
using GrainStreamProcessing.Functions;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IWindowJoin : Orleans.IGrainWithIntegerKey
    {
        Task Process();
        Task Init(string in1, string in2, string out1, long wdSize);
    }
}
