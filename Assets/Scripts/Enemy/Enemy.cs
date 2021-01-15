using UnityEngine;
using System.Collections.Generic;

public class Enemy : PositiveActor, TargetAble, BlockerActor {

	public int TeamId(){
		return 2;
	}

	[SerializeField]
	EnemyAnimationConfig animationConfig = default;

	[SerializeField]
	Transform model = default;

	
	[SerializeField]
	int id;

	EnemyFactory originFactory;

	List<GameTile> path = new List<GameTile>();

	public List<GameTile> Path{
		get=>path;
		set{path=value;}
	}

	GameTile tileFrom, tileTo;
	public GameTile TileFrom{
		get=>tileFrom;
	}
	Vector3 positionFrom, positionTo;
	Direction direction;
	DirectionChange directionChange;
	float directionAngleFrom, directionAngleTo;
	float progress, progressFactor;
	float pathOffset;
	float speed;

	EnemyConfigEntry enemyConfig;

	protected override void Init0 (object[] payloads) { 
		enemyConfig = DataManager.GetData<EnemyConfigEntry>(typeof(EnemyConfig), Id);
		SpawnOn(Tile);
		base.Init0(payloads);
		InitPath((int)payloads[0]);
		SkillQueue = new Skill[1];
		Skill skill = new Skill();
		skill.Init(2, this);
		SkillQueue[0] = skill; 
		ChangeSkill();
	} 

    protected override void InitState()
    {
		States.Add(ActorStateType.Idle, new List<ActorState>());
		States[ActorStateType.Idle].Add(new AttackState());
		States[ActorStateType.Idle].Add(new MoveState());
		States.Add(ActorStateType.Attack, new List<ActorState>());
		States[ActorStateType.Attack].Add(new IdleState());
		States[ActorStateType.Attack].Add(new MoveState());
    }
	public int ApplyDamage(float damage){
        Hp -= (int)damage;
		return Hp;
	}

	protected override int GetInterval(){
		return enemyConfig.Interval;
	} 

	public void InitPath(int pathId){
		List<PathConfigEntry> paths = DataManager.GetConfig<PathConfigWrapper>().GetByPathId(pathId);
		List<int[]> mainPoints = new List<int[]>();
		foreach(PathConfigEntry path in paths){
			int x = path.X;
			int y = path.Y;
			mainPoints.Add(new int[]{y, x});
		}
		path = Board.FindEnemyPath(new int[]{tileFrom.Y, tileFrom.X}, mainPoints);
		tileTo = path[0];
		path.RemoveAt(0);
	}
    public override int ActorTeamId()
    {
		return TeamId();
    }
    public override void ExecuteState(ActorState state, int deltaTime)
    {
		state.ExecuteEnemy(this, deltaTime);
    }

	public Tower BlockingTower{get;set;}
	EnemyAnimator animator;

	Collider targetPointCollider;
	private TargetPoint targetPoint;
	public TargetPoint TargetPoint{
		get=>targetPoint;
		set{targetPoint=value;}
	}

	public Collider TargetPointCollider {
		set {
			Debug.Assert(targetPointCollider == null, "Redefined collider!");
			targetPointCollider = value;
		}
	}

	public float Scale { get; private set; }


	public override bool Update0 () {
		base.Update0();
		animator.GameUpdate();
		//出生
		if (animator.CurrentClip == EnemyAnimator.Clip.Intro) {
			if (!animator.IsDone) {
				return true;
			}
			//出生结束开始移动
			animator.PlayMove(animationConfig.MoveAnimationSpeed * speed / Scale);
			targetPointCollider.enabled = true;
		}
		//死亡
		else if (animator.CurrentClip >= EnemyAnimator.Clip.Outro) {
			//死亡结束删除
			if (animator.IsDone) {
				Recycle();
				return false;
			}
			return true;
		}

		if (Hp <= 0f) {
			animator.PlayDying();
			targetPointCollider.enabled = false;
			return true;
		}
		if (tileTo == null) {
			Game.EnemyReachedDestination();
			animator.PlayOutro();
			targetPointCollider.enabled = false;
			return true;
		}
		progress += Time.deltaTime * progressFactor;
		if(progress >= 1){
			if(BlockingTower==null){
				progress = (progress - 1f) / progressFactor;
				if(!PrepareNextState())
					return true;
				progress *= progressFactor;
			}else{
				//被阻挡
				progress = Mathf.Min(1, progress);
			}
		}
		if(BlockingTower!=null){
			if(BlockingTower==tileFrom.Content){
				return true;
			}
		}
		if (directionChange == DirectionChange.None) {
			transform.localPosition =
				Vector3.LerpUnclamped(positionFrom, positionTo, progress);
		}
		else {
			float angle = Mathf.LerpUnclamped(
				directionAngleFrom, directionAngleTo, progress
			);
			transform.localRotation = Quaternion.Euler(0f, angle, 0f);
		}
		return true;
	}

