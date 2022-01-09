namespace GrainStreamProcessing.Functions
{
    public abstract class DataTuple
    {
        public long TimeStamp { get; set; }
        public int UserId { get; set; }
        public int? PhotoId { get; set; }
        public float? Lat { get; set; }
        public float? Long { get; set; }

        public override string ToString()
        {
            return $"user:{UserId}, photo:{PhotoId}, lat:{Lat}, long:{Long}, time:{TimeStamp}";
        }
    }

    public class PhotoTuple : DataTuple
    {
        public PhotoTuple(string message)
        {
            var numbers = message.Split();

            PhotoId = int.Parse(numbers[0]);
            UserId = int.Parse(numbers[1]);
            Lat = float.Parse(numbers[2]);
            Long = float.Parse(numbers[3]);
            TimeStamp = long.Parse(numbers[4]);
        }
    }

    public class GPSTuple : DataTuple
    {
        public GPSTuple(string message)
        {
            var numbers = message.Split();

            UserId = int.Parse(numbers[0]);
            Lat = float.Parse(numbers[1]);
            Long = float.Parse(numbers[2]);
            TimeStamp = long.Parse(numbers[3]);
        }
    }

    public class TagTuple : DataTuple
    {
        public TagTuple(string message)
        {
            var numbers = message.Split();

            PhotoId = int.Parse(numbers[0]);
            UserId = int.Parse(numbers[1]);
            TimeStamp = long.Parse(numbers[2]);
        }
    }

    public class AggregateTuple<T> : DataTuple
    {
        public T AggregateValue { get; set; }

        public override string ToString()
        {
            return $"time: {TimeStamp}, aggregate: {AggregateValue}";
        }
    }
}