using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using VioletTemplate.Main.Extentions;
using static VioletTemplate.Main.CreateButtons;
using static VioletTemplate.Main.Extentions.Addons;
using static VioletTemplate.Main.Extentions.PlayerManager;

namespace VioletTemplate.Main
{
    public class Core : MonoBehaviour
    {
        #region Variables

        public static string MenuName = "Violet Free V 0.1";
        public static bool MenuOpen = false;
        public static bool RightHand = true;
        public static bool RigidBody = true;
        public static bool Colliders = false;
        public static InputType MenuOpenButton = InputType.LSecondary;
        public static KeyCode PcOpenKey = KeyCode.F;

        public static GameObject Menu;
        public static GameObject MenuBackground;
        public static GameObject MenuCatagoryBackground;
        public static GameObject MenuPlayerBackground;
        public static GameObject Settings;
        public static GameObject NextPageButton;
        public static GameObject BackPageButton;
        public static GameObject Buttons;
        public static GameObject Canvas;
        public static GameObject clickerObj;
        public static GameObject TitleObject;
        public static GameObject CatagoryTextObject;

        public static Color MainColor = Color.black;
        public static Color SecondaryColor = Color.violet;
        public static float DestroyDelay = 0;
        public static int Theme = 1;
        public static int ButtonsPerPage = 6;
        public static int currentCategoryPage = 0;
        public static Color ButtonColorOn = Color.violet;
        public static Color ButtonColorOff = Color.darkViolet;

        public static Catagorys CurrentPage = Catagorys.Home;
        public static Rigidbody rb;
        public static List<Button> buttons = new List<Button>();

        public static GameObject camerea;
        private static Camera shoulderCamera;
        private static Vector3 menuOffset = new Vector3(0.5f, 0.3f, 1f);
        private static float menuDistance = 1;
        private static bool isVRMode = true;
        private static bool lastVRInputState = false;
        private static bool lastPCInputState = false;
        private static float lastVRTriggerTime = 0f;

        public static int PgNumber = 0;

        #endregion

        public void Awake()
        {
            try
            {
                RebuildMenu();
                CreateAllButtons();
                camerea = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
                if (camerea != null)
                    shoulderCamera = camerea.GetComponent<Camera>();
                else
                    shoulderCamera = Camera.main;

                isVRMode = GetLeftHand() != null;
            }
            catch (Exception) { }
        }

        private void Update()
        {
            try
            {
                bool vrInput = GetMenuInput();
                bool pcInput = GetPcMenuInput();
                bool vrInputPressed = vrInput && !lastVRInputState;
                bool pcInputPressed = pcInput && !lastPCInputState;

                lastVRInputState = vrInput;
                lastPCInputState = pcInput;

                Board();

                // Open / Close Menu
                if ((vrInputPressed || pcInputPressed) && !MenuOpen)
                {
                    isVRMode = vrInputPressed && GetLeftHand() != null;
                    CreateMenuObjects();

                    if (isVRMode && Colliders)
                    {
                        GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1")?.SetActive(false);
                    }
                    if (!isVRMode && Colliders)
                    {
                        var colliders = Menu.GetComponentsInChildren<Collider>();
                        foreach (var collider in colliders)
                            collider.enabled = true;
                    }

                    MenuOpen = true;
                }
                else if ((vrInputPressed || pcInputPressed) && MenuOpen)
                {
                    if (isVRMode)
                        GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1")?.SetActive(true);

                    MenuOpen = false;
                    if (Menu != null)
                    {
                        StartCoroutine(DestroyMenuAfterDelay(Menu, DestroyDelay));
                    }
                }

                if (MenuOpen && Menu != null)
                {
                    UpdateMenuPosition();
                    if (!isVRMode)
                        HandlePCMouseInput();
                }

                HandleButtonActions();
            }
            catch (Exception) { }
        }

        #region Input Handling

