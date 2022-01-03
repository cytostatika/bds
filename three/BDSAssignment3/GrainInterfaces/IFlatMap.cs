using System.Threading.Tasks;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IFlatMap : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
    }
}