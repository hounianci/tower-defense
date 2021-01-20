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

	public EnemyAnimationConfig AnimationConfig{get=>animationConfig;set{animationConfig=value;}}

	public GameTile TileFrom{get;set;}
	public GameTile TileTo{get;set;}
	public Vector3 positionFrom, positionTo;
	public Direction direction;
	public DirectionChange DirectionChange{get;set;}
	public float directionAngleFrom, directionAngleTo;
	public float Progress{get;set;}
	public float ProgressFactor{get;set;}
	float pathOffset;
	public float Speed{get;set;}

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
		Animator.Configure(model.GetChild(0).gameObject.AddComponent<Animator>(),animationConfig);
		Debug.Log(Animator.flag);
	} 

    protected override void InitState()
    {
		CurrentState = new IntroState();
        CurrentState.Enter(this);
		StatesTurns.Add(ActorStateType.Intro, new List<ActorState>());
		StatesTurns[ActorStateType.Intro].Add(new MoveState());

		StatesTurns.Add(ActorStateType.Idle, new List<ActorState>());
		StatesTurns[ActorStateType.Idle].Add(new AttackState());
		StatesTurns[ActorStateType.Idle].Add(new MoveState());

		StatesTurns.Add(ActorStateType.Attack, new List<ActorState>());
		StatesTurns[ActorStateType.Attack].Add(new IdleState());
		StatesTurns[ActorStateType.Attack].Add(new MoveState());
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
		path = Board.FindEnemyPath(new int[]{TileFrom.Y, TileFrom.X}, mainPoints);
		TileTo = path[0];
		path.RemoveAt(0);
	}
    public override int ActorTeamId()
    {
		return TeamId();
    }
    public override void ExecuteState(ActorState state, float deltaTime)
    {
		state.ExecuteEnemy(this, deltaTime);
    }
    public override void EnterState(ActorState state)
    {
		state.EnterEnemy(this);
    }

	public Tower BlockingTower{get;set;}
	public EnemyAnimator Animator;

	public Collider TargetPointCollider{get;set;}
	private TargetPoint targetPoint;
	public TargetPoint TargetPoint{
		get=>targetPoint;
		set{targetPoint=value;}
	}

	public float Scale { get; private set; }


	public override bool Update0 () {
		Animator.GameUpdate();
		return base.Update0();
	}

	public override void Recycle0 () {
		Animator.Stop();
	}

	public void Initialize (
		float scale, float speed, float pathOffset, float health
	) {
		Scale = scale;
		model.localScale = new Vector3(scale, scale, scale);
		this.Speed = speed;
		this.pathOffset = pathOffset;
		Hp = (int)health;
		TargetPointCollider.enabled = false;
	}

	public void SpawnOn (GameTile tile) {
		TileFrom = tile;
		Progress = 0f;
	}

	void Awake () {
	}

	public bool PrepareNextState () {
		TileFrom.Content.Enemies.Remove(this);
		if(TileFrom.Content.OnboardTargets.ContainsKey(2)){
			TileFrom.Content.OnboardTargets[2].Remove(this);
		}
		TileFrom = TileTo;
		Tile = TileFrom;
		if(!TileFrom.Content.OnboardTargets.ContainsKey(2)){
			TileFrom.Content.OnboardTargets.Add(2, new List<TargetAble>());
		}
		TileFrom.Content.Enemies.Add(this);
		TileFrom.Content.OnboardTargets[2].Add(this);
		if(path.Count==0){
			PrepareOutro();
			return false;
		}
		TileTo = path[0];
		path.RemoveAt(0);
		positionFrom = positionTo;
		positionTo = TileTo.transform.localPosition;
		DirectionChange = direction.GetDirectionChangeTo(TileFrom.PathDirection);
		direction = TileFrom.PathDirection;
		directionAngleFrom = directionAngleTo;
		switch (DirectionChange) {
			case DirectionChange.None: PrepareForward(); break;
			case DirectionChange.TurnRight: PrepareTurnRight(); break;
			case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
			default: PrepareTurnAround(); break;
		}
		if(TileTo.Content.OnboardTargets.ContainsKey(1)){
			Tower tower = (Tower)TileTo.Content.OnboardTargets[1][0];
			tower.enemyPass(this);
		}
		return true;
	}

	void PrepareForward () {
		transform.localRotation = direction.GetRotation();
		directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		ProgressFactor = Speed;
	}

	void PrepareTurnRight () {
		directionAngleTo = directionAngleFrom + 90f;
		model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
		transform.localPosition = positionFrom + direction.GetHalfVector();
		ProgressFactor = Speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
	}

	void PrepareTurnLeft () {
		directionAngleTo = directionAngleFrom - 90f;
		model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
		transform.localPosition = positionFrom + direction.GetHalfVector();
		ProgressFactor = Speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
	}

	void PrepareTurnAround () {
		directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localPosition = positionFrom;
		ProgressFactor =
			Speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
	}

	public void PrepareIntro () {
		positionFrom = TileFrom.transform.localPosition;
		transform.localPosition = positionFrom;
		positionTo = TileTo.transform.localPosition;
		direction = TileFrom.PathDirection;
		DirectionChange = DirectionChange.None;
		directionAngleFrom = directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localRotation = direction.GetRotation();
		ProgressFactor = 2f * Speed;
	}

	void PrepareOutro () {
		positionTo = TileTo.transform.localPosition;
		// directionChange = DirectionChange.None;
		// directionAngleTo = direction.GetAngle();
		// model.localPosition = new Vector3(pathOffset+10, 0f);
		// transform.localRotation = direction.GetRotation();
		// progressFactor = 2f * speed;
		TileTo = null;
	}

	void OnDestroy () {
		Animator.Destroy();
	}

	public bool IsBlockByMe(GameActor actor){
		return actor==BlockingTower;
	}
}