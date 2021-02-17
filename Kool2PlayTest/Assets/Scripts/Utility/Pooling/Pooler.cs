using System.Collections.Generic;
using UnityEngine;


namespace Kool2Play.Utility.Pooling
{
    public class Pooler : MonoBehaviour 
    {
        [SerializeField] protected Variables.IntegerReference _idGameObject;

        [SerializeField] protected Transform _spawnPosition;
        [SerializeField] protected GameObject _spawnObject;

        [SerializeField] protected int _poolAmount = 1;
        [SerializeField] protected int _expansionAmount = 1;
        [SerializeField] protected int _reductionAmount = 1;

        protected Queue<GameObject> _objectsToPool;
        protected GameObject _spawned;
        protected GameObject _queue;

        public int GameObjectId
        {
            get { return this._idGameObject.Value; }
        }


        public virtual void Awake()
        {
            if ((_spawnObject is null) ||
                (_spawnObject.GetComponent<Data.IPoolable>() is null))
            {
#if UNITY_EDITOR
                Debug.LogError("Poolable object doesn't have IPoolable member or object is empty", gameObject);
#endif
                return;
            }
            if (_spawnPosition is null) _spawnPosition = transform;

            _spawned = new GameObject("Spawned");
            _queue = new GameObject("Queue");

            _spawned.transform.parent = transform;
            _queue.transform.parent = transform;

            _objectsToPool = new Queue<GameObject>();

            PoolExtend(_poolAmount);
        }


        protected void PoolExtend(int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                PoolCreate();
            }
        }

        protected virtual void PoolCreate()
        {
            GameObject dummyIn = Instantiate(_spawnObject);

            if (dummyIn.TryGetComponent<Data.IPoolable>(out Data.IPoolable poolableDummy))
            {
                poolableDummy.Pooler = this;
                if (!poolableDummy.IsInitializationBuilt)
                    poolableDummy.BuildInitialization();
            }

            dummyIn.transform.parent = _queue.transform;
            dummyIn.SetActive(false);

            _objectsToPool.Enqueue(dummyIn);
        }

        protected virtual GameObject PoolOut()
        {
            GameObject dummyOut = _objectsToPool.Dequeue();
            dummyOut.SetActive(true);

            return dummyOut;
        }

        protected virtual void PoolIn(GameObject dummyIn)
        {
            dummyIn.SetActive(false);
            dummyIn.transform.parent = _queue.transform;

            _objectsToPool.Enqueue(dummyIn);
        }


        public GameObject Pool()
        {
            if (_spawnObject is null) return null;

            if (_objectsToPool.Count == 0)
                PoolExtend(_expansionAmount);

            GameObject dummyOut = PoolOut();
            Data.IPoolable poolableDummyOut = dummyOut.GetComponent<Data.IPoolable>();

            poolableDummyOut.PoolInitialize();
            poolableDummyOut.PoolOut();
            
            return dummyOut;
        }

        public void AddToPool(GameObject dummyIn)
        {
            if (_objectsToPool.Count >= (_poolAmount + _reductionAmount))
            {
                for (int i = 0; i < _reductionAmount; i++)
                    Destroy(PoolOut());
            }

            PoolIn(dummyIn);
        }
    }
}
