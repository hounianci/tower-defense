using UnityEngine;

[CreateAssetMenu]
public class GameScenario : ScriptableObject {
	EnemyWave[] waves = {};

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
			Debug.Assert(scenario.waves.Length > 0, "Empty scenario!");
			wave = scenario.waves[0].Begin();
		}

		public bool Progress () {
			float deltaTime = wave.Progress(timeScale * Time.deltaTime);
			while (deltaTime >= 0f) {
				if (++index >= scenario.waves.Length) {
					return false;
				}
				wave = scenario.waves[index].Begin();
				deltaTime = wave.Progress(deltaTime);
			}
			return true;
		}
	}
}