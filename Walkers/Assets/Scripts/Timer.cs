using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    ComponentList CL;
    //FlashLight
    public float FLTimer;

    //DeathTimer
    public bool runDeathTimer;
    public float DeathTimer;

    //SkinWalker Stages
    public float StageInterval;
    public int maxStages;
    public int curStage = 0;

	void Start ()
    {
        StartVars();
	}

    void StartVars()
    {
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
    }
	
	void Update ()
    {
        FLTimer = CL.PA.curBattery;

        if (runDeathTimer)
            DeathTimer -= Time.deltaTime;

        if (FLTimer < 0)
            FLEmpty();

        if (DeathTimer < 0)
            DeathEmpty();
	}

    void FLEmpty()
    {
        //...
        if(DeathTimer > 0)
            DeathTimer = 0;
    }


    void DeathEmpty()
    {
        //...
        if (FLTimer > 0)
            CL.PA.curBattery = 0;

        int i = 1;
        while (i <= maxStages)
        {
            if (DeathTimer < (i - 1) * -StageInterval && DeathTimer > i * -StageInterval)
            {
                curStage = i;
            }
            i++;
        }
    }
}
