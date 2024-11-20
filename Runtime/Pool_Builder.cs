using UnityEngine;

namespace AceLand.Pool
{
    public sealed partial class Pool<T>
    {
        public static IPoolSettingsBuilder Builder() =>
            new PoolBuilder();
        
        public interface IPoolSettingsBuilder
        {
            IPoolBuilder WithSettings(PoolSettings<T> settings);
        }

        public interface IPoolBuilder
        {
            Pool<T> Build();
            IPoolBuilder WithPoolParent(Transform poolParent);
            IPoolBuilder WithTargetParent(Transform targetParent);
        }

        private class PoolBuilder : IPoolSettingsBuilder, IPoolBuilder
        {
            private PoolSettings<T> _settings;
            private Transform _poolParent;
            private Transform _targetParent;

            public Pool<T> Build() =>
                new(_settings, _poolParent, _targetParent);
            
            public IPoolBuilder WithSettings(PoolSettings<T> settings)
            {
                _settings = settings;
                return this;
            }

            public IPoolBuilder WithPoolParent(Transform poolParent)
            {
                _poolParent = poolParent;
                return this;
            }

            public IPoolBuilder WithTargetParent(Transform targetParent)
            {
                _targetParent = targetParent;
                return this;
            }
        }
    }
}