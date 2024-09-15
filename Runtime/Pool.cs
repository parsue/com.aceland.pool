using AceLand.Library.Disposable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AceLand.Pool
{
    public class Pool<T> : DisposableObject 
        where T : MonoBehaviour, IPoolItem
    {
        ~Pool() => Dispose(false);
        
        private readonly PoolSettings<T> _settings;
        private readonly IObjectPool<T> _itemPool;
        private readonly List<T> _outItems = new();

        public IEnumerable<T> OutItems => _outItems;
        public string AssetName => _settings.OwnedItemPrefab.name;
        public int CountActive => _outItems.Count;
        public int CountInactive => _itemPool.CountInactive;

        private readonly System.Random _random = new();

        public Pool()
        {
            _outItems.Clear();
        }

        public Pool(PoolSettings<T> settings)
        {
            _settings = settings;
            _outItems.Clear();
            _itemPool = settings.PoolType switch
            {
                PoolType.Stack => new ObjectPool<T>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, settings.CollectionChecks, settings.PrewarmSize, settings.MaxSize),
                PoolType.LinkedList => new LinkedPool<T>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, settings.CollectionChecks, settings.MaxSize),
                _ => throw new ArgumentOutOfRangeException(nameof(PoolType), settings.PoolType, string.Empty),
            };
        }

        public Pool(PoolSettings<T> settings, Transform poolParent, Transform targetParent)
        {
            _settings = settings;
            _settings.SetPoolParent(poolParent);
            _settings.SetTargetParent(targetParent);
            _outItems.Clear();
            _itemPool = settings.PoolType switch
            {
                PoolType.Stack => new ObjectPool<T>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, settings.CollectionChecks, settings.PrewarmSize, settings.MaxSize),
                PoolType.LinkedList => new LinkedPool<T>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, settings.CollectionChecks, settings.MaxSize),
                _ => throw new ArgumentOutOfRangeException(nameof(PoolType), settings.PoolType, string.Empty),
            };
        }

        protected override void DisposeManagedResources()
        {
            ReleaseAll();
            Clear();
        }

        public void Clear()
        {
            _itemPool.Clear();
        }

        public T Get() => _itemPool.Get();
        public void Release(T item)
        {
            _itemPool.Release(item);
        }

        public void ReleaseRandom()
        {
            var index = _random.Next(_outItems.Count);
            _itemPool.Release(_outItems[index]);
        }
        public void ReleaseAll()
        {
            var items = _outItems.ToArray();
            foreach (var item in items)
                _itemPool.Release(item);
        }

        private T OnCreatePooledItem()
        {
            T item = UnityEngine.Object.Instantiate(_settings.OwnedItemPrefab, _settings.PoolParent).GetComponent<T>();
            if (!_outItems.Contains(item)) _outItems.Add(item);
            return item;
        }
        private void OnReturnedToPool(T pooledItem)
        {
            pooledItem.gameObject.SetActive(false);
            pooledItem.transform.SetParent(_settings.PoolParent);
            pooledItem.OnReturnToPool();
            if (_outItems.Contains(pooledItem)) _outItems.Remove(pooledItem);
        }

        private void OnTakeFromPool(T pooledItem)
        {
            pooledItem.transform.SetParent(_settings.TargetParent);
            pooledItem.gameObject.SetActive(true);
            pooledItem.OnTakeFromPool();
            if (!_outItems.Contains(pooledItem)) _outItems.Add(pooledItem);
        }

        private void OnDestroyPoolObject(T pooledItem)
        {
            if (pooledItem == null || pooledItem.gameObject == null) return;
            UnityEngine.Object.Destroy(pooledItem.gameObject);
        }
    }
}