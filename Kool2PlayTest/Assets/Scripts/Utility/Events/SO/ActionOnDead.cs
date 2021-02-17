using UnityEngine;


namespace Kool2Play.Utility.Events
{
    [CreateAssetMenu(menuName = "Action/OnDead")]
    class ActionOnDead : EventSO
    {
        protected override void OnRaise()
        {
            var enemies = FindObjectsOfType<Behaviour.Enemies.Enemy>();

            foreach (var enemy in enemies)
            { 
                enemy.Health.ModifyHealth(enemy.Health.Value);
            }
        }
    }
}
