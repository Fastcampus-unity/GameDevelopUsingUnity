using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager
{
    Dictionary<int, Actor> Actors = new Dictionary<int, Actor>();

    public bool Regist(int ActorInstanceID, Actor actor)
    {
        if(ActorInstanceID == 0)
        {
            Debug.LogError("Regist Error! ActorInstanceID is not set! ActorInstanceID = " + ActorInstanceID);
            return false;
        }

        if(Actors.ContainsKey(ActorInstanceID))
        {
            if(actor.GetInstanceID() != Actors[ActorInstanceID].GetInstanceID())
            {
                Debug.LogError("Regist Error! already exist! ActorInstanceID = " + ActorInstanceID);
                return false;
            }

            return true;
        }

        Actors.Add(ActorInstanceID, actor);
        return true;
    }

    public Actor GetActor(int ActorInstanceID)
    {
        if (!Actors.ContainsKey(ActorInstanceID))
        {
            Debug.LogError("GetActor Error! no exist! ActorInstanceID = " + ActorInstanceID);
            return null;
        }

        return Actors[ActorInstanceID];
    }

}
