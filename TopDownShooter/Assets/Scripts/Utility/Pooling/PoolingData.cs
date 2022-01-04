


namespace TopDownShooter.Utility.Pooling.Data
{
    public interface IPoolable
    {
        bool IsInitializationBuilt { get; }
        Pooler Pooler { get; set; }


        void PoolIn();
        void PoolOut();
        void PoolInitialize();
        void BuildInitialization();
    }
}
