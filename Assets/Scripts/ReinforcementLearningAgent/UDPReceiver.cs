using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


// Class to receive UDP messages from host:port and convey them to the Controller
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

                ReinforcementLearningController.Instance.ParseMessage(text);
                
            }
            catch (SocketException e)
            {
                Debug.Log(e.ToString());
                break;
            }
        }
    }

    private void OnDisable()
    {
        thread.Abort();
        if (udpClient != null)
        {
            udpClient.Close();
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

