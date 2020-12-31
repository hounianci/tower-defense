﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerFactory : GameObjectFactory 
{
	[SerializeField]
    Tower[] towerPrefabs = default;
	public Tower Get (TowerType type, GameTile tile) {
		Debug.Assert((int)type < towerPrefabs.Length, "Unsupported tower type!");
		Tower prefab = towerPrefabs[(int)type];
		Debug.Assert(type == prefab.TowerType, "Tower prefab at wrong index!");
		Tower tower = Get(prefab);
		int towerId = 0;
		switch(type){
			case TowerType.Laser:
			towerId=1;
			break;
			case TowerType.Mortar:
			towerId=2;
			break;
			case TowerType.Warrior:
			towerId=3;
			break;
		}
		tower.Init(tile, Game.Instance.Board, towerId);
		return tower;
	}
	T Get<T> (T prefab) where T : Tower {
		T instance = CreateGameObjectInstance(prefab);
		instance.OriginFactory = this;
		return instance;
	}
}
