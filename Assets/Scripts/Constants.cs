using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
	/*** Attack Types ***/
	public const int ATTACK_TYPE_UNTYPED = 0;
	public const int ATTACK_TYPE_BLUNT = 1;
	public const int ATTACK_TYPE_PIERCE = 2;
	public const int ATTACK_TYPE_PROJECTILE = 3;
	public const int ATTACK_TYPE_TRAP = 4;
	public const int ATTACK_TYPE_FIRE = 5;

	/*** Enemy Wound States ***/
	public const int ENEMY_WOUND_NONE = 0;
	public const int ENEMY_WOUND_LIGHT = 1;
	public const int ENEMY_WOUND_HEAVY = 2;

	/*** Player Wound States ***/
	public const int PLAYER_WOUND_NONE = 0;
	public const int PLAYER_WOUND_LIGHT = 1;
	public const int PLAYER_WOUND_MODERATE = 2;
	public const int PLAYER_WOUND_HEAVY = 3;
}
