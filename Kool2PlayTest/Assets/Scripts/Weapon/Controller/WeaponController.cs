using System.Collections;
using UnityEngine;
using System;


namespace Kool2Play.Weapons
{
    public class WeaponController : MonoBehaviour, Utility.IRunLater
    {
        public class WeaponAim
        {
            public Weapon Weapon;
         
            public void SetWeaponDefaults(Data.WeaponAutoUser option)
            {
                Weapon.Transform.localPosition = option.Shift;
                Weapon.Transform.localRotation = option.Rotation;
            }
        }


#pragma warning disable CS0649
        [SerializeField] private Transform _pivot; // For rotation
        [SerializeField] private Data.WeaponAutoUser[] _usingOptions;

        [SerializeField] private int _activeWeapon;
        [SerializeField] private Weapon[] _weapons;

        private Vector3 _targetPosition = Vector3.zero;
        private bool _canUseTarget = true;
        private bool _isWeaponEquipped;
        private int _activeOption;
        private Action _onShot;
        private bool _isInMove;
        private WeaponAim _aim;
#pragma warning disable CS0649


        public event Action OnShot
        {
            add { _onShot += value; }
            remove { _onShot -= value; }
        }

        public Data.WeaponAutoUser ActiveOption
        {
            get { return this._usingOptions[_activeOption]; }
        }
        public Weapon ActiveWeapon
        {
            get { return this._weapons[_activeWeapon]; }
        }
        public bool CanUseTarget
        {
            set
            {
                this._canUseTarget = value;

                if (!(this._canUseTarget))
                    _aim.SetWeaponDefaults(ActiveOption);
            }
        }
        public bool IsEquipped
        {
            get { return this._isWeaponEquipped; }
        }
        public int WeaponCount
        {
            get { return this._weapons.Length; }
        }
        public bool InMove
        {
            set
            {
                this._isInMove = value;
                CanUseTarget = true;

                if (_isInMove && (!ActiveOption.ActionOnMove))
                {
                    RemoveTarget();
                    CanUseTarget = false;
                }
            }
        }


        private void Awake()
        {
            _aim = new WeaponAim();

            if (_pivot is null)
                _pivot = transform;
        }

        private void Start()
        {
            SetActiveWeapon(_activeWeapon);
        }        

        private void Update()
        {
            if (!(_canUseTarget && 
                _isWeaponEquipped &&
                _weapons[_activeWeapon].IsActionExecutable)) return;

            if (_weapons[_activeWeapon].IsMagazineEmpty && _usingOptions[_activeWeapon].AutoReload)
                _weapons[_activeWeapon].Reload();
            else if (_usingOptions[_activeWeapon].AutoShot)
            {
                _weapons[_activeWeapon].Shoot();

                _onShot?.Invoke();
            }
        }


        public void Shoot()
        {
            _weapons[_activeWeapon].Shoot();
        }

        public void RemoveTarget()
        {
            _targetPosition = Vector3.zero;
            _aim.SetWeaponDefaults(ActiveOption);
        }

        public void SetActiveWeapon(int index)
        {
            if ((index < 0 || index >= _weapons.Length) ||
                ((!_weapons[_activeWeapon]?.IsActionExecutable ?? true)))
                return;

            for (int i = 0; i < _weapons.Length; i++)
            {
                if (i != index && _weapons[i].GameObj.activeSelf)
                    _weapons[i].GameObj.SetActive(false);
            }
            _weapons[index].GameObj.SetActive(true);

            _activeWeapon = index;            

            //Kool2Play.Scene.LevelManager.Instance.SceneUI.
            //    AttachWeaponToUI(_weapons[_activeWeapon]);
            //_weapons[_activeWeapon].UpdateUI();

            for (int i = 0; i < _usingOptions.Length; i++)
            {
                if (_weapons[_activeWeapon].Type == _usingOptions[i].Type)
                {
                    _activeOption = i;
                    _aim.Weapon = _weapons[_activeWeapon];
                    _aim.SetWeaponDefaults(_usingOptions[i]);
                    _isWeaponEquipped = true;
                    return;
                }
            }
        }

        public void SetTarget(Vector3 position)
        {
            _targetPosition = position;
        }

        public Weapon ChangeWeapon(Weapon weapon, int index)
        {
            if ((index < 0 || index >= _weapons.Length) ||
                ((!_weapons[_activeWeapon]?.IsActionExecutable) ?? true))
                return null;

            Weapon oldWeapon = _weapons[index];
            oldWeapon.Transform.parent = null;
            oldWeapon.Transform.position = weapon.Position;
            oldWeapon.Transform.rotation = Quaternion.identity;

            _weapons[index] = weapon;
            weapon.Transform.parent = _pivot;


            if (index == _activeWeapon)
                SetActiveWeapon(index);

            return oldWeapon;
        }


        public void RunLater(Action method, float waitSeconds)
        {
            RunLaterValued(method, waitSeconds);
        }

        public Coroutine RunLaterValued(Action method, float waitSeconds)
        {
            if ((waitSeconds < 0) || (method == null))
                return null;

            return StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        public IEnumerator RunLaterCoroutine(Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }
    }
}
