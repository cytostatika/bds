using System.Collections.Generic;
using System.Linq;

namespace GrainStreamProcessing.Model
{
    public abstract class DataTuple
    {
        public List<int> PhotoId { get; set; } = new List<int>();
        public List<int> UserId { get; set; } = new List<int>();
        public List<float> Lat { get; set; } = new List<float>();
        public List<float> Long { get; set; } = new List<float>();

        public override string ToString()
        {
            return
                $"user:{string.Join(",", UserId)}; photo:{string.Join(",", PhotoId)}; lat:{string.Join(",", Lat)}; long:{string.Join(",", Long)}";
        }
    }

    public class PhotoTuple : DataTuple
    {
        public PhotoTuple(string[] numbers)
        {
            PhotoId.Add(int.Parse(numbers[0]));
            UserId.Add(int.Parse(numbers[1]));
            Lat.Add(float.Parse(numbers[2]));
            Long.Add(float.Parse(numbers[3]));
        }
    }

    public class GpsTuple : DataTuple
    {
        public GpsTuple(string[] numbers)
        {
            UserId.Add(int.Parse(numbers[0]));
            Lat.Add(float.Parse(numbers[1]));
            Long.Add(float.Parse(numbers[2]));
        }
    }

    public class TagTuple : DataTuple
    {
        public TagTuple(string[] numbers)
        {
            PhotoId.Add(int.Parse(numbers[0]));
            UserId.Add(int.Parse(numbers[1]));
        }
    }

    public class MergeTuple : DataTuple // Just a single type of mergetuple for showcase - Rest are trivial
    {
        public MergeTuple(DataTuple tag, DataTuple gps, string key)
        {
            UserId = tag.UserId.Concat(gps.UserId).ToList();
            PhotoId = tag.PhotoId.Concat(gps.PhotoId).ToList();
            Long = tag.Long.Concat(gps.Long).ToList();
            Lat = tag.Lat.Concat(gps.Lat).ToList();

            switch (key)
            {
                case "UserId":
                    UserId = UserId.Distinct().ToList();
                    break;
                case "PhotoId":
                    PhotoId = PhotoId.Distinct().ToList();
                    break;
                case "Lat":
                    Lat = Lat.Distinct().ToList();
                    break;
                case "Long":
                    Long = Long.Distinct().ToList();
                    break;
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
    }
}