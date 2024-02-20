namespace Football.Core
{
    public interface IService
    {
        void Initialize();
        void Destroy();
    }

    public interface IService<T> : IService
    {
        T Configuration { get; set; }
    }
}