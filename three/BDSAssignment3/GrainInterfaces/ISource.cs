using System;
using System.Threading.Tasks;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface ISource : Orleans.IGrainWithGuidCompoundKey
    {
        Task Init();
    }
}
