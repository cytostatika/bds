using System;
namespace GrainStreamProcessing.DataModel
{
    public class Timestamp
    {
        long timestamp;
        public Timestamp(long timestamp)
        {
            this.timestamp = timestamp;
        }
        public long getTimestamp()
        {
            return timestamp;
        }
        public void setTimestamp(long timestamp)
        {
            this.timestamp = timestamp;
        }
    }
}
