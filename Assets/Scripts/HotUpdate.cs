using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XLuaFramework;
using XLuaFramework.Behaviour;
using XLuaFramework.DataInfo;
using XLuaFramework.Enums;
using XLuaFramework.Managers;
using XLuaFramework.Util;
using Object = UnityEngine.Object;

public class HotUpdate : MonoBehaviour
{
    private byte[] _readPathFileListData;
    private byte[] _serverFileListData;

    private GameObject _loadingObj;
    private LoadingUI _loadingUI;
    //文件数量
    private int _downloadCount;
        
    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="info">单个文件信息</param>
    /// <param name="complete">完成回调</param>
    /// <returns></returns>
    private IEnumerator DownloadFile(DownFileInfo info,Action<DownFileInfo> complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("下载文件出错: "+info.url);
            yield break;
            //重试
        }
        yield return new WaitForSeconds(0.2f);
            
        info.fileData = webRequest.downloadHandler;
        complete?.Invoke(info);
        webRequest.Dispose();
    }

    /// <summary>
    /// 下载多个文件
    /// </summary>
    /// <param name="infos">文件信息</param>
    /// <param name="complete">单个文件完成回调</param>
    /// <param name="downLoadComplete">多个文件完成回调</param>
    /// <returns></returns>
    private IEnumerator DownloadFile(IEnumerable<DownFileInfo> infos, Action<DownFileInfo> complete,
        Action downLoadComplete)
    {
        foreach (DownFileInfo info in infos)
        {
            yield return DownloadFile(info, complete);
        }
        downLoadComplete?.Invoke();
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="fileData">文件信息</param>
    /// <param name="path">路径</param>
    /// <returns>文件信息</returns>
    private List<DownFileInfo> GetFileList(string fileData,string path)
    {
        string content = fileData.Trim().Replace("\r", "");//剔除 \r
        string[] files = content.Split('\n');//切割文件
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');//切割依赖
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }

        return downFileInfos;
    }

    private void Start()
    {
        GameObject go = Resources.Load<GameObject>("LoadingUI");
        _loadingObj = Instantiate(go, this.transform.Find("UI"), true);
        _loadingUI = _loadingObj.GetComponent<LoadingUI>();
        _loadingUI.transform.localPosition = Vector3.zero;
            
        var isfirst = IsFirstInstall();
        if (isfirst)
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }

    private bool IsFirstInstall()
    {
        bool isExistsReadPath = FileUtil.IsExists(Path.Combine(PathUtil.ReadPath, AppConst.FILE_LIST_NAME));
        bool isExistsReadWritePath =
            FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FILE_LIST_NAME));

        return isExistsReadPath && !isExistsReadWritePath;
    }

    private void ReleaseResources()
    {
        _downloadCount = 0;
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FILE_LIST_NAME);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownloadFile(info, OnDownloadReadPathFileListComplete));//下载文件列表
    }



    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.RESOURCES_URL, AppConst.FILE_LIST_NAME);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownloadFile(info, OnDownloadServerFilelistComplete));
    }



    private void OnDownloadReadPathFileListComplete(DownFileInfo file)
    {
        _readPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownloadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
        _loadingUI.InitProgress(fileInfos.Count,"正在加载资源...");
    }



    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log($"OnReleaseFileComplete:{fileInfo.url}");
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.fileName);
        FileUtil.WriteFile(writeFile,fileInfo.fileData.data);
        _downloadCount++;
        _loadingUI.UpdateProgress(_downloadCount);
    }
        
    /// <summary>
    /// 写入 filelist
    /// </summary>
    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath,AppConst.FILE_LIST_NAME),_readPathFileListData);
        CheckUpdate();
    }
        
        
    private void OnDownloadServerFilelistComplete(DownFileInfo file)
    {
        _downloadCount = 0;
        _serverFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.RESOURCES_URL);
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if (!FileUtil.IsExists(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.RESOURCES_URL, fileInfos[i].fileName);
                downListFiles.Add(fileInfos[i]);
            }
        }

        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownloadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));
            _loadingUI.InitProgress(downListFiles.Count, "正在更新...");
        }
        else
        {
            EnterGame();
        }
    }

    private void EnterGame()
    {
        Manager.EventManager.Publish((int)GameEvent.GameInit);
        Destroy(_loadingObj);
    }

    private void OnUpdateFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log($"OnUpdateFileComplete:{fileInfo.url}");
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.fileName);
        FileUtil.WriteFile(writeFile,fileInfo.fileData.data);
        _downloadCount++;
        _loadingUI.UpdateProgress(_downloadCount);
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath,AppConst.FILE_LIST_NAME),_serverFileListData);
        EnterGame();
        _loadingUI.InitProgress(0,"正在载入...");
    }
        
    private void OnInitAsset(Object obj)
    {
        GameObject go = Instantiate(obj, this.transform, true) as GameObject;
        if (go != null)
        {
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
        }
    }
}