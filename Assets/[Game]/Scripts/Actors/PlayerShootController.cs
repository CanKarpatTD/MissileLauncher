using System.Collections;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Managers;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Actors
{
    public class PlayerShootController : Actor<PlayerManager>
    {
        [HideInInspector]public Transform firePos;
        [HideInInspector]public GameObject fireBall;

        [HideInInspector]public float fireTime;
        
        [HideInInspector]public SkinnedMeshRenderer blendShape;
        
        [HideInInspector]public bool shotFired;
        [HideInInspector]public bool canShoot,fill;
        [HideInInspector]private float scaleX;
        
        [HideInInspector]public GameObject vortex;
        [HideInInspector]public GameObject explosion,explosion2;
        [HideInInspector]public ParticleSystem shootEffect;

        [HideInInspector]public GameObject cross;
        [HideInInspector]public Transform leftEngine, rightEngine;

        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.Fire,StartFire);
                
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Play,GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Continue,GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Retry,GameStart);
                
                GameManager.Instance.Subscribe(ManagerEvents.FinishLevel,GameStop);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.Fire,StartFire);
                
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Play,GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Continue,GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Retry,GameStart);
                
                GameManager.Instance.Unsubscribe(ManagerEvents.FinishLevel,GameStop);
            }
        }

        private void GameStop(object[] arguments)
        {
            Manager.gameStart = false;
        }

        private void GameStart(object[] arguments)
        {
            Manager.playerObj.SetActive(true);
            StartCoroutine(Starter());
        }

        IEnumerator Starter()
        {
            yield return new WaitForSeconds(.5f);
            Manager.gameStart = true;
        }

        protected override void MB_Start()
        {
            scaleX = 100;
            blendShape.SetBlendShapeWeight(0,scaleX);

            leftEngine.DOLocalRotate(new Vector3(0, -360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            rightEngine.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }

        protected override void MB_Update()
        {
            if (Manager.gameStart)
            {
                if (shotFired)
                {
                    fireTime += 1 * Time.deltaTime;

                    DOTween.To(() => scaleX, x => scaleX = x, 0, Manager.reloadSpeed).OnUpdate(() =>
                    {
                        blendShape.SetBlendShapeWeight(0, scaleX);
                    });

                    if (fireTime >= Manager.reloadSpeed)
                    {
                        canShoot = true;
                        shotFired = false;
                        fireTime = 0;
                    }
                }

                if (fill)
                {
                    DOTween.To(() => scaleX, x => scaleX = x, 100, .1f).OnUpdate(() =>
                    {
                        blendShape.SetBlendShapeWeight(0, scaleX);
                    }).OnComplete(() => { fill = false; });
                }
            }
        }

        private void StartFire(object[] arguments)
        {
            var target = (Vector3) arguments[0];

            if (canShoot)
            {
                gameObject.transform.DOMove(new Vector3(0, -4.1f, 0), .1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    gameObject.transform.position = new Vector3(0, -4, 0);
                });
            }

            if (!Manager.multipleShoot && !Manager.explosionChain)
                SetFire(target);

            if (Manager.multipleShoot)
            {
                MultipleShot(target);
            }

            if (Manager.explosionChain)
            {
                ExplosionChain(target);
            }
        }
        
        private void SetFire(Vector3 target)
        {
            if (Manager.gameStart)
            {
                if (canShoot)
                {
                    VibrationManager.Instance.TriggerLightImpact();

                    shootEffect.Play();

                    Camera.main.transform.DOShakePosition(0.5f, 0.05f).OnComplete(() =>
                    {
                        Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                    });

                    var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

                    bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                    fill = true;
                    canShoot = false;
                    shotFired = true;

                    var vort = Instantiate(vortex, target, Quaternion.Euler(0, -90, 90));
                    var a = Instantiate(cross, target, Quaternion.identity);
                    bullet.transform.DOMove(target, Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                        Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                        Destroy(a);
                        vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                        {
                            vort.GetComponent<BoxCollider>().enabled = true;
                        });
                        Destroy(bullet);

                    }).OnStart(() => { vort.transform.position = target; });
                }
            }
        }

        private void MultipleShot(Vector3 target)
        {
            if (Manager.gameStart)
            {
                if (canShoot)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            VibrationManager.Instance.TriggerLightImpact();

                            shootEffect.Play();

                            Camera.main.transform.DOShakePosition(0.5f, 0.05f).OnComplete(() =>
                            {
                                Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                            });

                            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

                            bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                            fill = true;
                            canShoot = false;
                            shotFired = true;
                            var a = Instantiate(cross, target, Quaternion.identity);
                            var vort = Instantiate(vortex, target, Quaternion.Euler(0, -90, 90));

                            bullet.transform.DOMove(target, Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Destroy(a);
                                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                                {
                                    vort.GetComponent<BoxCollider>().enabled = true;
                                });
                                Destroy(bullet);

                            }).OnStart(() => { vort.transform.position = target; });

                            
                        }

                        if (i == 1)
                        {
                            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

                            bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                            fill = true;
                            canShoot = false;
                            shotFired = true;

                            var vort = Instantiate(vortex, new Vector3(target.x+1,target.y,target.z), Quaternion.Euler(0, -90, 90));

                            bullet.transform.DOMove(new Vector3(target.x+1,target.y,target.z), Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));

                                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                                {
                                    vort.GetComponent<BoxCollider>().enabled = true;
                                });
                                Destroy(bullet);

                            }).OnStart(() => { vort.transform.position = new Vector3(target.x + 1,target.y,target.z); });
                        }
                        
                        if (i == 2)
                        {
                            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

                            bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                            fill = true;
                            canShoot = false;
                            shotFired = true;

                            var vort = Instantiate(vortex, new Vector3(target.x-1,target.y,target.z), Quaternion.Euler(0, -90, 90));

                            bullet.transform.DOMove(new Vector3(target.x -1,target.y,target.z), Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));

                                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                                {
                                    vort.GetComponent<BoxCollider>().enabled = true;
                                });
                                Destroy(bullet);

                            }).OnStart(() => { vort.transform.position = new Vector3(target.x - 1,target.y,target.z); });
                        }
                    }
                }
            }
        }

        private void ExplosionChain(Vector3 target)
        {
            if (Manager.gameStart)
            {
                if (canShoot)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            VibrationManager.Instance.TriggerLightImpact();

                            shootEffect.Play();

                            Camera.main.transform.DOShakePosition(0.5f, 0.05f).OnComplete(() =>
                            {
                                Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                            });

                            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);
                            var a = Instantiate(cross, target, Quaternion.identity);
                            bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                            fill = true;
                            canShoot = false;
                            shotFired = true;

                            var vort = Instantiate(vortex, target, Quaternion.Euler(0, -90, 90));

                            bullet.transform.DOMove(target, Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Destroy(a);
                                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                                {
                                    vort.GetComponent<BoxCollider>().enabled = true;
                                });
                                Destroy(bullet);

                            }).OnStart(() => { vort.transform.position = target; });

                            
                        }

                        if (i == 1)
                        {
                            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

                            bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                            fill = true;
                            canShoot = false;
                            shotFired = true;

                            var vort = Instantiate(vortex, new Vector3(target.x,target.y + 1,target.z), Quaternion.Euler(0, -90, 90));

                            bullet.transform.DOMove(new Vector3(target.x,target.y + 1,target.z), Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));

                                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                                {
                                    vort.GetComponent<BoxCollider>().enabled = true;
                                });
                                Destroy(bullet);

                            }).OnStart(() => { vort.transform.position = new Vector3(target.x,target.y + 1,target.z); });
                        }
                        
                        if (i == 2)
                        {
                            var bullet = Instantiate(fireBall, firePos.position, Quaternion.identity);

                            bullet.transform.DOScale(0.03f, .3f).SetEase(Ease.OutBack);

                            fill = true;
                            canShoot = false;
                            shotFired = true;

                            var vort = Instantiate(vortex, new Vector3(target.x,target.y - 1,target.z), Quaternion.Euler(0, -90, 90));

                            bullet.transform.DOMove(new Vector3(target.x,target.y - 1,target.z), Manager.bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Instantiate(explosion, bullet.transform.position, Quaternion.Euler(0, -90, 90));
                                Instantiate(explosion2, bullet.transform.position, Quaternion.Euler(0, -90, 90));

                                vort.transform.DOScale(new Vector3(Manager.vortexScale, Manager.vortexScale, Manager.vortexScale), .1f).OnComplete(() =>
                                {
                                    vort.GetComponent<BoxCollider>().enabled = true;
                                });
                                Destroy(bullet);

                            }).OnStart(() => { vort.transform.position = new Vector3(target.x,target.y - 1,target.z); });
                        }
                    }
                }
            }
        }
    }
}