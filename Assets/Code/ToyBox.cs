using System.Collections.Generic;
using UnityEngine;

public class ToyBox : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private LegPartButton[] availablePartButtons;
    [SerializeField] private GameObject PartParent;
    [SerializeField] private GameObject SpawnSpot;

    [Header("Listener Events")]
    [SerializeField] private LegPartEventChannel onLegPartGrabbedByPlayer;

    private Dictionary<LegPart.LegTypes, bool> _legPartsGrabbed = new Dictionary<LegPart.LegTypes, bool>();


    private void OnEnable()
    {
        HideParts();

        onLegPartGrabbedByPlayer.OnEventTriggered += AddPartToBox;
    }

    private void OnDisable()
    {
        onLegPartGrabbedByPlayer.OnEventTriggered -= AddPartToBox;
    }
    private void AddPartToBox(LegPart part)
    {
        if (!_legPartsGrabbed.ContainsKey(part.type))
        {
            _legPartsGrabbed.Add(part.type, true);
        }
    }

    public void ShowParts()
    {
        foreach (LegPartButton partButton in availablePartButtons)
        {
            if (_legPartsGrabbed.ContainsKey(partButton.AssociatedLegPart.type))
                partButton.gameObject.SetActive(true);
            else 
                partButton.gameObject.SetActive(false);
        }
    }

    public void HideParts()
    {
        foreach (LegPartButton partButton in availablePartButtons)
        {
            partButton.gameObject.SetActive(false);
        }
    }

    public void InstantiatePart(LegPart part)
    {
        Instantiate(part, SpawnSpot.transform.position, SpawnSpot.transform.rotation, PartParent.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowParts();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideParts();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
