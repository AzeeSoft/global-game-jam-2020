﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI interactionInfoText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePickableInfo();
    }

    void UpdatePickableInfo()
    {
        var playerPickController = PlayerModel.Instance.playerInteractionController;

        if (playerPickController.isHoldingPickable && playerPickController.pickableOnHand.hasObjectPlacePointInRadius)
        {
            interactionInfoText.text = playerPickController.pickableOnHand.placeInstruction;
        }
        else if (playerPickController.isFocusedOnPickable)
        {
            interactionInfoText.text = playerPickController.pickableOnFocus.pickUpInstruction;
        }
        else if (playerPickController.isFocusedOnFixableObject)
        {
            interactionInfoText.text = playerPickController.fixableObjectOnFocus.fixInstruction;
        }
        else
        {
            interactionInfoText.text = "";
        }
    }
}