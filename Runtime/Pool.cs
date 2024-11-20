using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AceLand.Pool
{
    public sealed partial class Pool<T>
        where T : MonoBehaviour, IPoolItem
    {
        private readonly PoolSettings<T> _settings;
        private readonly IObjectPool<T> _itemPool;
        private readonly List<T> _pooledItems = new();

        public IEnumerable<T> PooledItems => _pooledItems;
        public string ItemName => _settings.OwnedItemPrefab.name;
        public int CountPooled => _pooledItems.Count;
        public int CountInactive => _itemPool.CountInactive;

        private readonly System.Random _random = new();

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
            var index = _random.Next(_pooledItems.Count);
            _itemPool.Release(_pooledItems[index]);
        }
        public void ReleaseAll()
        {
            var items = _pooledItems.ToArray();
            foreach (var item in items)
                _itemPool.Release(item);
        }

        private T OnCreatePooledItem()
        {
            var item = Object.Instantiate(_settings.OwnedItemPrefab, _settings.PoolParent).GetComponent<T>();
            if (!_pooledItems.Contains(item)) _pooledItems.Add(item);
            return item;
        }
        private void OnReturnedToPool(T pooledItem)
        {
            pooledItem.OnReturnToPool();
            pooledItem.transform.SetParent(_settings.PoolParent);
            pooledItem.gameObject.SetActive(false);
            if (_pooledItems.Contains(pooledItem)) _pooledItems.Remove(pooledItem);
        }

        private void OnTakeFromPool(T pooledItem)
        {
            pooledItem.OnTakeFromPool();
            pooledItem.gameObject.SetActive(true);
            pooledItem.transform.SetParent(_settings.TargetParent);
            if (!_pooledItems.Contains(pooledItem)) _pooledItems.Add(pooledItem);
        }

        private void OnDestroyPoolObject(T pooledItem)
        {
            if (pooledItem == null || pooledItem.gameObject == null) return;
            Object.Destroy(pooledItem.gameObject);
        }
    }
}