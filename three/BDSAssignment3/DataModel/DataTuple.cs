using System.Collections.Generic;
using System.Linq;

namespace GrainStreamProcessing.Functions
{
    public abstract class DataTuple
    {
        public int UserId { get; set; }
        public int? PhotoId { get; set; }
        public float? Lat { get; set; }
        public float? Long { get; set; }

        public override string ToString()
        {
            return $"user:{UserId}, photo:{PhotoId}, lat:{Lat}, long:{Long}";
        }
    }

    public class PhotoTuple : DataTuple
    {
        public PhotoTuple(IReadOnlyList<string> numbers)
        {
            PhotoId = int.Parse(numbers[0]);
            UserId = int.Parse(numbers[1]);
            Lat = float.Parse(numbers[2]);
            Long = float.Parse(numbers[3]);
        }
    }

    public class GpsTuple : DataTuple
    {
        public GpsTuple(IReadOnlyList<string> numbers)
        {
            UserId = int.Parse(numbers[0]);
            Lat = float.Parse(numbers[1]);
            Long = float.Parse(numbers[2]);
        }
    }

    public class TagTuple : DataTuple
    {
        public TagTuple(IReadOnlyList<string> numbers)
        {
            PhotoId = int.Parse(numbers[0]);
            UserId = int.Parse(numbers[1]);
        }
    }
    public class MergeTuple : DataTuple // Just a single type of mergetuple for showcase - Rest are trivial
    {

        public new IList<int> PhotoId { get; set; }
        public new IList<int> UserId { get; set; }
        public new IList<float> Lat { get; set; }
        public new IList<float> Long { get; set; }

        
        public MergeTuple(DataTuple tag, DataTuple gps)
        {
            var a = new List<DataTuple> {tag, gps};

            PhotoId = new List<int>();
            UserId = new List<int>();
            Lat = new List<float>();
            Long = new List<float>();

            foreach (var x in a)
            {
                if (x.PhotoId != null && !PhotoId.Contains((int) x.PhotoId)) PhotoId.Add((int) x.PhotoId);
                if (!UserId.Contains( x.UserId)) UserId.Add( x.UserId);
                if (x.Lat != null && !Lat.Contains((float) x.Lat)) Lat.Add((float) x.Lat);
                if (x.Long != null && !Long.Contains((float) x.Long)) Long.Add((float) x.Long);
            }
        }
    }

    public class AggregateTuple<T> : DataTuple
    {
        public T AggregateValue { get; set; }

        public override string ToString()
        {
            return $"aggregate: {AggregateValue}";
        }

        // public dynamic AggregateValue { get; set; }
        // private readonly Type _type;
        // public AggregateTuple(Type type)
        // {  
        //     this._type = type;
        //     AggregateValue = Activator.CreateInstance(type);
        // }
        //
        // public dynamic Function()
        // {
        //     return Activator.CreateInstance(_type);
        // }
    }
}