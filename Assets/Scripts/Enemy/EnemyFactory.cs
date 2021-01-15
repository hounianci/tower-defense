using UnityEngine;
using Slider;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory {
	public Enemy Get (int enemyId) {
		EnemyConfigEntry enemyConfigEntry = DataManager.GetData<EnemyConfigEntry>(typeof(EnemyConfig), enemyId);
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/Enemies/"+enemyConfigEntry.Model);
		Enemy enemyPrefab = gameObject.GetComponent<Enemy>();
		Enemy instance = CreateGameObjectInstance(enemyPrefab);
		instance.OriginFactory = this;
		instance.Initialize(
			enemyConfigEntry.Scale,
			enemyConfigEntry.Speed,
			0,
			enemyConfigEntry.Hp
		);
		instance.OriginFactory = this;
		return instance;
	}

	public void Reclaim (Enemy enemy) {
		Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
		Destroy(enemy.gameObject);
	}
}