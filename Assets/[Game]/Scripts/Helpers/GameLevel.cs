using TriflesGames.Helpers;
using UnityEngine;

namespace Game.Helpers
{
    [CreateAssetMenu(menuName = "ManagerFramework/Create Level", fileName = "Level")]
    public class GameLevel : Level
    {
        [Header("Player Variables")] [Space(10)]    
        public int playerHealth;                     
        public int playerNeedExp;                    
        
        [Header("Enemy Variables")]
        [Space(20)]public int missileCount;

        [Space(10)] public float spawnRate;

        [Header("Hangileri gelecek")]
        [Space(10)]public bool forSpaceship;

        [Space(5)]public bool forThickNormal, forFatNormal, forMeteor1, forMeteor2, forMeteor3, forGrey1, forGrey2, forGrey3;


    }
}
