using System;

namespace FalconNet.FalcLib
{
	public enum FalconDamageType {
      BulletDamage,
      MissileDamage,
      CollisionDamage,
      BombDamage,
      FODDamage,
      GroundCollisionDamage,
      ObjectCollisionDamage,
      FeatureCollisionDamage,
      DebrisDamage,
      ProximityDamage,
	  OtherDamage,		// KCK: Use if you don't want any messages/scoring to occur
   }
}

