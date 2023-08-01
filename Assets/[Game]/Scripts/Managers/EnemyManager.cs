using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Actors;
using Game.GlobalVariables;
using Game.Helpers;
using TMPro;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

namespace Game.Managers
{
    public class EnemyManager : Manager<EnemyManager>
    {
        [HideInInspector] public List<Transform> normalSpawnPoints;
        private bool gameStart;
        private float spawnTimer;
        public float spawnBorder;

        [HideInInspector] public int missileCounter;
        [HideInInspector] public int spawnedMissile;

        [HideInInspector] public GameObject flashExplosion;
        [HideInInspector] public GameObject hitExpo;

        [HideInInspector] public TextMeshProUGUI rocketText;
        [HideInInspector] public int spawnedCounter;
        public int rocketHolder;

        public List<GameObject> spawns;
        public GameObject thickMissile, fatMissile, spaceship, meteor1, meteor2, meteor3, new1, new2, new3;

        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Play, GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Continue, GameStart);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Retry, GameStart);
            }
            else
            {
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Play, GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Continue, GameStart);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Retry, GameStart);
            }
        }

        private void GameStart(object[] arguments)
        {
            spawns.Clear();
            
            spawnedMissile = 0;

            missileCounter = ((GameLevel) LevelManager.Instance.levelData).missileCount;

            spawnedCounter = missileCounter;
            rocketHolder = missileCounter;

            rocketText.text = spawnedCounter.ToString();
            rocketText.transform.DOScale(1.2f, 1f);

            spawnBorder = ((GameLevel) LevelManager.Instance.levelData).spawnRate;

            if (((GameLevel) LevelManager.Instance.levelData).forThickNormal)
            {
                spawns.Add(thickMissile);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forMeteor1)
            {
                spawns.Add(meteor1);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forMeteor2)
            {
                spawns.Add(meteor2);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forMeteor3)
            {
                spawns.Add(meteor3);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forSpaceship)
            {
                spawns.Add(spaceship);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forFatNormal)
            {
                spawns.Add(fatMissile);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forGrey1)
            {
                spawns.Add(new1);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forGrey2)
            {
                spawns.Add(new2);
            }

            if (((GameLevel) LevelManager.Instance.levelData).forGrey3)
            {
                spawns.Add(new3);
            }
        }


        protected override void MB_Update()
        {
            if (PlayerManager.Instance.gameStart)
            {
                if (rocketHolder != 0)
                {
                    spawnTimer += 1 * Time.deltaTime;

                    if (spawnTimer >= spawnBorder)
                    {
                        // spawnedMissile++;
                        rocketHolder--;

                        rocketText.transform.DOScale(1.2f, .11f).OnComplete(() =>
                        {
                            rocketText.transform.DOScale(1f, .11f);
                        });

                        spawnTimer = 0;

                        var type = Random.Range(0, spawns.Count);

                        var rndm = Random.Range(0, 1);

                        if (rndm == 0)
                        {
                            var get = Random.Range(0, normalSpawnPoints.Count);

                            Instantiate(spawns[type], normalSpawnPoints[get].position, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}