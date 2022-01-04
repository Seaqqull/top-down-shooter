using System.Collections.Generic;
using UnityEngine;


namespace TopDownShooter.Utility
{
    public class ComputingHandler : ScriptableObject { }

    public class ComputingData<T> : ScriptableObject
        where T : struct
    {
        [SerializeField]
        public T Data;
    }

    public class ComputingData<THandler, TData>
        where THandler : ComputingHandler
    {
#pragma warning disable 0649
        [SerializeField] private THandler _handler;
        [SerializeField] private TData[] _data;

        private IReadOnlyList<TData> _dataRestricted;
#pragma warning restore 0649

        public THandler Handler
        {
            get { return this._handler; }
        }
        public IReadOnlyList<TData> Data
        {
            get
            {
                return this._dataRestricted ??
                    (this._dataRestricted = _data);
            }
        }

    }
}
