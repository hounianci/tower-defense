using UnityEngine;
using System.Collections.Generic;

public class Enemy : GameBehavior {

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

	GameBoard board;
	public GameBoard Board{
		get=>board;
		set{board=value;}
	}

	private Tower blockingTower;

	public void InitPath(){
		string pathPointsStr = FileUtil.readFile(string.Format("Assets/Path/{0}Path.txt", id));
		string[] pathPoints = pathPointsStr.Split('|');
		List<int[]> mainPoints = new List<int[]>();
		foreach(string pathPointStr in pathPoints){
			string[] points = pathPointStr.Split(',');
			int x = int.Parse(points[1]);
			int y = int.Parse(points[0]);
			mainPoints.Add(new int[]{y, x});
		}
		path = board.FindEnemyPath(new int[]{tileFrom.Y, tileFrom.X}, mainPoints);
	}

	public Tower BlockingTower{
		get => blockingTower;
		set{
			blockingTower = value;
		}
	}
	public int Id{
		get => id;
	}
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

	public EnemyFactory OriginFactory {
		get => originFactory;
		set {
			Debug.Assert(originFactory == null, "Redefined origin factory!");
			originFactory = value;
		}
	}

	public bool IsValidTarget => animator.CurrentClip == EnemyAnimator.Clip.Move;

	public float Scale { get; private set; }

	float Health { get; set; }

	public void ApplyDamage (float damage) {
		Debug.Assert(damage >= 0f, "Negative damage applied.");
		Health -= damage;
	}

	public override bool GameUpdate () {
#if UNITY_EDITOR
		if (!animator.IsValid) {
			animator.RestoreAfterHotReload(
				model.GetChild(0).GetComponent<Animator>(),
				animationConfig,
				animationConfig.MoveAnimationSpeed * speed / Scale
			);
		}
#endif
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

		if (Health <= 0f) {
			animator.PlayDying();
			targetPointCollider.enabled = false;
			return true;
		}
		progress += Time.deltaTime * progressFactor;
		if(progress >= 1){
			if (tileTo == null) {
				Game.EnemyReachedDestination();
				animator.PlayOutro();
				targetPointCollider.enabled = false;
				return true;
			}
			if(blockingTower==null){
				progress = (progress - 1f) / progressFactor;
				PrepareNextState();
				progress *= progressFactor;
			}else{
				//被阻挡
				progress = Mathf.Min(1, progress);
			}
		}
		if(blockingTower!=null){
			if(blockingTower==tileFrom.Content){
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

	public override void Recycle () {
		animator.Stop();
		OriginFactory.Reclaim(this);
	}

	public void Initialize (
		float scale, float speed, float pathOffset, float health
	) {
		Scale = scale;
		model.localScale = new Vector3(scale, scale, scale);
		this.speed = speed;
		this.pathOffset = pathOffset;
		Health = health;
		animator.PlayIntro();
		targetPointCollider.enabled = false;
	}

	public void SpawnOn (GameTile tile) {
		tileFrom = tile;
		tileTo = tile.NextTileOnPath;
		progress = 0f;
		PrepareIntro();
	}

	void Awake () {
		animator.Configure(
			model.GetChild(0).gameObject.AddComponent<Animator>(),
			animationConfig
		);
	}

	void PrepareNextState () {
		tileFrom.Content.Enemies.Remove(this);
		tileFrom = tileTo;
		tileFrom.Content.Enemies.Add(this);
		tileTo = tileTo.NextTileOnPath;
		positionFrom = positionTo;
		if (tileTo == null) {
			PrepareOutro();
			return;
		}
		positionTo = tileFrom.ExitPoint;
		directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
		direction = tileFrom.PathDirection;
		directionAngleFrom = directionAngleTo;
		switch (directionChange) {
			case DirectionChange.None: PrepareForward(); break;
			case DirectionChange.TurnRight: PrepareTurnRight(); break;
			case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
			default: PrepareTurnAround(); break;
		}
		if(tileTo.Content.Type == GameTileContentType.Tower){
			Tower tower = (Tower)tileTo.Content;
			tower.enemyPass(this);
		}
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

	void PrepareIntro () {
		positionFrom = tileFrom.transform.localPosition;
		transform.localPosition = positionFrom;
		positionTo = tileFrom.ExitPoint;
		direction = tileFrom.PathDirection;
		directionChange = DirectionChange.None;
		directionAngleFrom = directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localRotation = direction.GetRotation();
		progressFactor = 2f * speed;
	}

	void PrepareOutro () {
		positionTo = tileFrom.transform.localPosition;
		directionChange = DirectionChange.None;
		directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localRotation = direction.GetRotation();
		progressFactor = 2f * speed;
	}

	void OnDestroy () {
		animator.Destroy();
	}
}