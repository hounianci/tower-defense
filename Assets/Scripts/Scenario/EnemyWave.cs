using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class EnemyWave : ScriptableObject {

	Stack<SpawnSequenceConfigEntry> spawnSeq ;

	public EnemyWave(int waveId){
		List<SpawnSequenceConfigEntry> seqs = DataManager.GetConfig<SpawnSequenceConfigWrapper>().GetByWaveId(waveId);
		seqs.Sort((a,b)=>{return (int)(b.SpawnTime-a.SpawnTime);});
		spawnSeq = new Stack<SpawnSequenceConfigEntry>(seqs);
	}

	public State Begin() => new State(this);

	[System.Serializable]
	public struct State {

		EnemyWave wave;
		private float progress;
		public State (EnemyWave wave) {
			this.wave = wave;
			progress = 0;
		}

		public bool Progress (float deltaTime) {
			progress += deltaTime;
			while(wave.spawnSeq.Count>0 && wave.spawnSeq.Peek().SpawnTime<progress){
				SpawnSequenceConfigEntry entry = wave.spawnSeq.Pop();
				Game.SpawnEnemy(entry.EnemyId, entry.PathId, entry.SpawnId);
			}
			return wave.spawnSeq.Count==0;
		}
	}
}