namespace AceLand.Pool
{
    public interface IPoolItem
    {
        public void OnTakeFromPool();
        public void OnReturnToPool();
    }
}
