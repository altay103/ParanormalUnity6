using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterEnvanter : MonoBehaviour
{
    public static CharacterEnvanter Instance;

    [Header("El Pozisyonları")] [HideInInspector]
    public Transform currentHand;

    [SerializeField] private GameObject notHand;
    [SerializeField] private GameObject keyHand;
    [SerializeField] private GameObject fenerHand;
    [SerializeField] private GameObject sebekeHand;
    [SerializeField] private GameObject cekicHand;
    [SerializeField] private GameObject benzinHand;

    [Header("Animasyonlar")] [SerializeField]
    private Animator FenerAnim;

    [SerializeField] private Animator NotAnim;
    [SerializeField] private Animator KeyAnim;
    [SerializeField] private Animator SebekeAnim;
    [SerializeField] private Animator CekicAnim;
    [SerializeField] private Animator BenzinAnim;

    [Header("Sistemler")] [SerializeField] private List<NotSystem> notSystems;
    [SerializeField] private List<KeySystem> keySystems;
    [SerializeField] private List<FenerSystem> fenerSystems;
    [SerializeField] private List<SebekeSystem> sebekeSystems;
    [SerializeField] private List<CekicSystem> cekicSystems;
    [SerializeField] private List<BenzinSystem> benzinSystems;

    [Header("Envanter Listeleri")] public List<GameObject> Fener = new();
    public List<GameObject> Notlar = new();
    public List<GameObject> Anahtarlar = new();
    public List<GameObject> Sebekeler = new();
    public List<GameObject> Cekicler = new();
    public List<GameObject> Benzinler = new();

    [Header("Envanterdeki Aktif Öğe")] public GameObject CurrentItem => aktifItem;

    [Header("UI Slot Görselleri")] [SerializeField]
    private Image[] slotImages = new Image[5];

    [SerializeField] private TextMeshProUGUI[] slotCounts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] slotNames = new TextMeshProUGUI[5];
    [SerializeField] private Sprite fenerIcon;
    [SerializeField] private Sprite notIcon;
    [SerializeField] private Sprite keyIcon;
    [SerializeField] private Sprite sebekeIcon;
    [SerializeField] private Sprite cekicIcon;
    [SerializeField] private Sprite benzinIcon;

    public List<string> OwnedKeyIDs = new();

    private GameObject aktifItem;
    private Dictionary<GameObject, int> itemSlotMap = new();
    private HashSet<int> usedSlots = new();
    private int aktifNotIndex = 0;
    public int aktifSebekeIndex = 0;

    private GameObject recentlyDroppedItem = null;

    private Coroutine currentSwitchCoroutine;

    public bool isSwitchingItem = false;

    private void Awake() => Instance = this;

    private void Start() => DeactivateAllHands();

    private void Update()
    {
        Debug.Log(CurrentItem);

        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                if (!IsActiveHandReady() || isSwitchingItem) return;

                GameObject targetItem = null;

                foreach (var pair in itemSlotMap)
                {
                    if (pair.Value == i)
                    {
                        if (pair.Key.CompareTag("Not") && Notlar.Count > 0)
                        {
                            if (Notlar.Count == 1)
                            {
                                targetItem = Notlar[0];
                            }
                            else
                            {
                                aktifNotIndex = (aktifNotIndex + 1) % Notlar.Count;
                                targetItem = Notlar[aktifNotIndex];
                            }
                        }
                        else if (pair.Key.CompareTag("Sebeke") && Sebekeler.Count > 0)
                        {
                            if (Sebekeler.Count == 1)
                            {
                                targetItem = Sebekeler[0];
                            }
                            else
                            {
                                aktifSebekeIndex = (aktifSebekeIndex + 1) % Sebekeler.Count;
                                targetItem = Sebekeler[aktifSebekeIndex];
                            }
                        }
                        else
                        {
                            targetItem = pair.Key;
                        }

                        break;
                    }
                }

                if (targetItem != null && targetItem != aktifItem)
                {
                    if (currentSwitchCoroutine != null)
                        StopCoroutine(currentSwitchCoroutine);

                    currentSwitchCoroutine = StartCoroutine(SwitchToItem(targetItem));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G)) ItemBirak();
    }

    public bool EnvantereEkle(GameObject item)
    {
        if (item == null) return false;

        if (item.CompareTag("Fener"))
        {
            if (!Fener.Contains(item)) Fener.Add(item);

            int slot = GetNextFreeSlot();
            if (slot == -1) return false;

            itemSlotMap[item] = slot;
            usedSlots.Add(slot);
            UpdateSlotUI(item, slot);
            StartCoroutine(SwitchToItem(item));
            return true;
        }

        if (item.CompareTag("Not"))
        {
            if (!Notlar.Contains(item)) Notlar.Add(item);

            GameObject anaNot = Notlar[0];

            if (!itemSlotMap.ContainsKey(anaNot))
            {
                int slot = GetNextFreeSlot();
                if (slot == -1) return false;

                itemSlotMap[anaNot] = slot;
                usedSlots.Add(slot);
            }

            int notSlot = itemSlotMap[anaNot];
            UpdateSlotUI(anaNot, notSlot);
            aktifNotIndex = Notlar.Count - 1;
            StartCoroutine(SwitchToItem(item));
            return true;
        }

        if (item.CompareTag("Sebeke"))
        {
            if (!Sebekeler.Contains(item)) Sebekeler.Add(item);

            GameObject anaSebeke = Sebekeler[0];

            if (!itemSlotMap.ContainsKey(anaSebeke))
            {
                int slot = GetNextFreeSlot();
                if (slot == -1) return false;

                itemSlotMap[anaSebeke] = slot;
                usedSlots.Add(slot);
            }

            int sebekeSlot = itemSlotMap[anaSebeke];
            UpdateSlotUI(anaSebeke, sebekeSlot);
            aktifSebekeIndex = Sebekeler.Count - 1;
            StartCoroutine(SwitchToItem(item));
            return true;
        }

        if (item.CompareTag("Key"))
        {
            if (!Anahtarlar.Contains(item)) Anahtarlar.Add(item);

            var keySystem = item.GetComponent<KeySystem>();
            if (keySystem && !OwnedKeyIDs.Contains(keySystem.keyID))
                OwnedKeyIDs.Add(keySystem.keyID);

            int slot = GetNextFreeSlot();
            if (slot == -1) return false;

            itemSlotMap[item] = slot;
            usedSlots.Add(slot);
            UpdateSlotUI(item, slot);
            StartCoroutine(SwitchToItem(item));
            return true;
        }

        if (item.CompareTag("Cekic"))
        {
            if (!Cekicler.Contains(item)) Cekicler.Add(item);

            int slot = GetNextFreeSlot();
            if (slot == -1) return false;

            itemSlotMap[item] = slot;
            usedSlots.Add(slot);
            UpdateSlotUI(item, slot);
            StartCoroutine(SwitchToItem(item));
            return true;
        }

        if (item.CompareTag("Benzin"))
        {
            if (!Benzinler.Contains(item)) Benzinler.Add(item);

            int slot = GetNextFreeSlot();
            if (slot == -1) return false;

            itemSlotMap[item] = slot;
            usedSlots.Add(slot);
            UpdateSlotUI(item, slot);
            StartCoroutine(SwitchToItem(item));
            return true;
        }

        return false;
    }

    private void UpdateSlotUI(GameObject item, int slot)
    {
        if (slot < 1 || slot > 5) return;

        Sprite icon = null;
        string countText = "";
        string nameText = "";

        var info = item.GetComponent<ItemInfo>();
        if (info != null)
            nameText = info.GetItemName();

        if (item.CompareTag("Fener")) icon = fenerIcon;
        else if (item.CompareTag("Not"))
        {
            icon = notIcon;
            countText = Notlar.Count.ToString();
        }
        else if (item.CompareTag("Sebeke"))
        {
            icon = sebekeIcon;
            countText = Sebekeler.Count.ToString();
        }
        else if (item.CompareTag("Key")) icon = keyIcon;
        else if (item.CompareTag("Cekic")) icon = cekicIcon;
        else if (item.CompareTag("Benzin")) icon = benzinIcon;

        if (slotImages[slot - 1] != null)
        {
            slotImages[slot - 1].sprite = icon;
            slotImages[slot - 1].color = icon != null ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 135f / 255f);
        }

        if (slotCounts.Length >= slot && slotCounts[slot - 1] != null)
            slotCounts[slot - 1].text = countText;

        if (slotNames.Length >= slot && slotNames[slot - 1] != null)
            slotNames[slot - 1].text = nameText;
    }

    private int GetNextFreeSlot()
    {
        for (int slot = 1; slot <= 5; slot++)
            if (!usedSlots.Contains(slot))
                return slot;
        return -1;
    }

    public void ItemCikar(GameObject item)
    {
        if (item == null) return;

        if (aktifItem == item)
        {
            FenerAnim?.SetTrigger("down");
            NotAnim?.SetTrigger("down");
            KeyAnim?.SetTrigger("down");
            SebekeAnim?.SetTrigger("down");
            CekicAnim?.SetTrigger("down");
            BenzinAnim?.SetTrigger("down");
            aktifItem = null;
            DeactivateAllHands();
        }

        void TemizleSlot(int slot)
        {
            if (slotImages[slot - 1] != null)
            {
                slotImages[slot - 1].sprite = null;
                slotImages[slot - 1].color = new Color(0f, 0f, 0f, 135f / 255f);
            }

            if (slotCounts.Length >= slot && slotCounts[slot - 1] != null)
                slotCounts[slot - 1].text = "";

            if (slotNames.Length >= slot && slotNames[slot - 1] != null)
                slotNames[slot - 1].text = "";
        }

        if (item.CompareTag("Not"))
        {
            Notlar.Remove(item);
            if (Notlar.Count == 0)
            {
                TemizleSlotByTag("Not");
            }
            else
            {
                GameObject yeniAnaNot = Notlar[0];
                int eskiSlot = -1;

                foreach (var kvp in new Dictionary<GameObject, int>(itemSlotMap))
                {
                    if (kvp.Key.CompareTag("Not"))
                    {
                        eskiSlot = kvp.Value;
                        itemSlotMap.Remove(kvp.Key);
                    }
                }

                if (!itemSlotMap.ContainsKey(yeniAnaNot))
                {
                    itemSlotMap[yeniAnaNot] = eskiSlot != -1 ? eskiSlot : GetNextFreeSlot();
                }

                UpdateSlotUI(yeniAnaNot, itemSlotMap[yeniAnaNot]);
            }
        }
        else if (item.CompareTag("Sebeke"))
        {
            Sebekeler.Remove(item);
            if (Sebekeler.Count == 0)
            {
                TemizleSlotByTag("Sebeke");
            }
            else
            {
                GameObject yeniAnaSebeke = Sebekeler[0];
                int eskiSlot = -1;

                foreach (var kvp in new Dictionary<GameObject, int>(itemSlotMap))
                {
                    if (kvp.Key.CompareTag("Sebeke"))
                    {
                        eskiSlot = kvp.Value;
                        itemSlotMap.Remove(kvp.Key);
                    }
                }

                if (!itemSlotMap.ContainsKey(yeniAnaSebeke))
                {
                    itemSlotMap[yeniAnaSebeke] = eskiSlot != -1 ? eskiSlot : GetNextFreeSlot();
                }

                UpdateSlotUI(yeniAnaSebeke, itemSlotMap[yeniAnaSebeke]);
            }
        }
        else if (item.CompareTag("Fener"))
        {
            Fener.Remove(item);
            if (itemSlotMap.TryGetValue(item, out int slot))
            {
                itemSlotMap.Remove(item);
                usedSlots.Remove(slot);
                TemizleSlot(slot);
            }
        }
        else if (item.CompareTag("Key"))
        {
            Anahtarlar.Remove(item);
            if (itemSlotMap.TryGetValue(item, out int slot))
            {
                itemSlotMap.Remove(item);
                usedSlots.Remove(slot);
                TemizleSlot(slot);
            }
        }
        else if (item.CompareTag("Cekic"))
        {
            Cekicler.Remove(item);
            if (itemSlotMap.TryGetValue(item, out int slot))
            {
                itemSlotMap.Remove(item);
                usedSlots.Remove(slot);
                TemizleSlot(slot);
            }
        }
        else if (item.CompareTag("Benzin"))
        {
            Benzinler.Remove(item);
            if (itemSlotMap.TryGetValue(item, out int slot))
            {
                itemSlotMap.Remove(item);
                usedSlots.Remove(slot);
                TemizleSlot(slot);
            }
        }
    }

    private void ItemBirak()
    {
        if (aktifItem == null) return;

        if (aktifItem.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        aktifItem.transform.SetParent(null);
        aktifItem.layer = LayerMask.NameToLayer("Interactible");
        recentlyDroppedItem = aktifItem;

        FenerAnim?.SetTrigger("down");
        NotAnim?.SetTrigger("down");
        KeyAnim?.SetTrigger("down");
        SebekeAnim?.SetTrigger("down");
        CekicAnim?.SetTrigger("down");
        BenzinAnim?.SetTrigger("down");

        ItemCikar(aktifItem);

        aktifItem = null;
        DeactivateAllHands();
        StartCoroutine(OtomatikGeciseZorla());
    }

    public void TemizleSlotByTag(string tag)
    {
        GameObject itemToRemove = null;
        int slot = -1;

        foreach (var kvp in itemSlotMap)
        {
            if (kvp.Key != null && kvp.Key.CompareTag(tag))
            {
                itemToRemove = kvp.Key;
                slot = kvp.Value;
                break;
            }
        }

        if (itemToRemove != null)
        {
            itemSlotMap.Remove(itemToRemove);
            usedSlots.Remove(slot);

            if (slotImages[slot - 1] != null)
            {
                slotImages[slot - 1].sprite = null;
                slotImages[slot - 1].color = new Color(0f, 0f, 0f, 135f / 255f);
            }

            if (slotCounts.Length >= slot && slotCounts[slot - 1] != null)
                slotCounts[slot - 1].text = "";

            if (slotNames.Length >= slot && slotNames[slot - 1] != null)
                slotNames[slot - 1].text = "";

            Debug.Log("TemizleSlotByTag: Slot temizlendi -> Tag: " + tag + ", Slot: " + slot);
        }
    }

    public IEnumerator SwitchToItem(GameObject item)
    {
        isSwitchingItem = true;

        ClearAllSlotTexts();

        if (aktifItem == item)
        {
            if (item != null && !item.activeSelf)
                item.SetActive(true);

            isSwitchingItem = false;
            yield break;
        }

        if (aktifItem != null)
        {
            int previousSlot = -1;

            if (aktifItem.CompareTag("Not") && Notlar.Count > 0)
                previousSlot = itemSlotMap[Notlar[0]];
            else if (aktifItem.CompareTag("Sebeke") && Sebekeler.Count > 0)
                previousSlot = itemSlotMap[Sebekeler[0]];
            else
                itemSlotMap.TryGetValue(aktifItem, out previousSlot);

            if (previousSlot != -1)
            {
                if (slotNames.Length >= previousSlot && slotNames[previousSlot - 1] != null)
                    slotNames[previousSlot - 1].text = "";

                if (slotCounts.Length >= previousSlot && slotCounts[previousSlot - 1] != null)
                {
                    if (aktifItem.CompareTag("Not") || aktifItem.CompareTag("Sebeke"))
                        slotCounts[previousSlot - 1].text = "";
                }
            }
        }

        FenerAnim?.SetTrigger("down");
        NotAnim?.SetTrigger("down");
        KeyAnim?.SetTrigger("down");
        SebekeAnim?.SetTrigger("down");
        CekicAnim?.SetTrigger("down");
        BenzinAnim?.SetTrigger("down");

        yield return new WaitForSeconds(0.4f);

        if (aktifItem != null && aktifItem != item && aktifItem != recentlyDroppedItem)
            aktifItem.SetActive(false);

        DeactivateAllHands();
        aktifItem = item;

        int slot = -1;
        if (item.CompareTag("Not") && Notlar.Count > 0)
            slot = itemSlotMap[Notlar[0]];
        else if (item.CompareTag("Sebeke") && Sebekeler.Count > 0)
            slot = itemSlotMap[Sebekeler[0]];
        else
            itemSlotMap.TryGetValue(item, out slot);

        if (slot != -1)
            UpdateSlotUI(item, slot); // SLOT ADI ARTIK GEÇ GELMİYOR

        if (item == null)
        {
            isSwitchingItem = false;
            yield break;
        }

        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Animator anim = null;

        if (item.CompareTag("Not"))
        {
            notHand?.SetActive(true);
            currentHand = notHand.transform;

            foreach (var system in notSystems)
                if (system && system.gameObject == item)
                    system.NotYerlestir(item);

            anim = NotAnim;
        }
        else if (item.CompareTag("Key"))
        {
            keyHand?.SetActive(true);
            currentHand = keyHand.transform;

            foreach (var system in keySystems)
                if (system && system.gameObject == item)
                    system.KeyYerlestir(item);

            anim = KeyAnim;
        }
        else if (item.CompareTag("Fener"))
        {
            fenerHand?.SetActive(true);
            currentHand = fenerHand.transform;

            foreach (var system in fenerSystems)
                if (system && system.gameObject == item)
                    system.FenerYerlestir(item);

            anim = FenerAnim;
        }
        else if (item.CompareTag("Sebeke"))
        {
            sebekeHand?.SetActive(true);
            currentHand = sebekeHand.transform;

            foreach (var system in sebekeSystems)
                if (system && system.gameObject == item)
                    system.SebekeYerlestir(item);

            anim = SebekeAnim;
        }
        else if (item.CompareTag("Cekic"))
        {
            cekicHand?.SetActive(true);
            currentHand = cekicHand.transform;

            foreach (var system in cekicSystems)
                if (system && system.gameObject == item)
                    system.CekicYerlestir(item);

            anim = CekicAnim;
        }
        else if (item.CompareTag("Benzin"))
        {
            benzinHand?.SetActive(true);
            currentHand = benzinHand.transform;

            foreach (var system in benzinSystems)
                if (system && system.gameObject == item)
                    system.BenzinYerlestir(item);

            anim = BenzinAnim;
        }

        if (anim != null)
        {
            anim.SetTrigger("up");
            yield return new WaitForSeconds(0.25f);
            anim.SetTrigger("idle");
        }

        recentlyDroppedItem = null;
        item.SetActive(true);

        currentSwitchCoroutine = null;
        isSwitchingItem = false;
    }

    public IEnumerator SwitchToItemSilently(GameObject item)
    {
        if (aktifItem != null && aktifItem.CompareTag("Sebeke"))
            SebekeAnim?.SetTrigger("down");

        if (aktifItem != null)
            aktifItem.SetActive(false);

        aktifItem = item;
        yield return null;
    }

    private void ClearAllSlotTexts()
    {
        for (int i = 0; i < slotNames.Length; i++)
        {
            if (slotNames[i] != null) slotNames[i].text = "";
            if (slotCounts[i] != null) slotCounts[i].text = "";
        }
    }

    private bool IsActiveHandReady()
    {
        if (aktifItem == null) return true;

        Animator currentAnimator = null;

        if (aktifItem.CompareTag("Fener")) currentAnimator = FenerAnim;
        else if (aktifItem.CompareTag("Not")) currentAnimator = NotAnim;
        else if (aktifItem.CompareTag("Key")) currentAnimator = KeyAnim;
        else if (aktifItem.CompareTag("Sebeke")) currentAnimator = SebekeAnim;
        else if (aktifItem.CompareTag("Cekic")) currentAnimator = CekicAnim;
        else if (aktifItem.CompareTag("Benzin")) currentAnimator = BenzinAnim;

        if (currentAnimator == null) return true;

        AnimatorStateInfo state = currentAnimator.GetCurrentAnimatorStateInfo(0);

        if ((state.IsName("up") || state.IsTag("Up")) && state.normalizedTime < 1f)
        {
            return false;
        }

        if (currentAnimator.IsInTransition(0))
        {
            return false;
        }

        return true;
    }

    private void DeactivateAllHands()
    {
        notHand?.SetActive(false);
        keyHand?.SetActive(false);
        fenerHand?.SetActive(false);
        sebekeHand?.SetActive(false);
        cekicHand?.SetActive(false);
        benzinHand?.SetActive(false);
    }

    public IEnumerator OtomatikGeciseZorla()
    {
        yield return new WaitForSeconds(0.2f);

        if (Sebekeler.Count > 0)
            yield return SwitchToItem(Sebekeler[Mathf.Clamp(aktifSebekeIndex, 0, Sebekeler.Count - 1)]);
        else if (Notlar.Count > 0)
            yield return SwitchToItem(Notlar[Mathf.Clamp(aktifNotIndex, 0, Notlar.Count - 1)]);
        else if (Anahtarlar.Count > 0)
            yield return SwitchToItem(Anahtarlar[0]);
        else if (Fener.Count > 0)
            yield return SwitchToItem(Fener[0]);
        else if (Cekicler.Count > 0)
            yield return SwitchToItem(Cekicler[0]);
        else if (Benzinler.Count > 0)
            yield return SwitchToItem(Benzinler[0]);
        else
            DeactivateAllHands();
    }
}