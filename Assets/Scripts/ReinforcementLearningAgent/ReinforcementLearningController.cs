using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to convey the received messages to actions in the simulation
public class ReinforcementLearningController : MonoBehaviour
{

    public static ReinforcementLearningController Instance;

    private bool stop_simulation = false;
    private bool start_simulation = false;

    void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    // The unity simulation can only be stopped from the main thread
    private void Update()
    {
        if (stop_simulation)
        {
            Time.timeScale = 0;
            stop_simulation = false;
        }
        else if (start_simulation)
        {
            Time.timeScale = 1;
            start_simulation = false;
        }
    }

    //Stop the simulation
    public void StopSimulation()
    {
        Debug.Log("Stop Simulation");
        //Time.timeScale = 0;
        stop_simulation = true;
    }

    //Resume the simulation
    public void ResumeSimulation()
    {
        Debug.Log("Resume Simulation");
        //Time.timeScale = 1;
        start_simulation = true;
    }

    private void HandleMessage(CommunicationMessage message)
    {
        //Handle the received message according to its type
        if (message is ControlMessage)
        {
            ControlMessage controlMessage = (ControlMessage)message;
            if (controlMessage.SendMessage == "STOP")
            {
                StopSimulation();
            }
            else
            {
                ResumeSimulation();
            }
        }
        else if (message is InfoMessage)
        {
            InfoMessage infoMessage = (InfoMessage)message;
            Debug.Log("Received Info: " + infoMessage.Info);
        }
    }

    public void ParseMessage(string text)
    {
        CommunicationMessage message = InfoMessage.Parse(text);
        if (message == null)
        {
            message = ControlMessage.Parse(text);
            if (message == null)
            {
                Debug.Log("Could not translate into message! Received: " + text);
                return;
            }
        }

        HandleMessage(message);
    }

}
