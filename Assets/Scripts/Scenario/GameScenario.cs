using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class GameScenario : ScriptableObject {
	
	public int ScenarioId{get;set;}
	List<SpawnWaveConfigEntry> waves ;
	EnemyWave currentWave ;

	public GameScenario(int scenarioId){
		waves = DataManager.GetConfig<SpawnWaveConfigWrapper>().GetByScenarioId(scenarioId);
		ScenarioId = scenarioId;
		if(waves.Count > 0){
			currentWave = new EnemyWave(waves[0].Id);
		}
	}

	public State Begin () => new State(this);

	[System.Serializable]
	public struct State {

		GameScenario scenario;

		int index;

		float timeScale;

		EnemyWave.State wave;

		public State (GameScenario scenario) {
			this.scenario = scenario;
			index = 0;
			timeScale = 1f;
			wave = scenario.currentWave.Begin();
		}

		public bool Progress () {
			bool isFinish = wave.Progress(timeScale * Time.deltaTime);
			while (isFinish) {
				if (++index >= scenario.waves.Count) {
					return false;
				}
				wave = new EnemyWave(scenario.waves[index].Id).Begin();
				isFinish = wave.Progress(0);
			}
			return true;
		}
	}
}