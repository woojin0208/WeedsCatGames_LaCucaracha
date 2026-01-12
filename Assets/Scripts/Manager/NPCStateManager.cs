using System.Collections.Generic;
using UnityEngine;

public class NPCStateManager : MonoBehaviour
{
    private readonly Dictionary<NPCId, NPCState> npcStates = new();

    private static NPCStateManager instance;
    public static NPCStateManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public NPCState GetState(NPCId id)
        => npcStates.TryGetValue(id, out var s) ? s : NPCState.FirstMeet;

    public void SetState(NPCId id, NPCState state)
        => npcStates[id] = state;
}

public enum NPCId
{
    FrogBoy,
    MouseMan,
    DesertMan,
    TurtleBoy,
    WhiteGuard,
    PinkGirl,
    Kimbob,
    TutorialSpeaker,
}