using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ChessAgent : Agent {
    public int agentID;
    Agent m_Agent;

    public override void CollectObservations (VectorSensor sensor) {
        //Turn 0:black 1:white
        sensor.AddObservation (BoardManager.Instance.isWhiteTurn);
        //Chessman Pos
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                int cell = -1;
                if ()
            }
        }

    }

    public void AgentAction () {
        BoardManager.Instance.AreaAction ();
    }

    public override void OnActionReceived (float[] vectorAction) { }

    public override void Heuristic (float[] actionsOut) {
        actionsOut[0] = 0;
        if (Input.GetKey (KeyCode.D)) {
            actionsOut[0] = 3;
        } else if (Input.GetKey (KeyCode.W)) {
            actionsOut[0] = 1;
        } else if (Input.GetKey (KeyCode.A)) {
            actionsOut[0] = 4;
        } else if (Input.GetKey (KeyCode.S)) {
            actionsOut[0] = 2;
        }
    }

    public override void OnEpisodeBegin () { }
}