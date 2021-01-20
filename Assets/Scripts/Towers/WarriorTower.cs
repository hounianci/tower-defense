using UnityEngine;

public class WarriorTower : Tower 
{
	public override TowerType TowerType => TowerType.Warrior;

	protected override int blockNum(){
		return 5;
	}

}
