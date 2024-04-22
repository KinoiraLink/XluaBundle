using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Exception = System.Exception;

namespace XLuaFramework.Network
{
    public class NetClient
    {
        private const int BUFFER_SIZE = 1024 * 64;
        
        private TcpClient _client;
        private NetworkStream _tcpStream;
        private byte[] _buffer;
        private MemoryStream _memoryStream;
        private BinaryReader _binaryReader;

        public NetClient()
        {
            _buffer = new byte[BUFFER_SIZE];
            _memoryStream = new MemoryStream();
            _binaryReader = new BinaryReader(_memoryStream);
        }

        public void OnConnectServer(string host, int port)
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                if (addresses.Length == 0)
                {
                    Debug.LogError("host invalid");
                    return;
                }

                _client = addresses[0].AddressFamily == AddressFamily.InterNetworkV6 ? new TcpClient(AddressFamily.InterNetworkV6) : new TcpClient(AddressFamily.InterNetwork);
                _client.SendTimeout = 1000;
                _client.ReceiveTimeout = 1000;
                _client.NoDelay = true;
                _client.BeginConnect(host, port, OnConnect, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            if (_client == null || !_client.Connected)
            {
                Debug.LogError("connect server error!");
                return;
            }

            Managers.Manager.NetManager.OnNetConnected();
            _tcpStream = _client.GetStream();
            _tcpStream.BeginRead(_buffer, 0, BUFFER_SIZE, OnRead, null);
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                if (_client == null || _tcpStream == null)
                    return;

                //收到的消息的长度
                int length = _tcpStream.EndRead(ar);
                if (length < 1)
                {
                    OnDisConnected();
                    return;
                }

                ReceiveData(length);
                lock (_tcpStream)
                {
                    Array.Clear(_buffer,0,_buffer.Length);//清空数据
                    _tcpStream.BeginRead(_buffer, 0, BUFFER_SIZE, OnRead, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                OnDisConnected();
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        private void ReceiveData(int len)
        {
            _memoryStream.Seek(0, SeekOrigin.End);//指向末尾,方便追加写入
            _memoryStream.Write(_buffer,0,len);//写入内存
            _memoryStream.Seek(0, SeekOrigin.Begin);//指向开头，方便读取
            
            while (RemainingBytesLength() >= 8)
            {
                int msgId = _binaryReader.ReadInt32();
                int msgLen = _binaryReader.ReadInt32();
                //剩余数据是否完整
                if (RemainingBytesLength() >= msgLen)
                {
                    //数据正确，读取
                    byte[] data = _binaryReader.ReadBytes(msgLen);
                    //json数据
                    string message = System.Text.Encoding.UTF8.GetString(data);

                    Managers.Manager.NetManager.Receive(msgId, message);
                }
                else
                {
                    //数据错误，退还
                    _memoryStream.Position = _memoryStream.Position - 8;
                    break;
                }
            }
            //剩余字节
            byte[] leftover = _binaryReader.ReadBytes(RemainingBytesLength());
            _memoryStream.SetLength(0);
            _memoryStream.Write(leftover,0,leftover.Length);
        }

        /// <summary>
        /// 同时多条消息的判断处理
        /// </summary>
        /// <returns></returns>
        private int RemainingBytesLength()
        {
            return (int)(_memoryStream.Length - _memoryStream.Position);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgID">数据ID</param>
        /// <param name="message">Json</param>
        public void SendMessage(int msgID, string message)
        {
            using MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            BinaryWriter bw = new BinaryWriter(ms);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            bw.Write(msgID);
            bw.Write(data.Length);
            bw.Write(data);
            bw.Flush();
            if (_client is { Connected: true })
            {
                byte[] sendData = ms.ToArray();
                _tcpStream.BeginWrite(sendData, 0, sendData.Length, OnEndSend, null);
            }
            else
            {
                Debug.LogError("服务器未连接");
            }
        }

        private void OnEndSend(IAsyncResult ar)
        {
            try
            {
                _tcpStream.EndWrite(ar);//结束发送
            }
            catch (Exception e)
            {
                OnDisConnected();
                Debug.LogError(e);
                
            }
        }

        private void OnDisConnected()
        {
            if (_client is { Connected: true })
            {
                _client.Close();
                _client = null;
                _tcpStream.Close();
                _tcpStream = null;
            }

            Managers.Manager.NetManager.OnNetDisconnected();
        }
    }
}