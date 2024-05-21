using Steamworks;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamWorkshopExporter : IDisposable
{
    private TaskCompletionSource<CreateItemResult_t> createItemSource;
    private TaskCompletionSource<SubmitItemUpdateResult_t> updateItemSource;

    private CancellationTokenSource steamCallbackRunner;
    private AppId_t appId;

    public Action<string> OnOpenSteamLegalAgreement;

    public static string LogFilePath => $"{Application.dataPath}/export_log.txt";

    public class UpdateItemParams
    {
        public string contentPath;
        public string imagePath;
        public string changeNotes;
    }

    public SteamWorkshopExporter(AppId_t appId)
    {
        this.appId = appId;

        SteamAPI.Init();

        steamCallbackRunner = new CancellationTokenSource();

        Task.Run(async () =>
         {
             while (!steamCallbackRunner.IsCancellationRequested)
             {
                 SteamAPI.RunCallbacks();
                 await Task.Delay(50);
             }
         }, steamCallbackRunner.Token);
    }

    // Create new items
    public async Task<CreateItemResult_t> CreateWorkshopItem()
    {
        Debug.Log($"Creating new workshop item...");

        createItemSource = new();

        var createHandle = SteamUGC.CreateItem(appId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        var callResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(HandleCreateItemResult));
        callResult.Set(createHandle);

        return await createItemSource.Task;
    }

    private void HandleCreateItemResult(CreateItemResult_t result, bool bIOFailure)
    {
        Debug.Log("Creating item...");
        if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
        {
            var url = result.m_nPublishedFileId.GetWorkshopUrl();
            WriteLog($"Workshop item creation failed. ID:{result.m_eResult}", error: true);
            OnOpenSteamLegalAgreement?.Invoke(url);
        }

        if (result.m_eResult == EResult.k_EResultOK)
        {
            WriteLog($"Workshop item created. ID:{result.m_nPublishedFileId} URL:https://steamcommunity.com/sharedfiles/filedetails/?id={result.m_nPublishedFileId}", debug: true);
        }
        else
        {
            WriteLog($"Workshop item creation failed. ID:{result.m_eResult}", error:true);
        }

        createItemSource.SetResult(result);
    }

    // Update items
    public async Task<SubmitItemUpdateResult_t> UpdateWorkshopItem(PublishedFileId_t itemId, UpdateItemParams updateItemParams)
    {
        Debug.Log("Updating item...");

        updateItemSource = new();

        // Initialize the item update
        var updateHandle = SteamUGC.StartItemUpdate(appId, itemId);

        // Sets the folder that will be stored as the content for an item. (https://partner.steamgames.com/doc/api/ISteamUGC#SetItemContent)
        SteamUGC.SetItemContent(updateHandle, updateItemParams.contentPath);

        if (!string.IsNullOrEmpty(updateItemParams.imagePath))
        {
            // Sets the primary preview image for the item. (https://partner.steamgames.com/doc/api/ISteamUGC#SetItemPreview)
            SteamUGC.SetItemPreview(updateHandle, updateItemParams.imagePath);
        }

        SteamUGC.SetItemTags(updateHandle, new string[] { "location" });

        var itemUpdateHandle = SteamUGC.SubmitItemUpdate(updateHandle, updateItemParams.changeNotes);
        var callResult = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(HandleItemUpdateResult));
        callResult.Set(itemUpdateHandle);

        return await updateItemSource.Task;
    }

    private void HandleItemUpdateResult(SubmitItemUpdateResult_t result, bool bIOFailure)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            WriteLog($"Update Item success: ID:{result.m_nPublishedFileId} {result.m_nPublishedFileId.GetWorkshopUrl()}", debug:true);
        else
            WriteLog($"Workshop item update failed: {result.m_eResult}", error:true);

        updateItemSource.SetResult(result);
    }

    public void Dispose()
    {
        steamCallbackRunner.Cancel();
        SteamAPI.Shutdown();
    }

    private void WriteLog(string line, bool debug = false, bool error = false)
    {
        using (var file = File.AppendText(LogFilePath))
        {
            file.WriteLine(line);
        }

        if (debug)
            Debug.Log(line);

        if (error)
            Debug.LogError(line);
    }
}
