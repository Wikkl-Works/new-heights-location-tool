using Steamworks;
using System;
using System.IO;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

public class WorkshopExporterWindow : EditorWindow
{
    private const uint APP_ID = 2179440;

    private string workshopItemId;
    private string imagePath, changeNotes;
    private bool pending;
    private bool exported;
    private bool uploaded;

    private string error;

    private bool isExistingItem;

    private bool openLegal;
    private string openLegalUrl;
    private bool HasError => !string.IsNullOrWhiteSpace(error);

    [MenuItem("New Heights/Steam Workshop Wizard")]
    public static void ShowWindow()
    {
        var window = GetWindow<WorkshopExporterWindow>();
        window.titleContent = new GUIContent("Steam Workshop");
    }

    private GUIStyle GetBoldTextFieldStyle()
    {
        GUI.contentColor = Color.white;
        GUIStyle textStyle = EditorStyles.textField;
        textStyle.wordWrap = true;
        textStyle.fontStyle = FontStyle.Bold;
        return textStyle;
    }
    private GUIStyle GetBoldStyle()
    {
        GUI.contentColor = Color.white;
        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;
        textStyle.fontStyle = FontStyle.Bold;
        return textStyle;
    }
    private GUIStyle GetNormalStyle()
    {
        GUI.contentColor = Color.white;
        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;
        textStyle.fontStyle = FontStyle.Normal;
        return textStyle;
    }
    private GUIStyle GetItalicStyle()
    {
        GUI.contentColor = Color.white;
        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;
        textStyle.fontStyle = FontStyle.Italic;
        return textStyle;
    }
    private GUIStyle GetErrorStyle()
    {
        GUI.contentColor = new Color(1f, 0.6f, 0.6f);
        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;
        textStyle.fontStyle = FontStyle.Bold;
        return textStyle;
    }

    private void Update()
    {
        if (openLegal)
        {
            Debug.Log("open legal thingy!!");
;            openLegal = false;
            Application.OpenURL(openLegalUrl);
        }
    }

