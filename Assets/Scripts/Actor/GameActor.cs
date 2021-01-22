using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameActor : MonoBehaviour
{
    static int idGen = 1;
    public int identity;
    public bool NeedRemoveFromUpdate{get;set;}
    protected int Hp{get;set;}
    public GameBoard Board{get;set;}
    public GameTile Tile{get;set;}
	public int Id{get;set;}

    public Dictionary<ActorStateType, List<ActorStateType>> StatusTurns = new Dictionary<ActorStateType, List<ActorStateType>>();
    public Dictionary<ActorStateType, ActorState> AllStatus = new Dictionary<ActorStateType, ActorState>();
    public ActorState CurrentState{get;set;}
	Dictionary<ActorStateType, int> StateRank;

    public void Init(GameTile tile, GameBoard board, int actorId, GameObjectFactory factory, object[] payloads){
        identity = idGen++;
        this.Tile = tile;
        this.Board = board;
		this.OriginFactory = factory;
        transform.SetParent(tile.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        Id = actorId;
		Init0(payloads);
        InitState();
        InitStateRank();
        board.AddActor(this);
		transform.parent = board.transform;

    }

    protected void InitStateRank(){
        StateRank = new Dictionary<ActorStateType, int>();
        StateRank.Add(ActorStateType.Intro, 0);
        StateRank.Add(ActorStateType.Attack, 1);
        StateRank.Add(ActorStateType.Idle, 3);
        StateRank.Add(ActorStateType.Move, 2);
        StateRank.Add(ActorStateType.Outro, 0);
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

    public void ExecuteState(ActorState state, float deltaTime){
        ActorState nextState = GetNextState();
        if(nextState!=null && StateRank[nextState.GetActorType()]<StateRank[CurrentState.GetActorType()]){
            TurnNextState();
        }else{
            ExecuteState0(state, deltaTime);
        }
    }

    protected abstract void ExecuteState0(ActorState state, float deltaTime);

    public void EnterState(ActorStateType state){
        EnterState(AllStatus[state]);
    }
    public abstract void EnterState(ActorState state);
    public void TurnNextState(){
        ActorState nextState = GetNextState();
        if(nextState!=null){
            TurnState(nextState);
        }
    }

    public ActorState GetNextState(){
        ActorState nextState = null;
        if(!StatusTurns.ContainsKey(CurrentState.GetActorType())){
            return nextState;
        }
        foreach(ActorStateType state in StatusTurns[CurrentState.GetActorType()]){
            ActorState actorState = AllStatus[state];
            if(actorState.CanEnter(this)){
                nextState = actorState;
                break;
            }
        }
        return nextState;
    }

    public void TurnState(ActorState state){
        Debug.Log(DebugName()+" turn to "+state.GetActorType()+".");
        CurrentState.Exit(this);
        CurrentState = state;
        CurrentState.Enter(this);
    }

    public void TurnState(ActorStateType stateType){
        TurnState(AllStatus[stateType]);
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
        if(NeedRemoveFromUpdate){
            return false;
        }
        CurrentState.Execute(this, Time.deltaTime);
        bool alive = Update0();
        if(!alive && CurrentState.GetActorType()!=ActorStateType.Outro){
            TurnState(ActorStateType.Outro);
        }
        return alive&&!NeedRemoveFromUpdate;
    }
    public virtual bool Update0(){
        return isAlive();
    }
    public virtual void Recycle0(){}
    public virtual int ActorTeamId(){
        return 0;
    }

    protected abstract string DebugName();
}