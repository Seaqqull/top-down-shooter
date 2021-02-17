using UnityEngine;


namespace Kool2Play.Utility.Pooling
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

                if (_emitCheckerSO is { })
                    flag &= _emitCheckerSO.SingleExecution;

                if (_emitCheckerBehaviour is { })
                    flag &= _emitCheckerSO.SingleExecution;

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
                return ((_emitCheckerSO is { }) ||
                        (_emitCheckerBehaviour is { }));
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

                if (_emitCheckerSO is { })
                    poolFlag |= _emitCheckerSO.Perform();

                if (_emitCheckerBehaviour is { })
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

            if (_parent is null)
                _parent = Transform;
        }


        public void Pool()
        {
            _onLatePool.Invoke();
        }
    }
}
