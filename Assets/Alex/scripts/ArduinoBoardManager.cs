using System.Collections.Generic;
using UnityEngine;

public class ArduinoBoardManager : MonoBehaviour
{
    [Header("Board Slots")]
    public List<BoardSlot> slots = new List<BoardSlot>();

    [Header("Circuit State")]
    public bool circuitStarted = false;

    [Header("Alarm Sequence")]
    public AlarmSequenceManager alarmSequenceManager;

    [Header("Visual Debug - Placed Components")]
    public bool arduinoPlaced;
    public bool buttonPlaced;
    public bool relayPlaced;
    public bool switchPlaced;
    public bool multiplexerPlaced;
    public bool wiresPlaced;
    public bool codePlaced;
    public bool allComponentsPlaced;
    public bool allReadyToStart;

    private readonly ItemType[] placementPriority =
    {
        ItemType.Arduino,
        ItemType.Button,
        ItemType.Relay,
        ItemType.Switch,
        ItemType.Multiplexer,
        ItemType.Wires,
        ItemType.Code
    };

    private void Update()
    {
        UpdateVisualDebug();
    }

    public void InteractWithBoard(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        if (circuitStarted)
        {
            Debug.Log("Circuitul este deja pornit.");
            return;
        }

        if (CanStartCircuit())
        {
            IBoardCommand startCommand = new StartCircuitCommand(this);
            startCommand.Execute();
            return;
        }

        TryPlaceNextComponent(inventory);
    }

    private void TryPlaceNextComponent(PlayerInventory inventory)
    {
        foreach (ItemType item in placementPriority)
        {
            if (inventory.HasItem(item) && !IsPlaced(item))
            {
                BoardSlot slot = GetSlot(item);

                if (slot == null)
                {
                    Debug.LogWarning("Nu exista slot pe board pentru: " + item);
                    return;
                }

                IBoardCommand placeCommand = new PlaceComponentCommand(slot, inventory);
                placeCommand.Execute();

                UpdateVisualDebug();
                return;
            }
        }

        DebugMissingRequirements();
    }

    public bool HasAllComponentsPlaced()
    {
        foreach (ItemType item in placementPriority)
        {
            if (!IsPlaced(item))
                return false;
        }

        return true;
    }

    public bool CanStartCircuit()
    {
        UpdateVisualDebug();
        return allComponentsPlaced && !circuitStarted;
    }

    public void StartCircuit()
    {
        if (circuitStarted)
            return;

        if (!CanStartCircuit())
        {
            Debug.LogWarning("Circuitul NU poate porni.");
            DebugMissingRequirements();
            return;
        }

        circuitStarted = true;
        UpdateVisualDebug();

        Debug.Log("START CIRCUIT");

        if (alarmSequenceManager != null)
        {
            Debug.Log("START ALARM SEQUENCE");
            alarmSequenceManager.StartAlarmSequence();
        }
        else
        {
            Debug.LogError("AlarmSequenceManager este NULL pe ArduinoBoardManager.");
        }
    }

    public string GetInteractionMessage(PlayerInventory inventory)
    {
        UpdateVisualDebug();

        if (circuitStarted)
            return "Circuitul este deja pornit";

        if (CanStartCircuit())
            return "Apasa E ca sa pornesti alarma";

        if (inventory == null)
            return "Inventarul nu este gasit";

        foreach (ItemType item in placementPriority)
        {
            if (inventory.HasItem(item) && !IsPlaced(item))
                return "Apasa E ca sa plasezi: " + item;
        }

        ItemType? missingItem = GetFirstMissingComponent();

        if (missingItem.HasValue)
            return "Lipseste componenta: " + missingItem.Value;

        return "Circuitul nu poate fi pornit";
    }

    public bool IsPlaced(ItemType item)
    {
        BoardSlot slot = GetSlot(item);
        return slot != null && slot.isPlaced;
    }

    public BoardSlot GetSlot(ItemType item)
    {
        foreach (BoardSlot slot in slots)
        {
            if (slot != null && slot.itemType == item)
                return slot;
        }

        return null;
    }

    private ItemType? GetFirstMissingComponent()
    {
        foreach (ItemType item in placementPriority)
        {
            if (!IsPlaced(item))
                return item;
        }

        return null;
    }

    private void UpdateVisualDebug()
    {
        arduinoPlaced = IsPlaced(ItemType.Arduino);
        buttonPlaced = IsPlaced(ItemType.Button);
        relayPlaced = IsPlaced(ItemType.Relay);
        switchPlaced = IsPlaced(ItemType.Switch);
        multiplexerPlaced = IsPlaced(ItemType.Multiplexer);
        wiresPlaced = IsPlaced(ItemType.Wires);
        codePlaced = IsPlaced(ItemType.Code);

        allComponentsPlaced =
            arduinoPlaced &&
            buttonPlaced &&
            relayPlaced &&
            switchPlaced &&
            multiplexerPlaced &&
            wiresPlaced &&
            codePlaced;

        allReadyToStart = allComponentsPlaced && !circuitStarted;
    }

    private void DebugMissingRequirements()
    {
        UpdateVisualDebug();

        Debug.Log("===== BOARD DEBUG =====");
        Debug.Log("Arduino = " + arduinoPlaced);
        Debug.Log("Button = " + buttonPlaced);
        Debug.Log("Relay = " + relayPlaced);
        Debug.Log("Switch = " + switchPlaced);
        Debug.Log("Multiplexer = " + multiplexerPlaced);
        Debug.Log("Wires = " + wiresPlaced);
        Debug.Log("Code = " + codePlaced);
        Debug.Log("All Components Placed = " + allComponentsPlaced);
        Debug.Log("Circuit Started = " + circuitStarted);
        Debug.Log("All Ready To Start = " + allReadyToStart);
        Debug.Log("=======================");
    }
}