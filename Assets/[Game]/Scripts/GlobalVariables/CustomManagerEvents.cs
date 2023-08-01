using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalVariables
{
    /// <summary>
    /// Add custom managers events here.
    /// <example> <code> public const string SomeEvent = nameof(SomeEvent); </code> </example>
    /// </summary>
    public static partial class CustomManagerEvents
    {
        public const string Fire = nameof(Fire);
        public const string GivePlayerDamage = nameof(GivePlayerDamage);
        public const string GainXp = nameof(GainXp);
        public const string GetUpgradeScreen = nameof(GetUpgradeScreen);
        
        public const string BulletSpeedUpgrade = nameof(BulletSpeedUpgrade);
        public const string ReloadSpeedUpgrade = nameof(ReloadSpeedUpgrade);
        public const string ExplosionPowerUpgrade = nameof(ExplosionPowerUpgrade);
        public const string MultipleShotsUpgrade = nameof(MultipleShotsUpgrade);
        public const string ExplosionChainUpgrade = nameof(ExplosionChainUpgrade);
        public const string AutoTurretUpgrade = nameof(AutoTurretUpgrade);
        public const string LevelDone = nameof(LevelDone);
    }
}
