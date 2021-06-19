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

	/*** Enemy Types ***/
	public const int ENEMY_TYPE_NORMAL = 0;
	public const int ENEMY_TYPE_RUNNER = 1;
	public const int ENEMY_TYPE_SPITTER = 3;
	public const int ENEMY_TYPE_BOSS = 2;

	/*** Stamina Costs ***/
	public const int STAMINA_COST_CRAFT_DEFAULT = 6;
	public const int STAMINA_COST_SCAVANGE = 20;
	public const int STAMINA_COST_COMPUTER = 8;
	public const int STAMINA_COST_FIRSTAID = 10;
}
