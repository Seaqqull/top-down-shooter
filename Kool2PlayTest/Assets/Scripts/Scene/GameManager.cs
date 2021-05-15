using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using UnityEngine;
using System;


namespace Kool2Play.Scene
{
    
    class GameManager : MonoBehaviour, Utility.IRunLater
    {
        private static GameManager _instance;

        [Header("General")]
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Behaviour.Player.Player _player;
        [SerializeField] [Utility.OwnAttribute.ReadOnly] private bool _gameStarted;
        [Header("UI")]
        [SerializeField] private Utility.Variables.IntegerVariable _killedAmount;
        [SerializeField] private Utility.Variables.IntegerVariable _overalAmount;
        [Header("Events")]
        [SerializeField] private UnityEvent _onSpawn;
        [SerializeField] private UnityEvent _onKill;
        [SerializeField] private UnityEvent _onEnd;

        private bool _poolInitialized;
        private bool _initialized;

        public static GameManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<GameManager>();
                if (_instance != null) return _instance;


                GameObject instance = new GameObject(nameof(GameManager), typeof(GameManager));
                instance.transform.SetAsFirstSibling();

                _instance = instance.GetComponent<GameManager>();
                _instance.Initialize();

                return _instance;
            }
        }
        public bool GameStarted
        {
            get 
            {
                return this._gameStarted;
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
            if (_initialized) return;
            
            if (_player == null)
                _player = FindObjectOfType<Behaviour.Player.Player>();

            if (_player != null)
                _player.SetActiveWeapon(0);
        }

        private IEnumerator LoadAsynchronously(int sceneIndex)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
            loadOperation.allowSceneActivation = false;

            while (!loadOperation.isDone)
            {
                if (loadOperation.progress >= 0.9f)
                {
                    loadOperation.allowSceneActivation = true;
                }

                yield return null;
            }
        }


        public void EndGame()
        {
            _gameStarted = false;


            // Notification about game end
            _onEnd.Invoke();

            // Other stuff
            // Disable pooling
            Utility.Pooling.PoolManager.Instance.ClearDelayedPool();

            // Reseting data
            _overalAmount.SetValue(0);
            _killedAmount.SetValue(0);
                             
            // False notification
            _onKill.Invoke();
            _onSpawn.Invoke();

            // Reseting the player
            RunLater(() =>
            {
                _player.Revive();
                _player.Transform.position = _startPosition.position;
            }, Utility.Constants.Action.RESET_DELAY);            
        }

        public void BeginGame()
        {
            _gameStarted = true;
            

            _overalAmount.SetValue(0);
            _killedAmount.SetValue(0);

            var delayedPool = FindObjectsOfType<Utility.Pooling.PoolPoint>();

            foreach (var poolPoint in delayedPool)
            {                
                Utility.Pooling.PoolManager.Instance.AddToDelayedPool(poolPoint);
                if(_poolInitialized) continue;

                // Update of pool data
                poolPoint.OnLatePool += () => {
                    GameObject newEnemy = Utility.Pooling.PoolManager.Instance.Pool(poolPoint);
                    if (newEnemy == null) return;

                    _overalAmount.ApplyChange(1);
                    _onSpawn.Invoke();

                    // Update of kill data
                    if (newEnemy.TryGetComponent<Behaviour.Enemies.Enemy>(out var pooledEnemy))
                    {
                        pooledEnemy.SetTarget(_player);
                        pooledEnemy.OnDead += (enemy) =>
                        {
                            _killedAmount.ApplyChange(1);
                            _onKill.Invoke();
                        };
                    }
                };
            }
            _poolInitialized = true;
        }        

        public void ChangeWeapon(bool flag)
        {
            if (_player == null) return;

            _player.SetActiveWeapon((flag) ? 0 : 1);
        }


        #region RunLater
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
        #endregion
    }
}
