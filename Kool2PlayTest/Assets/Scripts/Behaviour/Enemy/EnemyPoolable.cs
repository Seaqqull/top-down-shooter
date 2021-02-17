using UnityEngine;


namespace Kool2Play.Behaviour.Enemies
{
    public abstract class EnemyPoolable : Enemy, Utility.Pooling.Data.IPoolable
    {
        private Utility.Pooling.Pooler _pooler;
        private LayerMask _poolLayer;
        private bool _isInitialized;
        private int _poolHealth;

        public bool IsInitializationBuilt
        {
            get { return this._isInitialized; }
        }
        public Utility.Pooling.Pooler Pooler
        {
            get { return this._pooler; }
            set { this._pooler = value; }
        }


        protected override void OnPostDeath()
        {
            PoolIn();
        }


        protected abstract void OnInitialize();


        public void PoolIn()
        {
            SetTarget(null);
            gameObject.SetActive(false);
            Pooler.AddToPool(gameObject);
            StopAllCoroutines();
        }

        public void PoolOut()
        {
            _isDead = false;

            _gfxRenderer.material = _baseMaterial;
            _baseMaterial.SetColor(Utility.Constants.Material.BASE_COLOR, _baseColor);
        }

        public void PoolInitialize()
        {
            _health.ResetHealth(_poolHealth);
            gameObject.layer = _poolLayer.value;
            _isDead = false;

            _body.isKinematic = false;
            _body.detectCollisions = true;

            _collider.enabled = true;

            OnInitialize();
        }

        public void BuildInitialization()
        {
            _poolHealth = _health.Value;
            _poolLayer = gameObject.layer;
            _isInitialized = true;
        }
    }
}
