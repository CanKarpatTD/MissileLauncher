using DG.Tweening;
using Game.Managers;
using TriflesGames.ManagerFramework;
using UnityEngine;

namespace Game.Actors
{
    public class NovaVortexControllerActor : Actor<PlayerManager>
    {
        private float timer;

        private bool can;

        [HideInInspector]public Transform rotator;
        [HideInInspector]public Transform follower;

        public float closeTime;
        protected override void MB_Start()
        {
            if (rotator != null)
                rotator.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        protected override void MB_Update()
        {
            if (!can)
            {
                timer += 1 * Time.deltaTime;

                if (timer >= closeTime)
                {
                    can = true;
                    // GetComponent<BoxCollider>().enabled = false;
                    transform.DOScale(0, 1).SetEase(Ease.InBack).OnComplete(() => { Destroy(gameObject); });
                }
            }
        }
    }
}