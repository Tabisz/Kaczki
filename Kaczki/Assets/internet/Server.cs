using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine.UI;

public class ServerClient
{
    public int connectionId;
    public string playerName;
    public Vector3 position;
    public int childCount = 0;

}

public class Server : MonoBehaviour
{

    private const int MAX_CONNECTION = 100;

    private int port = 3001;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unreliableChannel;

    private bool isStarted = false;
    private byte error;

    private List<ServerClient> clients = new List<ServerClient>();

    private float lastMovementUpdate;
    public float movementUpdateRate = 0.05f;

    public void StartServer()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        string PortField = GameObject.Find("PortInput").GetComponent<InputField>().text;
        if (PortField != "")
            port = int.Parse(PortField);

        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);
        Debug.Log("Starting server on port: " + port);
        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log("Player " + connectionId + " has connected");
                OnConnection(connectionId);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                //Debug.Log("Receiving from " + connectionId + " : " + msg);

                string[] splitData = msg.Split('|');

                switch (splitData[0])
                {
                    case "NAMEIS":
                        OnNameIs(connectionId, splitData[1]);
                        break;
                    case "MYPOSITION":
                        OnMyPosition(connectionId, float.Parse(splitData[1]), float.Parse(splitData[2]));
                        break;
                    case "ADDCHILD":
                        OnAddChild(connectionId);
                        break;
                    case "DELCHILD":
                        OnDelChild(connectionId, float.Parse(splitData[1]), float.Parse(splitData[2]));
                        break;

                    default:
                        Debug.Log("Invalid message : " + msg);
                        break;
                }
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Player " + connectionId + " has disconnected");
                OnDisconnection(connectionId);
                break;
        }

        if (Time.time - lastMovementUpdate > movementUpdateRate)
        {
            lastMovementUpdate = Time.time;
            string m = "ASKPOSITION|";
            foreach (ServerClient sc in clients)
                m += sc.connectionId.ToString() + '%' + sc.position.x.ToString() + '%' + sc.position.z.ToString() + '%'+ '|';
            m = m.Trim('|');

            Send(m, unreliableChannel, clients);



        }
    }


    private void OnConnection(int cnnId)
    {
        ServerClient c = new ServerClient();
        c.connectionId = cnnId;
        c.playerName = "Temp";
        clients.Add(c);

        string msg = "ASKNAME|" + cnnId + "|";

        foreach (ServerClient sc in clients)
            msg += sc.playerName + '%' + sc.connectionId + '%' + sc.childCount + '|';

        msg = msg.Trim('|');
        Send(msg, reliableChannel, cnnId);
    }
    private void OnDisconnection(int cnnId)
    {
        clients.Remove(clients.Find(x => x.connectionId == cnnId));

        Send("DC|" + cnnId, reliableChannel, clients);
    }
    private void OnNameIs(int cnnId, string playerName)
    {
        clients.Find(x => x.connectionId == cnnId).playerName = playerName;
        Send("CNN|" + playerName + '|' + cnnId, reliableChannel, clients);

    }
    private void OnMyPosition(int cnnId, float x, float z)
    {
        clients.Find(c => c.connectionId == cnnId).position = new Vector3(x, 0.4f, z);


    }

    private void OnAddChild(int cnnId)
    {
        Send("ADDCHILD|" + cnnId, reliableChannel,clients);
        clients.Find(c => c.connectionId == cnnId).childCount++;
    }
    private void OnDelChild(int cnnId, float x, float z)
    {
        Send("DELCHILD|" + cnnId + '|' + x.ToString() + '|' + z.ToString(), reliableChannel, clients);
        clients.Find(c => c.connectionId == cnnId).childCount--;
    }

    private void Send(string message, int channelId, int cnnId)
    {
        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == cnnId));
        Send(message, channelId, c);

    }
    private void Send(string message, int channelId, List<ServerClient> c)
    {
        Debug.Log("Sending: " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);
        foreach (ServerClient sc in c)
        {
            NetworkTransport.Send(hostId, sc.connectionId, channelId, msg, message.Length * sizeof(char), out error);
        }
    }

}
