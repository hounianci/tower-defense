using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameActor : MonoBehaviour
{
    protected int Hp{get;set;}
    public GameBoard Board{get;set;}
    public GameTile Tile{get;set;}
	public int Id{get;set;}

    public Dictionary<ActorStateType, List<ActorState>> States = new Dictionary<ActorStateType, List<ActorState>>();
    public ActorState CurrentState{get;set;}

    public void Init(GameTile tile, GameBoard board, int actorId, GameObjectFactory factory, object[] payloads){
        this.Tile = tile;
        this.Board = board;
		this.OriginFactory = factory;
        transform.SetParent(tile.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        Id = actorId;
		Init0(payloads);
        InitState();
        Hp = 10;
        board.AddActor(this);
		transform.parent = board.transform;
    }

    protected abstract void InitState();

    List<GameActor> blockActors;
    public List<GameActor> BlockActors{
        get=>blockActors;
        set{blockActors=value;}
    }
	GameObjectFactory originFactory;
	public GameObjectFactory OriginFactory {
		get => originFactory;
		set {
			originFactory = value;
		}
	}

    public abstract void ExecuteState(ActorState state, int deltaTime);
    public void TurnNextState(){
        CurrentState.Exit(this);
        ActorState nextState = null;
        foreach(ActorState state in States[CurrentState.GetActorType()]){
            if(state.CanEnter(this)){
                nextState = state;
                break;
            }
        }
        if(nextState!=null){
            CurrentState = nextState;
            CurrentState.Enter(this);
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
	protected virtual void Init0 (object[] payloads) { }
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