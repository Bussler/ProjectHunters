using JetBrains.Annotations;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public enum StopType
{
	STOP,
	RESUME
}

[Serializable]
public class CommunicationMessage
{
    public string ClassType;
}

[Serializable]
public class ControlMessage: CommunicationMessage
{
	public string SendMessage;

	public ControlMessage(string message)
	{
		ClassType = "ControlMessage";
		SendMessage = message;
	}

	public static ControlMessage Parse(string data)
	{
        var jsonData = JsonUtility.FromJson<ControlMessage>(data);
		if (jsonData.ClassType != "ControlMessage")
		{
			return null;
		}
		return new ControlMessage(jsonData.SendMessage);
	}
}

[Serializable]
public class InfoMessage: CommunicationMessage
{
    public string Info;

	public InfoMessage(string info)
	{
		ClassType = "InfoMessage";
		Info = info;
	}

    public string AsJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static InfoMessage Parse(string data)
	{
        var jsonData = JsonUtility.FromJson<InfoMessage>(data);
		if (jsonData.ClassType != "InfoMessage")
		{
			return null;
		}
		return new InfoMessage(jsonData.Info);
	}
}
