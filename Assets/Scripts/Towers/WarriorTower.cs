using UnityEngine;

public class WarriorTower : Tower 
{
	public override TowerType TowerType => TowerType.Warrior;

	protected override int blockNum(){
		return 5;
	}


    public override void GameUpdate()
    {
		if(blockingEnemy.Count != 0){
			this.transform.LookAt(blockingEnemy[0].transform.position);
		}		
    }

}
