using System.Collections.Generic;
using UnityEngine;

public class ArduinoBoardManager : MonoBehaviour
{
    [Header("Board Slots")]
    public List<BoardSlot> slots = new List<BoardSlot>();

    [Header("Code State")]
    public int codeState = 0;

    [Header("Circuit State")]
    public bool circuitStarted = false;

    [Header("Alarm Sequence")]
    public AlarmSequenceManager alarmSequenceManager;

    private readonly ItemType[] placementPriority =
    {
        ItemType.Arduino,
        ItemType.Button,
        ItemType.Relay,
        ItemType.Switch,
        ItemType.Multiplexer,
        ItemType.Wires
    };

    public void InteractWithBoard(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

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

                Debug.Log("Componenta montata pe board: " + item);
                return;
            }
        }

        Debug.Log("Nu ai componente valide de montat pe board.");
    }

    public void ReceiveCodeFromRobot(int newCodeState)
    {
        codeState = newCodeState;
        Debug.Log("Board-ul a primit cod. Code state = " + codeState);
    }

    public bool HasAllComponentsPlaced()
    {
        return IsPlaced(ItemType.Arduino)
            && IsPlaced(ItemType.Button)
            && IsPlaced(ItemType.Relay)
            && IsPlaced(ItemType.Switch)
            && IsPlaced(ItemType.Multiplexer)
            && IsPlaced(ItemType.Wires);
    }

    public bool CanStartCircuit()
    {
        return codeState != 0 && HasAllComponentsPlaced() && !circuitStarted;
    }

    public void StartCircuit()
    {
        if (circuitStarted)
            return;

        if (!CanStartCircuit())
        {
            Debug.Log("Circuitul nu poate porni. Lipseste codul sau componentele.");
            return;
        }

        circuitStarted = true;

        Debug.Log("Circuit pornit! Code state: " + codeState);

        if (alarmSequenceManager != null)
        {
            alarmSequenceManager.StartAlarmSequence();
        }
        else
        {
            Debug.LogWarning("AlarmSequenceManager nu este conectat la ArduinoBoardManager.");
        }
    }

    public string GetInteractionMessage(PlayerInventory inventory)
    {
        if (CanStartCircuit())
        {
            return "Apasa E ca sa pornesti alarma";
        }

        if (inventory == null)
        {
            return "Inventarul nu este gasit";
        }

        foreach (ItemType item in placementPriority)
        {
            if (inventory.HasItem(item) && !IsPlaced(item))
            {
                return "Apasa E ca sa plasezi: " + item;
            }
        }

        if (codeState == 0)
        {
            return "Board-ul are nevoie de cod";
        }

        if (!HasAllComponentsPlaced())
        {
            return "Board-ul are nevoie de toate componentele";
        }

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
            if (slot.itemType == item)
                return slot;
        }

        return null;
    }
}