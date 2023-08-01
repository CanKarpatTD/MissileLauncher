using DG.Tweening;
using Game.Managers;
using TriflesGames.ManagerFramework;
using UnityEngine;

namespace Game.Actors
{
    public class AutoTurretActor : Actor<PlayerManager>
    {
        [HideInInspector]public Transform firePos;
        [HideInInspector]public GameObject fireBall;

        [HideInInspector]public float fireTime;
        
        [HideInInspector]public SkinnedMeshRenderer blendShape;
        [HideInInspector]public Material liquidColor;
        [HideInInspector]public Color notReadyColor,readyColor;
        
        [HideInInspector]public bool shotFired;
        [HideInInspector]public bool fill;
        private float scaleX;
        
        public float reloadSpeed;
        
        [HideInInspector]public GameObject vortex;
        [HideInInspector]public GameObject explosion,explosion2;
        [HideInInspector]public ParticleSystem shootEffect;

        [HideInInspector]public Transform nearestEnemy;

        [HideInInspector]public Transform body;
        
        protected override void MB_Update()
        {
            if (Manager.gameStart)
            {
                if (shotFired)
                {
                    if (nearestEnemy != null)
                    {
                        fireTime += 1 * Time.deltaTime;

                        DOTween.To(() => scaleX, x => scaleX = x, 100, reloadSpeed).OnUpdate(() =>
                        {
                            blendShape.SetBlendShapeWeight(0, scaleX);
                        });

                        if (fireTime >= reloadSpeed)
                        {
                            shotFired = false;
                            fireTime = 0;
                            Shoot();
                        }
                    }
                }

                if (fill)
                {
                    DOTween.To(() => scaleX, x => scaleX = x, 0, .1f).OnUpdate(() =>
                    {
                        blendShape.SetBlendShapeWeight(0, scaleX);
                    }).OnComplete(() => { fill = false; });
                }

                if (blendShape.GetBlendShapeWeight(0) > 10)
                {
                    liquidColor.DOColor(notReadyColor, .5f);
                }
                else if (blendShape.GetBlendShapeWeight(0) < 10)
                {
                    liquidColor.DOColor(readyColor, .5f);
                }
                
                FindNearestEnemy();

                if (nearestEnemy != null)
                    body.DOLookAt(nearestEnemy.position, 1f);
            }
        }

        private void FindNearestEnemy()
        {
            float minimumDistance = 120;

            nearestEnemy = null;

            foreach (var player in GameObject.FindGameObjectsWithTag("Missile"))
            {
                if (player != null)
                {
                    if (player.name != gameObject.name)
                    {
                        float distance = (transform.position - player.transform.position).sqrMagnitude;

                        if (distance < minimumDistance)
                        {
                            minimumDistance = distance;
                            nearestEnemy = player.transform;
                        }
                    }
                }
            }
        }

        private void Shoot()
        {
            shootEffect.Play();

            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

            bullet.transform.DOScale(0.03f, .1f).SetEase(Ease.OutBack);

            fill = true;
            shotFired = true;

            var vort = Instantiate(vortex, nearestEnemy.position, Quaternion.Euler(0, -90, 90));

            bullet.transform.DOMove(nearestEnemy.position, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));

                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .01f).OnComplete(() =>
                {
                    vort.GetComponent<BoxCollider>().enabled = true;
                });
                Destroy(bullet);

            }).OnStart(() => { vort.transform.position = nearestEnemy.position; });
        }
    }
}