    private void OnGUI()
    {
        // -------------- 0. Steam Things ---------------
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("0. Start Steam & Accept the Agreement", EditorStyles.largeLabel);
        EditorGUILayout.Space();

        if (!SteamAPI.IsSteamRunning())
        {
            StartSteamButton();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.LabelField("Great! It looks like Steam is running.", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Did you already accept the Steam Subscriber Agreement?", EditorStyles.boldLabel);

        if (GUILayout.Button($"Open Steam Subscriber Agreement", GUILayout.ExpandWidth(false)))
            Application.OpenURL("https://steamcommunity.com/workshop/workshoplegalagreement");

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        // End --------------


        // -------------- 1. Export and Test ---------------
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("1. Select or Create Item", EditorStyles.largeLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Are you editing an existing item?", EditorStyles.label);

        isExistingItem = EditorGUILayout.Toggle("Existing item", isExistingItem);

        // Workshop item ID
        if (isExistingItem)
        {
            EditorGUILayout.LabelField("", EditorStyles.boldLabel);
            workshopItemId = EditorGUILayout.TextField("Existing Item ID", workshopItemId, GetBoldTextFieldStyle());
            EditorGUILayout.LabelField("Here you can fill in an existing Steam Workshop Item ID to edit your existing item. Did you create the item here? Then you could find the ID in the export log.", GetItalicStyle());

            if (GUILayout.Button($"Open export log"))
            {
                var path = SteamWorkshopExporter.LogFilePath;
                Debug.Log(path);
                Application.OpenURL(path);
            }
        }
        else
        {
            if (!isExistingItem && GUILayout.Button($"Create New Item", GUILayout.Height(30)))
                CreateMod();
        }

        EditorGUILayout.Space();


        // Workshop Item Link
        if (!string.IsNullOrEmpty(workshopItemId))
        {
            EditorGUILayout.LabelField("Your item:", EditorStyles.boldLabel);
            var url = workshopItemId.GetWorkshopUrl();
            GUIStyle style = new() { richText = true, fontStyle = FontStyle.Bold };
            EditorGUILayout.TextField($"<a href=\"{url}\">{url}</a>", style);
            if (GUILayout.Button($"Open URL", GUILayout.ExpandWidth(false)))
                Application.OpenURL(url);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("If you just created this item, it will be completely empty at first. You can edit the title / description in the Steam Workshop environment. The image can be uploaded in the 3rd step of this wizard.", GetItalicStyle());
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        // End --------------

        GUI.enabled = !pending;

        // -------------- 2. Export and Test ---------------
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("2. Export and Test", EditorStyles.largeLabel);
        EditorGUILayout.Space();

        // Workshop create/upload button

        if (string.IsNullOrEmpty(workshopItemId))
            GUI.enabled = false;

        if (GUILayout.Button($"Export Item To Game Folder", GUILayout.Height(30)))
        {
            if (uint.TryParse(workshopItemId, out var itemId))
                ExportMod((PublishedFileId_t)itemId);
        }
        if (exported)
            EditorGUILayout.LabelField("Export complete!", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Your export can be played in New Heights under the name of item_{workshopItemId}. (When you or others subscribe to the workshop asset, they will see the name you entered in at the Workshop page instead and they will also see the uploaded image.)", GetItalicStyle());
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        // End --------------

        // -------------- 3 . Upload to Workshop ---------------
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("3. Upload to Workshop", EditorStyles.largeLabel);
        EditorGUILayout.Space();

        if (string.IsNullOrEmpty(workshopItemId) || !Directory.Exists(workshopItemId.GetContentPath()))
            GUI.enabled = false;

        // Image directory
        GUILayout.BeginHorizontal();

        EditorGUILayout.TextField("Link to image:", imagePath, GetBoldTextFieldStyle());

        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string selectedDirectory = EditorUtility.OpenFilePanel("Choose thumbnail", imagePath, "png,jpg,gif");
            if (!string.IsNullOrEmpty(selectedDirectory))
                imagePath = selectedDirectory;

            Repaint();
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Must link to a .png or .jpg file. It can be outside the unity project.", GetItalicStyle());
        EditorGUILayout.Space();

        // Change notes

        EditorGUILayout.LabelField("Change Notes", GetBoldStyle());
        EditorGUILayout.LabelField("Write a short description of what has changed in this version of the item.", GetItalicStyle());
        changeNotes = EditorGUILayout.TextArea(changeNotes, GUILayout.MinHeight(50));

        if (GUILayout.Button($"Upload Item To Workshop", GUILayout.Height(30)))
        {
            if (uint.TryParse(workshopItemId, out var itemId))
                UpdateMod((PublishedFileId_t)itemId, workshopItemId.GetContentPath(), imagePath, changeNotes);
        }

        if (pending)
            EditorGUILayout.LabelField("Uploading in background...", EditorStyles.boldLabel);
        if (uploaded)
            EditorGUILayout.LabelField("Upload complete!", EditorStyles.boldLabel);

        if (HasError)
        {
            EditorGUILayout.LabelField(error, GetErrorStyle());
            GUI.contentColor = Color.white;
        }

        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        // End --------------
    }

    void StartSteamButton()
    {
        GUI.enabled = false;
        Color originalBackgroundColor = GUI.backgroundColor;
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        GUI.color = Color.yellow;
        GUILayout.Button($"Startup Steam first..", GUILayout.Height(30));
        GUI.backgroundColor = originalBackgroundColor;
        GUI.color = originalColor;
        GUI.enabled = true;
    }

    private async void CreateMod()
    {
        pending = true;

        using (var exporter = new SteamWorkshopExporter(new AppId_t(APP_ID)))
        {
            exporter.OnOpenSteamLegalAgreement += SetOpenSteamLegal;
            var result = await exporter.CreateWorkshopItem();

            if (result.m_eResult == EResult.k_EResultOK)
            {
                workshopItemId = result.m_nPublishedFileId.ToString();
                isExistingItem = true;
            }
            else
            {
                error = $"Workshop item creation error: {result.m_eResult}. You could check: \n1. Are you logged in Steam with an account that owns a copy of New Heights? \n2. Did you accept the Steam Workshop EULA?";
            }
            exporter.OnOpenSteamLegalAgreement -= SetOpenSteamLegal;
        }

        pending = false;
        Debug.Log("Mod creation finished!");
    }

    private string ExportMod(PublishedFileId_t fileId)
    {
        var outputDirectory = SteamWorkshopUtils.GetModsRootFolder();
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        var modName = $"item_{fileId}";

        ModTool.Editor.Exporting.ExportSettings.outputDirectory = outputDirectory;
        ModTool.Editor.Exporting.ExportSettings.name = modName;
        ModTool.Editor.Exporting.ExportSettings.platforms = ModTool.Shared.ModPlatform.Windows;
        ModTool.Editor.Exporting.ModExporter.ExportMod();

        return $"{outputDirectory}/{modName}";
    }

    private async void UpdateMod(PublishedFileId_t itemId, string contentPath, string imagePath, string changeNotes)
    {
        pending = true;

        using (var exporter = new SteamWorkshopExporter(new AppId_t(APP_ID)))
        {
            exporter.OnOpenSteamLegalAgreement += SetOpenSteamLegal;
            await exporter.UpdateWorkshopItem(itemId, new SteamWorkshopExporter.UpdateItemParams()
            {
                imagePath = imagePath,
                contentPath = contentPath,
                changeNotes = changeNotes
            });
            exporter.OnOpenSteamLegalAgreement -= SetOpenSteamLegal;
        }

        pending = false;
        uploaded = true;
        Debug.Log("Mod update finished!");
        Repaint();
    }

    private void SetOpenSteamLegal(string url)
    {
        openLegal = true;
        openLegalUrl = url;
    }
}
