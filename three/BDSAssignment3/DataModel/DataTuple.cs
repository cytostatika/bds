using System.Collections.Generic;

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