using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActor : MonoBehaviour
{
    protected int Hp{get;set;}
    public GameBoard Board{get;set;}
    public GameTile Tile{get;set;}
	public int Id{get;set;}

    public void Init(GameTile tile, GameBoard board, int actorId){
        this.Tile = tile;
        this.Board = board;
        transform.SetParent(tile.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        Id = actorId;
		Init0();
        Hp = 10;
    }
    List<GameActor> blockActors;
    public List<GameActor> BlockActors{
        get=>blockActors;
        set{blockActors=value;}
    }
	GameObjectFactory originFactory;
	public GameObjectFactory OriginFactory {
		get => originFactory;
		set {
			Debug.Assert(originFactory == null, "Redefined origin factory!");
			originFactory = value;
		}
	}

	public void Recycle () {
        Recycle0();
        if(this is TargetAble){
            TargetAble ta = (TargetAble)this;
            if(Tile.Content.OnboardTargets.ContainsKey(ta.TeamId())){
                Tile.Content.OnboardTargets[ta.TeamId()].Remove(ta);
            }
        }
        originFactory.Reclaim(this);
    }


	public Vector3 GetPosition(){
		return transform.position;
	}

    public Vector2Int GetTilePosition(){
		return new Vector2Int(Tile.X, Tile.Y);
	}
	public bool isAlive(){
		return Hp>0;
	}
	protected virtual void Init0 () { }
    public void OnSelected(){
        OnSelected0();
    }
    protected virtual void OnSelected0(){}
    public void OnDisSelected(){
        OnDisSelected0();
    }
    protected virtual void OnDisSelected0(){}
	public bool GameUpdate(){
        return Update0();
    }
    public virtual bool Update0(){return true;}
    public virtual void Recycle0(){}
    public virtual int ActorTeamId(){
        return 0;
    }
}