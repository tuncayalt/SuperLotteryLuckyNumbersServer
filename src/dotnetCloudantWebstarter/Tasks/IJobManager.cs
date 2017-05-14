namespace CloudantDotNet.Tasks
{
    public interface IJobManager
    {
        void AddJob(IJob job);
        void RemoveJob(IJob job);
    }
}
