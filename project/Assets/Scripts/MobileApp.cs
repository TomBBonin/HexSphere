using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Trigger
{
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(StandaloneInputModule))]
    [RequireComponent(typeof(Animator))]
    public class MobileApp : MonoBehaviour
    {
        private Animator _animator;

        protected void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected IEnumerator Start()
        {
            // wait a frame
            yield return null;

            // loading complete
            DoLoaded();
        }

        protected void Update() { }

        private void DoLoaded() { _animator.SetTrigger("DoLoaded"); }
    }
}