using System;
using AceLand.Library.Disposable;
using UnityEngine;
using UnityEngine.Pool;

namespace AceLand.Pool
{
    public sealed partial class Pool<T> : DisposableObject
    {
        private Pool(PoolSettings<T> settings, Transform poolParent, Transform targetParent)
        {
            _settings = settings;
            if (poolParent) _settings.SetPoolParent(poolParent);
            if (targetParent) _settings.SetTargetParent(targetParent);
            _pooledItems.Clear();
            _itemPool = settings.PoolType switch
            {
                PoolType.Stack => new ObjectPool<T>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, settings.CollectionChecks, settings.PrewarmSize, settings.MaxSize),
                PoolType.LinkedList => new LinkedPool<T>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, settings.CollectionChecks, settings.MaxSize),
                _ => throw new ArgumentOutOfRangeException(nameof(PoolType), settings.PoolType, string.Empty),
            };
        }
        
        ~Pool() => Dispose(false);

        protected override void DisposeManagedResources()
        {
            ReleaseAll();
            Clear();
        }
    }
}