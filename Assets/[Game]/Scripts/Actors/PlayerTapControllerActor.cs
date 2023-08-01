using DG.Tweening;
using Game.GlobalVariables;
using Game.Managers;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

namespace Game.Actors
{
    public class PlayerTapControllerActor : Actor<PlayerManager>
    {
        [HideInInspector]public Vector3 worldPosition;
        
        private Vector3 newDirection;
        [HideInInspector]public LayerMask layer;

        public bool stop;
        protected override void MB_Update()
        {
            if (Manager.gameStart)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitData;
                
                if (Physics.Raycast(ray, out hitData, 1000, layer))
                {
                    worldPosition = hitData.point;
                    
                    if (Input.GetMouseButtonUp(0))
                    {
                        Push(CustomManagerEvents.Fire, worldPosition);
                    }
                }
                var pointingTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);
                
                Manager.movingHand.transform.position = Vector3.Lerp(Manager.movingHand.transform.position, pointingTarget, 0.5f);

                if (!stop)
                    Manager.playerObj.transform.LookAt(pointingTarget, Vector3.back);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                stop = true;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                stop = false;
            }
        }
    }
}