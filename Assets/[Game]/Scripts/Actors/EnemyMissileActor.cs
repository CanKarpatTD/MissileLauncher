using System;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Managers;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Game.Actors
{
    public class EnemyMissileActor : Actor<EnemyManager>
    {
        public bool fatMissile, thickMissile,spaceship;
        
        public Transform missileChild;

        [Space(10)]public float speed;
        
        [Space(10)]public float damage;
        public bool meteor;
        public GameObject meteorChild;

        private bool canMove;
        
        private bool lookVortex;


        [HideInInspector]public TrailRenderer trail;

        [HideInInspector]public GameObject missileLittle;
        [HideInInspector]public bool jump;

        [HideInInspector]public Vector3 f;
        
        public Transform child;

        [HideInInspector][Space(20)] public Transform pos1;
        [HideInInspector]public Transform pos2;
        private float spawnTimer;
        
        [Space(10)]public float spawnBorder;
        
        [HideInInspector]public bool littleMissile;
        public GameObject littleExplosion;
        public bool splitMissile;

        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Play,Kill);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Continue,Kill);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Retry,Kill);
                
                PlayerManager.Instance.Subscribe(CustomManagerEvents.LevelDone,Kill);
            }
            else
            {
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Play,Kill);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Continue,Kill);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Retry,Kill);  
                
                PlayerManager.Instance.Unsubscribe(CustomManagerEvents.LevelDone,Kill);
            }
        }

        private void Kill(object[] arguments)
        {
            Destroy(gameObject);
        }

        protected override void MB_Start()
        {
            if(jump)
                Jumper();
            
            if (meteorChild != null)
                meteorChild.transform.DOLocalRotate(new Vector3(0, -360, 0), 1f, RotateMode.FastBeyond360)
                    .SetLoops(-1).SetEase(Ease.Linear);
        }

        private void Jumper()
        {
            gameObject.transform.DOJump(f, 0.5f,1,1).OnComplete(() =>
            {
                jump = false;
            }).SetEase(Ease.Linear);
        }

        protected override void MB_Update()
        {
            if (PlayerManager.Instance.gameStart)
            {
                if (!canMove)
                {
                    if (!spaceship)
                    {
                        if (missileChild != null)
                            missileChild.DOLookAt(PlayerManager.Instance.playerObj.transform.position, .1f).SetId(2);
                        

                        if (!jump)
                            transform.Translate(missileChild.forward * speed * Time.deltaTime);
                    }
                    else
                    {
                        if (missileChild != null)
                            missileChild.DOLookAt(PlayerManager.Instance.playerObj.transform.position, .1f).SetId(2);
                        if (!jump)
                            transform.Translate(missileChild.forward * speed * Time.deltaTime);

                        if (gameObject.transform.position.x > 0)
                        {
                            if (child != null)
                                child.localEulerAngles = new Vector3(child.localEulerAngles.x, child.localEulerAngles.y,
                                    -90);
                        }
                        if (gameObject.transform.position.x < 0)
                        {
                            if (child != null)
                                child.localEulerAngles =
                                    new Vector3(child.localEulerAngles.x, child.localEulerAngles.y, 90);
                        }
                        Spawner();
                    }
                }

                if (splitMissile)
                {
                    if (gameObject.transform.position.x > 0)
                    {
                        if (child != null)
                            child.localEulerAngles = new Vector3(child.localEulerAngles.x, -90,child.localEulerAngles.z);
                    }
                    if (gameObject.transform.position.x < 0)
                    {
                        if (child != null)
                            child.localEulerAngles = new Vector3(child.localEulerAngles.x, 90, child.localEulerAngles.z);
                    }
                }
            }
        }

        private void Spawner()
        {
            spawnTimer += 1 * Time.deltaTime;

            if (spawnTimer >= spawnBorder)
            {
                // spawnedMissile++;
                        
                spawnTimer = 0;


                var rndm = Random.Range(0, 2);

                if (rndm == 0)
                {
                   var a = Instantiate(Manager.thickMissile, pos1.position, Quaternion.identity);
                   Instantiate(littleExplosion, pos1.position, Quaternion.identity);
                   a.GetComponent<EnemyMissileActor>().littleMissile = true;
                }
                else if (rndm == 1)
                {
                    var a = Instantiate(Manager.thickMissile, pos2.position, Quaternion.identity);
                    Instantiate(littleExplosion, pos2.position, Quaternion.identity);
                    a.GetComponent<EnemyMissileActor>().littleMissile = true;
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Vortex"))
            {
                if (!fatMissile)
                {
                    if (trail != null)
                        trail.enabled = false;

                    gameObject.tag = "Untagged";

                    if (!littleMissile)
                        Manager.spawnedMissile++;
                    
                    Destroy(other.gameObject.GetComponent<BoxCollider>());

                    missileChild.parent = other.gameObject.GetComponent<NovaVortexControllerActor>().follower;
                    missileChild.localPosition = new Vector3(0, 0, 0);
                    missileChild.DOLocalRotate(new Vector3(0, 0, 0), 0.1f);

                    gameObject.transform.DOLocalMove(other.transform.position, .5f);

                    // flame.SetActive(false);

                    canMove = true;
                    lookVortex = true;

                    VibrationManager.Instance.TriggerSoftImpact();

                    Camera.main.transform.DOShakePosition(0.1f, 0.2f).OnComplete(() =>
                    {
                        Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                    });

                    // transform.DORotate(new Vector3(transform.rotation.x,transform.rotation.y,-360), 1f,RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
                    Instantiate(Manager.flashExplosion, gameObject.transform.position, Quaternion.identity);

                    Push(CustomManagerEvents.GainXp);

                    Push(CustomManagerEvents.GivePlayerDamage, gameObject);
                    
                    if (!littleMissile)
                    {
                        Manager.spawnedCounter--;
                        Manager.rocketText.text = Manager.spawnedCounter.ToString();
                    }
                    
                    missileChild.DOScale(0, 1.5f).OnComplete(() => { Destroy(missileChild.gameObject); });


                    transform.DOScale(0, 1.5f).OnComplete(() => { Destroy(gameObject); });

                    
                }
                else
                {
                    Instantiate(Manager.flashExplosion, gameObject.transform.position, Quaternion.identity);
                    Instantiate(Manager.hitExpo, gameObject.transform.position, Quaternion.identity);
                    
                    Destroy(other.gameObject);

                    var a = Instantiate(missileLittle, gameObject.transform.position, quaternion.Euler(0,0,0));
                    a.GetComponent<EnemyMissileActor>().jump = true;
                    a.GetComponent<EnemyMissileActor>().littleMissile = true;
                    a.GetComponent<EnemyMissileActor>().f = new Vector3(-1,1.5f,0);
                    
                    var b = Instantiate(missileLittle, gameObject.transform.position, quaternion.Euler(0,0,0));
                    b.GetComponent<EnemyMissileActor>().jump = true;
                    b.GetComponent<EnemyMissileActor>().littleMissile = true;
                    b.GetComponent<EnemyMissileActor>().f = new Vector3(1,1.5f,0);
                    
                    if (!littleMissile)
                        Manager.spawnedMissile++;
                    
                    if (!littleMissile)
                    {
                        Manager.spawnedCounter--;
                        Manager.rocketText.text = Manager.spawnedCounter.ToString();
                    }
                    
                    Destroy(gameObject);
                }
            }
            
            if (other.gameObject.CompareTag("Player"))
            {
                if (!littleMissile)
                    Manager.spawnedMissile++;
                
                gameObject.tag = "Untagged";
                
                Push(CustomManagerEvents.GivePlayerDamage, other.gameObject,damage);
                
                Instantiate(Manager.hitExpo, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);

                if (!littleMissile)
                {
                    Manager.spawnedCounter--;
                    Manager.rocketText.text = Manager.spawnedCounter.ToString();
                }

                if (meteor)
                {
                    Camera.main.transform.DOShakePosition(0.3f, .3f).OnComplete(() =>
                    {
                        Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                    });
                }

                if (!meteor)
                {
                    Camera.main.transform.DOShakePosition(0.2f, 0.2f).OnComplete(() =>
                    {
                        Camera.main.transform.DOLocalMove(new Vector3(0, -1.4f, -9.8f), 0.02f);
                    });
                }

                if (other.gameObject.GetComponent<PlayerShootController>() == null)
                {
                    PlayerManager.Instance.hitCount++;
                    if (PlayerManager.Instance.hitCount == 3)
                    {
                        Instantiate(PlayerManager.Instance.smoke, gameObject.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
}