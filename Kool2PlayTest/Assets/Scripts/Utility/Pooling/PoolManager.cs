using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;


namespace Kool2Play.Utility.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private Variables.FloatReference _updateTime;

        private List<PoolPoint> _delayedPool;
        private List<Pooler> _poolers;

        public List<PoolPoint> DelayedPool
        {
            get
            {
                return this._delayedPool ??
                       (this._delayedPool = new List<PoolPoint>());
            }
        }


        public void Awake()
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


        public GameObject Pool(PoolPoint point)
        {
            GameObject pooled =
                _poolers.SingleOrDefault((pooler) => pooler.GameObjectId == point.IdOfObject)?.Pool();

            if (pooled is null) return null;

            Transform objTransform = pooled.transform;
            objTransform.parent = point.Parent;
            objTransform.position = point.Position;

            return pooled;
        }
    }
}
