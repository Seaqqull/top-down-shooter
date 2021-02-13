using System.Collections;
using UnityEngine;


namespace Kool2Play.Utility
{
    public interface IRunLater
    {
        void RunLater(System.Action method, float waitSeconds);

        Coroutine RunLaterValued(System.Action method, float waitSeconds);
        IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds);
    }

}