        private void HandlePCMouseInput()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && shoulderCamera != null)
            {
                Ray ray = shoulderCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

                if (hits.Length > 0)
                {
                    hits = hits.OrderBy(h => h.distance).ToArray();
                    foreach (var hit in hits)
                    {
                        var btnCollider = hit.collider?.GetComponent<BtnCollider>();
                        if (btnCollider != null && btnCollider.clickedButton != null)
                        {
                            hit.collider.transform.localScale *= 0.9f;
                            ToggleButton(btnCollider.clickedButton);

                            if (!string.IsNullOrEmpty(btnCollider.clickedButton.Description))
                                Notifications.Show("Button: " + btnCollider.clickedButton.Name, btnCollider.clickedButton.Description, Color.white);

                            RefreshMenu();
                            break;
                        }
                    }
                }
            }
        }

        public static bool GetMenuInput()
        {
            try { return GetInput(MenuOpenButton); }
            catch (Exception) { return false; }
        }

        public static bool GetPcMenuInput()
        {
            try { return UnityInput.Current.GetKeyDown(PcOpenKey); }
            catch (Exception) { return false; }
        }

        #endregion

        #region Buttons

        private static void HandleButtonActions()
        {
            foreach (Button button in buttons)
            {
                try
                {
                    if (button.Enabled && button.OnClick != null)
                        button.OnClick.Invoke();
                }
                catch (Exception) { }
            }
        }

        private static void ToggleButton(Button button)
        {
            try
            {
                if (!button.Toggle)
                {
                    button.OnClick?.Invoke();
                }
                else
                {
                    button.Enabled = !button.Enabled;
                    if (button.Enabled) button.OnClick?.Invoke();
                    else button.OnDisable?.Invoke();
                }
                RefreshMenu();
            }
            catch (Exception) { }
        }

        #endregion

        #region Paging

        public static void NavigatePage(bool forward)
        {
            int totalPages = GetTotalPages(CurrentPage);
            int lastPage = totalPages - 1;

            currentCategoryPage += forward ? 1 : -1;
            if (currentCategoryPage < 0)
                currentCategoryPage = lastPage;
            else if (currentCategoryPage > lastPage)
                currentCategoryPage = 0;

            RebuildMenu();
        }

        public static void ReturnToMainPage()
        {
            CurrentPage = Catagorys.Home;
            currentCategoryPage = 0;
            RebuildMenu();
        }

        private static int GetTotalPages(Catagorys page)
        {
            return (buttons.Count(b => b.Catagory == page) + ButtonsPerPage - 1) / ButtonsPerPage;
        }

        private static List<Button> GetButtonInfoByPage(Catagorys page)
        {
            return buttons.Where(b => b.Catagory == page).ToList();
        }

        public static void ChangePage(Catagorys page)
        {
            currentCategoryPage = 0;
            CurrentPage = page;
            RebuildMenu();
        }

        public static void ChangeButtonText(string CurrentButtonText, string NewText)
        {
            foreach (var button in buttons)
            {
                try
                {
                    if (button.Name.Contains(CurrentButtonText))
                    {
                        button.SetText(NewText);
                        RefreshMenu();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error while changing text of button '{button.Name}': {ex.Message}\nStack Trace: {ex.StackTrace}");
                }
            }
        }

        #endregion

        #region Menu Construction & UI

        public static void CreateMenuObjects()
        {
            Menu = new GameObject("Menu");
            Menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);

            if (isVRMode || Colliders)
            {
                var menuCollider = Menu.AddComponent<BoxCollider>();
                menuCollider.isTrigger = true;
            }

            CreateButtons();

            if (XRSettings.isDeviceActive)
            {
                if (RightHand) AddButtonClicker(GetRightHand());
                else AddButtonClicker(GetLeftHand());
            }

            // Backgrounds
            MenuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MenuBackground.name = "MenuBackground";
            AddOutline(MenuBackground, Color.violet);
            MenuBackground.transform.parent = Menu.transform;
            MenuBackground.transform.rotation = Quaternion.identity;
            MenuBackground.transform.localScale = new Vector3(0.11f, 0.97f, 0.88f);
            MenuBackground.transform.localPosition = new Vector3(0.29f, 0f, 0f);
            MenuBackground.GetComponent<Renderer>().material.color = MainColor;

            MenuCatagoryBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MenuCatagoryBackground.name = "MenuCatagoryBackground";
            AddOutline(MenuCatagoryBackground, Color.violet);
            MenuCatagoryBackground.transform.parent = Menu.transform;
            MenuCatagoryBackground.transform.rotation = Quaternion.identity;
            MenuCatagoryBackground.transform.localScale = new Vector3(0.11f, 0.24f, 0.88f);
            MenuCatagoryBackground.transform.localPosition = new Vector3(0.29f, -0.67f, 0f);
            MenuCatagoryBackground.GetComponent<Renderer>().material.color = MainColor;

            MenuPlayerBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MenuPlayerBackground.name = "MenuPlayerBackground";
            AddOutline(MenuPlayerBackground, Color.violet);
            MenuPlayerBackground.transform.parent = Menu.transform;
            MenuPlayerBackground.transform.rotation = Quaternion.identity;
            MenuPlayerBackground.transform.localScale = new Vector3(0.11f, 0.24f, 0.88f);
            MenuPlayerBackground.transform.localPosition = new Vector3(0.29f, 0.67f, 0f);
            MenuPlayerBackground.GetComponent<Renderer>().material.color = MainColor;

            // Settings / Home Button
            Settings = GameObject.CreatePrimitive(PrimitiveType.Cube);
            AddOutline(Settings, Color.violet);
            Settings.transform.parent = MenuBackground.transform;
            Settings.transform.rotation = Quaternion.identity;
            Settings.transform.localScale = new Vector3(0.5f, 0.28f, 0.1f);
            Settings.transform.localPosition = new Vector3(0.36f, 0f, -0.34f);

            var settingsCollider = Settings.AddComponent<BtnCollider>();
            settingsCollider.clickedButton = CurrentPage == Catagorys.Settings
                ? new Button("Settings", "", Catagorys.Home, () => ChangePage(Catagorys.Home), null, false, false, false)
                : new Button("home", "", Catagorys.Home, () => ChangePage(Catagorys.Settings), null, false, false, false);

            Settings.GetComponent<Renderer>().material.color = MainColor;

            // Settings Text
            GameObject settingsTextObject = new GameObject("SettingsText");
            settingsTextObject.transform.parent = Settings.transform;
            TextMeshPro settingsText = settingsTextObject.AddComponent<TextMeshPro>();
            settingsText.text = CurrentPage == Catagorys.Settings ? "Home" : "Settings";
            settingsText.fontStyle = FontStyles.Bold;
            settingsText.fontSize = 0.3f;
            Addons.ApplyGradient(settingsText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            settingsText.alignment = TextAlignmentOptions.Center;
            settingsText.enableAutoSizing = true;
            settingsTextObject.transform.localPosition = new Vector3(0.86f, 0f, 0);
            settingsTextObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            settingsTextObject.transform.localScale = new Vector3(0.05f, 0.12f, 0.04f);

            // Disconnect / Join Random Button
            string CurrentRoom = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in Room";

            GameObject DissconectButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            AddOutline(DissconectButton, Color.violet);
            DissconectButton.transform.parent = MenuBackground.transform;
            DissconectButton.transform.rotation = Quaternion.identity;
            DissconectButton.transform.localScale = new Vector3(0.5f, 0.8f, 0.14f);
            DissconectButton.transform.localPosition = new Vector3(0f, 0f, 0.6f);

            var DissconectButtonThing = DissconectButton.AddComponent<BtnCollider>();
            DissconectButtonThing.clickedButton = PhotonNetwork.InRoom && !string.IsNullOrEmpty(CurrentRoom)
                ? new Button("Disconnect", "", Catagorys.Home, () => PhotonNetwork.Disconnect(), null, false, false, false)
                : new Button("JoinRand", "", Catagorys.Home, () => PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.GetSelectedMapJoinTrigger()), null, false, false, false);

            DissconectButton.GetComponent<Renderer>().material.color = MainColor;

            GameObject DissconectTextObj = new GameObject("SettingsText");
            DissconectTextObj.transform.parent = DissconectButton.transform;
            TextMeshPro DissconectText = DissconectTextObj.AddComponent<TextMeshPro>();
            DissconectText.text = PhotonNetwork.InRoom && !string.IsNullOrEmpty(CurrentRoom) ? "Disconnect" : "Join Rand";
            DissconectText.fontStyle = FontStyles.Bold;
            DissconectText.fontSize = 0.3f;
            Addons.ApplyGradient(DissconectText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            DissconectText.alignment = TextAlignmentOptions.Center;
            DissconectText.enableAutoSizing = true;
            DissconectTextObj.transform.localPosition = new Vector3(0.86f, 0f, 0);
            DissconectTextObj.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            DissconectTextObj.transform.localScale = new Vector3(0.035f, 0.2f, 0.04f);

            // Page Navigation Buttons
            NextPageButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            AddOutline(NextPageButton, Color.violet);
            NextPageButton.transform.parent = MenuBackground.transform;
            NextPageButton.transform.rotation = Quaternion.identity;
            NextPageButton.transform.localScale = new Vector3(0.4f, 0.3f, 0.1f);
            NextPageButton.transform.localPosition = new Vector3(0.36f, -0.32f, -0.34f);

            var nextPageCollider = NextPageButton.AddComponent<BtnCollider>();
            nextPageCollider.clickedButton = new Button("Next Page", "", Catagorys.Home, () => NavigatePage(true), null, false, false, false);
            NextPageButton.GetComponent<Renderer>().material.color = MainColor;

            GameObject nextPageTextObject = new GameObject("NextPageText");
            nextPageTextObject.transform.parent = NextPageButton.transform;
            TextMeshPro nextPageText = nextPageTextObject.AddComponent<TextMeshPro>();
            nextPageText.text = ">";
            nextPageText.fontStyle = FontStyles.Bold;
            nextPageText.fontSize = 0.7f;
            Addons.ApplyGradient(nextPageText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            nextPageText.alignment = TextAlignmentOptions.Center;
            nextPageText.enableAutoSizing = true;
            nextPageTextObject.transform.localPosition = new Vector3(0.7f, 0f, 0);
            nextPageTextObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            nextPageTextObject.transform.localScale = new Vector3(0.1f, 0.24f, 0.04f);

            BackPageButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            AddOutline(BackPageButton, Color.violet);
            BackPageButton.transform.parent = MenuBackground.transform;
            BackPageButton.transform.rotation = Quaternion.identity;
            BackPageButton.transform.localScale = new Vector3(0.4f, 0.3f, 0.1f);
            BackPageButton.transform.localPosition = new Vector3(0.36f, 0.32f, -0.34f);

            var backPageCollider = BackPageButton.AddComponent<BtnCollider>();
            backPageCollider.clickedButton = new Button("Back Page", "", Catagorys.Home, () => NavigatePage(false), null, false, false, false);
            BackPageButton.GetComponent<Renderer>().material.color = MainColor;

            GameObject backPageTextObject = new GameObject("BackPageText");
            backPageTextObject.transform.parent = BackPageButton.transform;
            TextMeshPro backPageText = backPageTextObject.AddComponent<TextMeshPro>();
            backPageText.text = "<";
            backPageText.fontStyle = FontStyles.Bold;
            backPageText.fontSize = 0.7f;
            Addons.ApplyGradient(backPageText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            backPageText.alignment = TextAlignmentOptions.Center;
            backPageText.enableAutoSizing = true;
            backPageTextObject.transform.localPosition = new Vector3(0.7f, 0f, 0);
            backPageTextObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            backPageTextObject.transform.localScale = new Vector3(0.1f, 0.24f, 0.04f);

            // Title & Category Text
            TitleObject = new GameObject("TitleText");
            TitleObject.transform.parent = MenuBackground.transform;
            TextMeshPro titletext = TitleObject.AddComponent<TextMeshPro>();
            titletext.text = MenuName;
            titletext.fontStyle = FontStyles.Bold;
            titletext.fontSize = 0.9f;
            titletext.material = new Material(Shader.Find("UI/Default"));
            titletext.AddComponent<MovingGradient>();
            titletext.alignment = TextAlignmentOptions.Center;
            titletext.enableAutoSizing = true;
            TitleObject.transform.localPosition = new Vector3(1.5f, 0f, 0.42f);
            TitleObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            TitleObject.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

            CatagoryTextObject = new GameObject("CatagoryText");
            CatagoryTextObject.transform.parent = MenuBackground.transform;
            TextMeshPro tmpText = CatagoryTextObject.AddComponent<TextMeshPro>();
            tmpText.text = "Code: " + CurrentRoom.ToLower();
            tmpText.fontSize = 0.9f;
            tmpText.fontStyle = FontStyles.Bold;
            tmpText.material = new Material(Shader.Find("UI/Default"));
            Addons.ApplyGradient(tmpText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableAutoSizing = true;
            CatagoryTextObject.transform.localPosition = new Vector3(1.5f, 0f, -0.44f);
            CatagoryTextObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            CatagoryTextObject.transform.localScale = new Vector3(0.025f, 0.02f, 0.02f);

            DrawCategoryTabs();
        }

        private static void DrawCategoryTabs()
        {
            if (PgNumber == 0)
            {
                DrawCategoryTab(0f, "Settings", () => ChangePage(Catagorys.Settings));
                DrawCategoryTab(1f, "Room", () => ChangePage(Catagorys.Room));
                DrawCategoryTab(2f, "Movement", () => ChangePage(Catagorys.Movement));
                DrawCategoryTab(3f, "Comp", () => ChangePage(Catagorys.Comp));
                DrawCategoryTab(4f, "Player", () => ChangePage(Catagorys.Player));
                DrawCategoryTab(5f, "World", () => ChangePage(Catagorys.World));
                DrawCategoryTab(6f, "Visual", () => ChangePage(Catagorys.Visuals));
                DrawCategoryTab(7f, "Overpowered", () => ChangePage(Catagorys.Overpowered));
                DrawCategoryTab(8f, "Projectiles", () => ChangePage(Catagorys.Projectiles));
                DrawCategoryTab(9f, "Next Page", () => PgNumber = 1);
            }
            else if (PgNumber == 1)
            {
                DrawCategoryTab(0f, "Spammers", () => ChangePage(Catagorys.Spammers));
                DrawCategoryTab(9f, "Back Page", () => PgNumber = 0);
            }
        }

        private static void DrawCategoryTab(float multiplier, string name, Action action, Vector3 textOffset = default)
        {
            float positionOffset = multiplier * 0.08f;
            var btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btn.GetComponent<BoxCollider>().isTrigger = true;
            btn.transform.parent = Menu.transform;
            btn.transform.rotation = Quaternion.identity;
            btn.transform.localScale = new Vector3(0.03f, 0.2f, 0.06f);
            btn.transform.localPosition = new Vector3(0.56f, RightHand ? -0.66f : 0.66f, 0.38f - positionOffset);

            var col = btn.AddComponent<BtnCollider>();
            col.clickedButton = new Button("CatButton", "", Catagorys.Home, action, null, false, false, false);
            btn.GetComponent<Renderer>().material.color = ButtonColorOff;

            GameObject textObject = new GameObject("CategoryText");
            textObject.transform.parent = btn.transform;
            TextMeshPro tmpText = textObject.AddComponent<TextMeshPro>();
            tmpText.text = name;
            tmpText.fontSize = 0.9f;
            tmpText.fontStyle = FontStyles.Bold;
            Addons.ApplyGradient(tmpText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableAutoSizing = true;
            textObject.transform.localPosition = new Vector3(1.5f, 0f, -0.05f);
            textObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            textObject.transform.localScale = new Vector3(0.05f, 0.12f, 0.02f);
        }

        private static void CreateButtons()
        {
            ButtonPool.ResetPool();
            var buttonsToDraw = GetButtonInfoByPage(CurrentPage)
                .Skip(currentCategoryPage * ButtonsPerPage)
                .Take(ButtonsPerPage)
                .ToArray();

            for (int i = 0; i < buttonsToDraw.Length; i++)
            {
                AddModButton(i * 0.09f, buttonsToDraw[i]);
            }
        }

        private static void AddModButton(float offset, Button button)
        {
            var modButton = ButtonPool.GetButton();
            Destroy(modButton.GetComponent<Rigidbody>());

            var btnCollider = modButton.GetComponent<BoxCollider>();
            if (btnCollider != null)
            {
                btnCollider.isTrigger = true;
                btnCollider.size = new Vector3(1.5f, 1.5f, 1.5f);
                btnCollider.enabled = true;
            }

            modButton.transform.SetParent(Menu.transform, false);
            modButton.transform.rotation = Quaternion.identity;
            modButton.transform.localScale = new Vector3(0.09f, 0.84f, 0.08f);
            modButton.transform.localPosition = new Vector3(0.36f, 0f, 0.28f - offset);

            var btnColScript = modButton.GetComponent<BtnCollider>() ?? modButton.AddComponent<BtnCollider>();
            btnColScript.clickedButton = button;

            GameObject textObject = new GameObject("PlayerText");
            textObject.transform.parent = Menu.transform;
            TextMeshPro tmpText = textObject.AddComponent<TextMeshPro>();
            tmpText.text = button.Name;
            tmpText.fontSize = 0.16f;
            tmpText.fontStyle = FontStyles.Bold;
            Addons.ApplyGradient(tmpText, Color.white, Color.whiteSmoke, Color.violet, Color.whiteSmoke);
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableAutoSizing = true;
            textObject.transform.localPosition = new Vector3(0.425f, 0f, 0.276f - offset);
            textObject.transform.localRotation = Quaternion.Euler(180f, 90f, 90f);
            textObject.transform.localScale = new Vector3(0.018f, 0.015f, 0.1f);

            var btnRenderer = modButton.GetComponent<Renderer>();
            if (btnRenderer != null)
            {
                btnRenderer.material.color = button.Enabled ? ButtonColorOn : ButtonColorOff;
            }
        }

        public static void UpdateMenuPosition()
        {
            if (Menu != null && MenuOpen)
            {
                if (isVRMode)
                {
                    var leftHand = GetLeftHand();
                    if (leftHand != null)
                    {
                        Menu.transform.position = leftHand.position;
                        Menu.transform.rotation = leftHand.rotation;
                    }
                }
                else if (shoulderCamera != null)
                {
                    Menu.transform.position = shoulderCamera.transform.position + shoulderCamera.transform.TransformDirection(menuOffset) * menuDistance;

                    Vector3 directionToCamera = (shoulderCamera.transform.position - Menu.transform.position).normalized;
                    directionToCamera.y = 0f;

                    if (directionToCamera != Vector3.zero)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
                        Quaternion additionalRotation = Quaternion.Euler(270f, 0f, 270);
                        Menu.transform.rotation = lookRotation * additionalRotation;
                    }
                    else
                    {
                        Vector3 euler = Menu.transform.rotation.eulerAngles;
                        euler.y = 0f;
                        Menu.transform.rotation = Quaternion.Euler(euler);
                    }
                }
            }
        }

        public static void CreateCanvas()
        {
            if (Canvas == null)
            {
                Canvas = new GameObject("Canvas");
                Canvas.transform.parent = Menu.transform;
                Canvas.transform.localPosition = Vector3.zero;
                Canvas.transform.localRotation = Quaternion.identity;

                Canvas canvas = Canvas.AddComponent<Canvas>();
                var canvasScaler = Canvas.AddComponent<CanvasScaler>();
                Canvas.AddComponent<GraphicRaycaster>();

                canvas.renderMode = RenderMode.WorldSpace;
                canvasScaler.dynamicPixelsPerUnit = 100;
                canvasScaler.referencePixelsPerUnit = 100;

                var rectTransform = Canvas.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
                rectTransform.localScale = Vector3.one * 0.01f;
            }
        }

        #endregion

        #region Cleanup

        public static void RebuildMenu()
        {
            try
            {
                if (Menu != null)
                {
                    Destroy(Menu);
                    Menu = null;
                    MenuBackground = null;
                    MenuCatagoryBackground = null;
                    MenuPlayerBackground = null;
                    Buttons = null;
                    Canvas = null;
                    TitleObject = null;
                    CatagoryTextObject = null;
                    Settings = null;
                    NextPageButton = null;
                    BackPageButton = null;
                    Destroy(clickerObj);
                    clickerObj = null;
                }

                CreateMenuObjects();

                if (CatagoryTextObject != null)
                {
                    var tmpText = CatagoryTextObject.GetComponent<TextMeshPro>();
                    if (tmpText != null)
                    {
                        tmpText.text = "Category: " + CurrentPage.ToString();
                    }
                }
            }
            catch (Exception) { }
        }

        public static void RefreshMenu()
        {
            ClearMenuObjects();
            CreateMenuObjects();
        }

        public static void ClearMenuObjects()
        {
            DestroyObject(ref Menu);
            DestroyObject(ref MenuBackground);
            rb = null;
        }

        public static void CleanupMenu(float delay = 0f)
        {
            DestroyObject(ref Menu, delay);
            DestroyObject(ref clickerObj);
            rb = null;
        }

        private static IEnumerator DestroyMenuAfterDelay(GameObject menu, float delay)
        {
            Destroy(clickerObj);
            yield return new WaitForSeconds(delay);

            if (menu != null)
            {
                Destroy(menu);
                Menu = null;
                MenuBackground = null;
                MenuCatagoryBackground = null;
                MenuPlayerBackground = null;
                Buttons = null;
                Canvas = null;
                TitleObject = null;
                CatagoryTextObject = null;
                Settings = null;
                NextPageButton = null;
                BackPageButton = null;
            }
        }

        public static void DestroyObject<T>(ref T obj, float delay = 0f) where T : UnityEngine.Object
        {
            if (obj != null)
            {
                if (obj is Component component)
                {
                    UnityEngine.Object.Destroy(component.gameObject, delay);
                }
                else
                {
                    UnityEngine.Object.Destroy(obj, delay);
                }
                obj = null;
            }
        }

        #endregion

        #region Button Clicker & VR Interaction

        public static void AddButtonClicker(Transform parentTransform)
        {
            clickerObj = new GameObject("buttonclicker");
            var clickerCollider = clickerObj.AddComponent<SphereCollider>();
            clickerCollider.isTrigger = true;
            clickerCollider.radius = 0.01f;

            var meshFilter = clickerObj.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

            var clickerRenderer = clickerObj.AddComponent<MeshRenderer>();
            clickerRenderer.material.color = Color.black;
            clickerRenderer.material.shader = Shader.Find("GUI/Text Shader");

            if (parentTransform != null)
            {
                clickerObj.transform.parent = parentTransform;
                clickerObj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                clickerObj.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            }
        }

        public class BtnCollider : MonoBehaviour
        {
            public Button clickedButton;

            private void OnTriggerEnter(Collider other)
            {
                if (!Core.isVRMode || other.gameObject != Core.clickerObj) return;
                if (Time.time <= Core.lastVRTriggerTime + 0.1f) return;

                Core.lastVRTriggerTime = Time.time;
                GorillaTagger.Instance.StartVibration(!Core.RightHand, GorillaTagger.Instance.tagHapticStrength / 2, GorillaTagger.Instance.tagHapticDuration / 2);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, Core.RightHand, 1);

                Core.ToggleButton(clickedButton);
                Core.RefreshMenu();
            }
        }

        #endregion

        #region Object Pooling

        public static class ButtonPool
        {
            private static List<GameObject> buttonPool = new List<GameObject>();
            private static int currentIndex = 0;

            public static GameObject GetButton()
            {
                if (currentIndex < buttonPool.Count)
                {
                    GameObject button = buttonPool[currentIndex];
                    if (button == null)
                    {
                        button = CreateNewButton();
                        buttonPool[currentIndex] = button;
                    }

                    var existingText = button.transform.Find("PlayerText");
                    if (existingText != null)
                        Destroy(existingText.gameObject);

                    button.SetActive(true);
                    currentIndex++;
                    return button;
                }
                else
                {
                    GameObject newButton = CreateNewButton();
                    buttonPool.Add(newButton);
                    currentIndex++;
                    return newButton;
                }
            }

            public static void ResetPool()
            {
                currentIndex = 0;
                foreach (GameObject obj in buttonPool)
                {
                    if (obj != null)
                    {
                        var textObj = obj.transform.Find("PlayerText");
                        if (textObj != null)
                            Destroy(textObj.gameObject);

                        obj.SetActive(false);
                        obj.transform.parent = null;
                    }
                }
            }

            private static GameObject CreateNewButton()
            {
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                button.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                return button;
            }
        }

        public static class TextPool
        {
            private static List<GameObject> textPool = new List<GameObject>();
            private static int currentIndex = 0;

            public static GameObject GetTextObject()
            {
                if (currentIndex < textPool.Count)
                {
                    GameObject textObj = textPool[currentIndex];
                    if (textObj == null)
                    {
                        textObj = CreateNewTextObject();
                        textPool[currentIndex] = textObj;
                    }
                    textObj.SetActive(true);
                    currentIndex++;
                    return textObj;
                }
                else
                {
                    GameObject newTextObj = CreateNewTextObject();
                    textPool.Add(newTextObj);
                    currentIndex++;
                    return newTextObj;
                }
            }

            public static void ResetPool()
            {
                currentIndex = 0;
                foreach (GameObject textObj in textPool)
                {
                    if (textObj != null)
                    {
                        textObj.SetActive(false);
                    }
                }
            }

            private static GameObject CreateNewTextObject()
            {
                GameObject textObj = new GameObject();
                Text text = textObj.AddComponent<Text>();
                text.fontStyle = FontStyle.Normal;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                return textObj;
            }
        }

        #endregion
    }
}