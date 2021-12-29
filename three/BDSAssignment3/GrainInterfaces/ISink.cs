using System.Threading.Tasks;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface ISink : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
    }
}