	public override void Recycle0 () {
		animator.Stop();
	}

	public void Initialize (
		float scale, float speed, float pathOffset, float health
	) {
		Scale = scale;
		model.localScale = new Vector3(scale, scale, scale);
		this.speed = speed;
		this.pathOffset = pathOffset;
		Hp = (int)health;
		animator.PlayIntro();
		targetPointCollider.enabled = false;
	}

	public void SpawnOn (GameTile tile) {
		tileFrom = tile;
		progress = 0f;
	}

	void Awake () {
		animator.Configure(
			model.GetChild(0).gameObject.AddComponent<Animator>(),
			animationConfig
		);
	}

	bool PrepareNextState () {
		tileFrom.Content.Enemies.Remove(this);
		if(tileFrom.Content.OnboardTargets.ContainsKey(2)){
			tileFrom.Content.OnboardTargets[2].Remove(this);
		}
		tileFrom = tileTo;
		Tile = tileFrom;
		if(!tileFrom.Content.OnboardTargets.ContainsKey(2)){
			tileFrom.Content.OnboardTargets.Add(2, new List<TargetAble>());
		}
		tileFrom.Content.Enemies.Add(this);
		tileFrom.Content.OnboardTargets[2].Add(this);
		if(path.Count==0){
			PrepareOutro();
			return false;
		}
		tileTo = path[0];
		path.RemoveAt(0);
		positionFrom = positionTo;
		positionTo = tileTo.transform.localPosition;
		directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
		direction = tileFrom.PathDirection;
		directionAngleFrom = directionAngleTo;
		switch (directionChange) {
			case DirectionChange.None: PrepareForward(); break;
			case DirectionChange.TurnRight: PrepareTurnRight(); break;
			case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
			default: PrepareTurnAround(); break;
		}
		if(tileTo.Content.OnboardTargets.ContainsKey(1)){
			Tower tower = (Tower)tileTo.Content.OnboardTargets[1][0];
			tower.enemyPass(this);
		}
		return true;
	}

	void PrepareForward () {
		transform.localRotation = direction.GetRotation();
		directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		progressFactor = speed;
	}

	void PrepareTurnRight () {
		directionAngleTo = directionAngleFrom + 90f;
		model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
		transform.localPosition = positionFrom + direction.GetHalfVector();
		progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
	}

	void PrepareTurnLeft () {
		directionAngleTo = directionAngleFrom - 90f;
		model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
		transform.localPosition = positionFrom + direction.GetHalfVector();
		progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
	}

	void PrepareTurnAround () {
		directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localPosition = positionFrom;
		progressFactor =
			speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
	}

	public void PrepareIntro () {
		positionFrom = tileFrom.transform.localPosition;
		transform.localPosition = positionFrom;
		positionTo = tileTo.transform.localPosition;
		direction = tileFrom.PathDirection;
		directionChange = DirectionChange.None;
		directionAngleFrom = directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localRotation = direction.GetRotation();
		progressFactor = 2f * speed;
	}

	void PrepareOutro () {
		positionTo = tileTo.transform.localPosition;
		// directionChange = DirectionChange.None;
		// directionAngleTo = direction.GetAngle();
		// model.localPosition = new Vector3(pathOffset+10, 0f);
		// transform.localRotation = direction.GetRotation();
		// progressFactor = 2f * speed;
		tileTo = null;
	}

	void OnDestroy () {
		animator.Destroy();
	}

	public bool IsBlockByMe(GameActor actor){
		return actor==BlockingTower;
	}
}