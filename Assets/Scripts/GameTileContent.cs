using UnityEngine;
using System.Collections.Generic;

[SelectionBase]
public class GameTileContent : MonoBehaviour {

	[SerializeField]
	GameTileContentType type = default;
	int x, y;
	public int X{
		get=>x;
		set{x=value;}
	}
	public int Y{
		get=>y;
		set{y=value;}
	}
	GameTileContentFactory originFactory;
	Dictionary<int, List<TargetAble>> onboardTargets = new Dictionary<int, List<TargetAble>>();
	

	public Dictionary<int, List<TargetAble>> OnboardTargets{
		get => onboardTargets;
		set {onboardTargets = value;}
	}

	List<Enemy> enemies = new List<Enemy>();

	public List<Enemy> Enemies{
		get => enemies;
		set{enemies = value;}
	}

	public bool BlocksPath =>
		Type == GameTileContentType.Wall;

	public GameTileContentType Type => type;

	public GameTileContentFactory OriginFactory {
		get => originFactory;
		set {
			Debug.Assert(originFactory == null, "Redefined origin factory!");
			originFactory = value;
		}
	}

	void Awake(){
	}


	public void Recycle () {
		originFactory.Reclaim(this);
	}

}