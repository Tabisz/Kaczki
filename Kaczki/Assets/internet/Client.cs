using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player
{
    public string playerName;
    public GameObject avatar;
    public int connectionId;
}
public class Client : MonoBehaviour
{

    private const int MAX_CONNECTION = 10;

    private int port = 3001;
    private string ip = "127.0.0.1";

    private int hostId;

    private int reliableChannel;
    private int unreliableChannel;


    private int ourClientId;
    private int connectionId;

    private float connectionTime;
    private bool isConnected = false;
    private bool isStarted = false;
    private byte error;

    private string playerName;

    public GameObject playerPrefab;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();

    public void Connect()
    {
        string pName = GameObject.Find("NameInput").GetComponent<InputField>().text;
        if (pName == "")
        {
            Debug.Log("Wpisz imie");
            return;
        }
        string IpField = GameObject.Find("IpInput").GetComponent<InputField>().text;
        if (IpField != "")
            ip = GameObject.Find("IpInput").GetComponent<InputField>().text;
        string PortField = GameObject.Find("PortInput").GetComponent<InputField>().text;
        if (PortField != "")
            port = int.Parse(PortField);

        playerName = pName;

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, 0);
        connectionId = NetworkTransport.Connect(hostId, ip, port, 0, out error);
        Debug.Log(error.ToString());
        connectionTime = Time.time;
        isConnected = true;


    }

    void Update()
    {
        if (!isConnected)
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
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
               // Debug.Log("Receiveing: " + msg);
                string[] splitData = msg.Split('|');    

                switch (splitData[0])           // dekodowanie przyjętej wiadomości na podstawie słów kluczowych
                {
                    case "ASKNAME":
                        OnAskName(splitData);
                        break;

                    case "CNN":
                        SpawnPlayer(splitData[1], int.Parse(splitData[2]), 0);
                        break;

                    case "DC":
                        PlayerDisconnected(int.Parse(splitData[1]));
                        break;

                    case "ASKPOSITION":
                        OnAskPosition(splitData);
                        break;

                    case "ADDCHILD":
                        OnAddChild(splitData);
                        break;
                    case "DELCHILD":
                        OnDelChild(splitData);
                        break;

                    default:
                        Debug.Log("Invalid message : " + msg);
                        break;
                }
                break;
        }

    }

    private void OnAskName(string[] data)       //gdy server zapyta o twoje imie
    {
        ourClientId = int.Parse(data[1]);

        Send("NAMEIS|" + playerName, reliableChannel);      


        for (int i = 2; i < data.Length - 1; i++)
        {
            string[] d = data[i].Split('%');

            SpawnPlayer(d[0], int.Parse(d[1]), int.Parse(d[2]));
        }
    }
    private void OnAskPosition(string[] data)
    {
        if (!isStarted)
            return;

        for (int i = 1; i < data.Length; i++)
        {
            string[] d = data[i].Split('%');
            if (ourClientId != int.Parse(d[0]))
            {
                Vector3 position = new Vector3(float.Parse(d[1]), 0.4f, float.Parse(d[2]));
                players[int.Parse(d[0])].avatar.GetComponent<AvatarController>().UpdatePos(position);
            }
        }
        Vector3 myPosition = players[ourClientId].avatar.transform.position;

        string m = "MYPOSITION|" + myPosition.x.ToString() + '|' + myPosition.z.ToString();

        Send(m, unreliableChannel);


    }

    private void OnAddChild(string[] data)// - add child, 1 - playersid 
    {
        if(int.Parse(data[1]) != ourClientId)
        players[int.Parse(data[1])].avatar.GetComponent<HordeHandler>().AddDuck();
    }
    private void OnDelChild(string[] data)// - del child, 1 - playersid , 2 - x, 3 - z 
    {
        if (int.Parse(data[1]) != ourClientId)
            players[int.Parse(data[1])].avatar.GetComponent<HordeHandler>().DelDuck(float.Parse(data[2]), float.Parse(data[3]));
    }

    private void SpawnPlayer(string playerName, int cnnId, int childCount)
    {
        GameObject go = Instantiate(playerPrefab) as GameObject;
        if (cnnId == ourClientId)
        {
            GameObject.Find("CameraHang").GetComponent<CameraMove>().Mother = go;
            go.AddComponent<Rigidbody>();
            go.AddComponent<PlayerController>();
            
            GameObject.Find("Canvas").SetActive(false);
            isStarted = true;
        }
        else
        {
            go.AddComponent<AvatarController>();
            go.tag = "Enemy";

        }

        Player p = new Player();
        players.Add(cnnId, p);
        p.avatar = go;
        p.playerName = playerName;
        p.connectionId = cnnId;
        p.avatar.GetComponentInChildren<TextMesh>().text = playerName;
        for(int i = 0; i<childCount;i++)
        p.avatar.GetComponent<HordeHandler>().LocalAddDuck();



    }
    private void PlayerDisconnected(int cnnId)

    {
        Destroy(players[cnnId].avatar);
        players.Remove(cnnId);
    }
    private void Send(string message, int channelId)
    {
       // Debug.Log("Sending: " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);

    }

    //dodawanie/ usuwanie małych kaczek
    public void AddChild()
    {
        Send("ADDCHILD|", reliableChannel);
    }
    public void DelChild(Vector3 dst)
    {

        Send("DELCHILD|" + dst.x.ToString() + '|' + dst.z.ToString(), reliableChannel);
    }




}
