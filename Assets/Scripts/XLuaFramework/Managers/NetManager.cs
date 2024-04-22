using System.Collections.Generic;
using UnityEngine;
using XLua;
using XLuaFramework.Network;

namespace XLuaFramework.Managers
{
    public class NetManager : MonoBehaviour
    {
        private NetClient _netClient;

        private Queue<KeyValuePair<int, string>> _messageQueue;

        private LuaFunction _receiveMessage;

        private void Awake()
        {
            _messageQueue = new Queue<KeyValuePair<int, string>>();
        }

        public void Init()
        {
            _netClient = new NetClient();
            _receiveMessage = Manager.LuaManager.luaEnv.Global.Get<LuaFunction>("ReceiveMessage");
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        public void SendMessage(int messageId, string message)
        {
            _netClient.SendMessage(messageId,message);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void ConnectedServer(string host,int port)
        {
            _netClient.OnConnectServer(host,port);
        }
        
        

        public  void OnNetConnected()
        {
           
        }
        public  void OnNetDisconnected()
        {
            
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="message"></param>
        public  void Receive(int msgId, string message)
        {
            _messageQueue.Enqueue(new KeyValuePair<int, string>(msgId,message));
        }

        private void Update()
        {
            if (_messageQueue.Count > 0)
            {
                KeyValuePair<int,string> msg = _messageQueue.Dequeue();
                _receiveMessage?.Call(msg.Key,msg.Value);
            }
        }
    }
}