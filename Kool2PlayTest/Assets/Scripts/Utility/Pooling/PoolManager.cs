using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;


namespace Kool2Play.Utility.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;

        [SerializeField] private Variables.FloatReference _updateTime;

        private List<PoolPoint> _delayedPool;
        private List<Pooler> _poolers;
        private bool _initialized;

        private List<PoolPoint> DelayedPool
        {
            get
            {
                return this._delayedPool ??
                       (this._delayedPool = new List<PoolPoint>());
            }
        }
        public static PoolManager Instance
        {
            get
            {
                if (_instance is { }) return _instance;

                _instance = FindObjectOfType<PoolManager>();
                if (_instance is { }) return _instance;


                GameObject instance = new GameObject(nameof(PoolManager), typeof(PoolManager));
                instance.transform.SetAsFirstSibling();

                _instance = instance.GetComponent<PoolManager>();
                _instance.Initialize();

                return _instance;
            }
        }


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                Initialize();
                return;
            }
            Destroy(gameObject);
        }


        private void Initialize()
        {

            _poolers = FindObjectsOfType<Pooler>().ToList();
            StartCoroutine(PoolDelayed());
        }


        private IEnumerator PoolDelayed()
        {
            var updateDelay =
                new WaitForSeconds(_updateTime.Value);

            while (true)
            {
                for (int i = DelayedPool.Count - 1; i >= 0; i--)
                {
                    if (!_delayedPool[i].PoolDelayed) continue;

                    _delayedPool[i].Pool();

                    if (_delayedPool[i].SingleTimeExecutable)
                        _delayedPool.RemoveAt(i);
                }

                yield return updateDelay;
            }
        }


        public void ClearDelayedPool()
        {
            DelayedPool.Clear();
        }

        public GameObject Pool(PoolPoint point)
        {
            GameObject pooled =
                _poolers.SingleOrDefault((pooler) => pooler.GameObjectId == point.IdOfObject)?.Pool();

            if (pooled == null) return null;

            Transform objTransform = pooled.transform;
            objTransform.parent = point.Parent;
            objTransform.position = point.Position;

            return pooled;
        }

        public void AddToDelayedPool(PoolPoint point)
        {
            DelayedPool.Add(point);
        }
    }
}
