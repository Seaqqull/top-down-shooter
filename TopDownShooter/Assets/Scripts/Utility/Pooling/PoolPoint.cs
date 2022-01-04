using UnityEngine;


namespace TopDownShooter.Utility.Pooling
{
    public class PoolPoint : Points.ScenePoint
    {
#pragma warning disable 0649
        [SerializeField] protected Color _color = Color.gray;
#pragma warning restore 0649

        [Header("Properties")]
        [SerializeField] protected Variables.IntegerReference _idOfObject;
        [SerializeField] protected Variables.StringReference _additionalTag;
        [SerializeField] protected Transform _parent;
        [SerializeField] protected Events.ActionBoolSO _emitCheckerSO;
        [SerializeField] protected Events.ActionBoolBehaviour _emitCheckerBehaviour;

        private System.Action _onLatePool = delegate { };

        public event System.Action OnLatePool
        {
            add { this._onLatePool += value; }
            remove { this._onLatePool -= value; }
        }
        public bool SingleTimeExecutable
        {
            get
            {
                bool flag = true;

                if (_emitCheckerSO != null)
                    flag &= _emitCheckerSO.SingleExecution;

                if (_emitCheckerBehaviour != null)
                    flag &= _emitCheckerBehaviour.SingleExecution;

                return flag;
            }
        }
        public string AdditionalTag
        {
            get { return _additionalTag.Value; }
        }
        public override Color Color
        {
            get { return this._color; }
        }
        public bool IsDelayedPool
        {
            get
            {
                return ((_emitCheckerSO != null) ||
                        (_emitCheckerBehaviour != null));
            }
        }
        public Transform Parent
        {
            get { return this._parent; }
        }
        public bool PoolDelayed
        {
            get
            {
                bool poolFlag = false;

                if (_emitCheckerSO != null)
                    poolFlag |= _emitCheckerSO.Perform();

                if (_emitCheckerBehaviour != null)
                    poolFlag |= _emitCheckerBehaviour.Perform();

                return poolFlag;
            }
        }
        public int IdOfObject
        {
            get { return _idOfObject.Value; }
        }


        protected override void Awake()
        {
            base.Awake();

            if (_parent == null)
                _parent = Transform;
        }


        public void Pool()
        {
            _onLatePool.Invoke();
        }
    }
}
