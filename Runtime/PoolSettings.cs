using System;
using UnityEngine;

namespace AceLand.Pool
{
    [Serializable]
    public struct PoolSettings<T> where T : MonoBehaviour, IPoolItem
    {
        [Header("Pool")]
        [SerializeField] private T ownedItemPrefab;
        [SerializeField] private Transform poolParent;
        [SerializeField] private Transform targetParent;
        [SerializeField] private PoolType poolType;
        [SerializeField] private int prewarmSize;
        [SerializeField] private int maxSize;
        [SerializeField] private bool collectionChecks;

        public readonly T OwnedItemPrefab => ownedItemPrefab;
        public readonly Transform PoolParent => poolParent;
        public readonly Transform TargetParent => targetParent;
        public readonly PoolType PoolType => poolType;
        public readonly int PrewarmSize => prewarmSize;
        public readonly int MaxSize => maxSize;
        public readonly bool CollectionChecks => collectionChecks;

        public void SetPoolParent(Transform parent) => poolParent = parent;
        public void SetTargetParent(Transform parent) => targetParent = parent;
    }
}
