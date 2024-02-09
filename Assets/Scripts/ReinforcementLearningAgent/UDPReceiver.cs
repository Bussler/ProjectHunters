using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class UDPReceiver : MonoBehaviour
{
    public string host = "127.0.0.1";
    public int port = 1337;

    private UdpClient udpClient;
    private Thread thread;

    void Start()
    {
        udpClient = new UdpClient(port);
        thread = new Thread(new ThreadStart(ReceiveData));
        thread.Start();
    }

    // Receive data from the UDP connection
    void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(host), port);
                byte[] data = udpClient.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                InfoMessage imessenge = InfoMessage.Parse(text);
                if (imessenge != null)
                {
                    Debug.Log("Received Info: " + imessenge.Info);
                }
                else
                {
                    ControlMessage cmessenge = ControlMessage.Parse(text);
                    if (cmessenge != null)
                    {
                        Debug.Log("Received Control: " + cmessenge.SendMessage);
                    }
                    else
                    {
                        Debug.Log("Received: " + text);
                    }
                }
                
            }
            catch (SocketException e)
            {
                Debug.Log(e.ToString());
                break;
            }
        }
    }

    // Close the UDP connection and stop the thread when the application is closed
    void OnApplicationQuit()
    {
        thread.Abort();
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}

