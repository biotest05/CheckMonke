using System;
using BepInEx;
using UnityEngine;
using Utilla;
using UnityEngine.XR;
using UnityEngine.InputSystem.Layouts;

namespace CheckMonke.Plugin
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[ModdedGamemode]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(CheckMonke.Plugin.PluginInfo.GUID, CheckMonke.Plugin.PluginInfo.Name, CheckMonke.Plugin.PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		bool inRoom;
		bool isModOn;
		bool isHoldingTrigger;
		bool isHoldingPrimaryButton;
		Vector3 checkPointPosition;
		bool isHoldingGrip;
		GameObject checkPoint;

		void Start()
		{
			/* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

			Utilla.Events.GameInitialized += OnGameInitialized;
		}

		void OnEnable()
		{
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();
		}

		void OnDisable()
		{
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			HarmonyPatches.RemoveHarmonyPatches();
		}

		void OnGameInitialized(object sender, EventArgs e)
		{
			/* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
			Debug.Log("(BIOTEST Mod - CheckMonke) Game Initialized");
			Update();
		}

		void Update()
		{
			if (inRoom)
			{
				isModOn = true;
			} 
			else if (inRoom == false)
			{
				isModOn = false;
			}

			if (isModOn)
			{
				InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out isHoldingTrigger);
				InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out isHoldingGrip);
				InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out isHoldingPrimaryButton);
				if (isHoldingTrigger)
				{
					if (checkPoint != null) Destroy(checkPoint);
					if (checkPoint == null) { 
						checkPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
						checkPoint.transform.position = GorillaLocomotion.Player.Instance.rightHandFollower.position;
						checkPoint.GetComponent<MeshRenderer>().material.color = Color.white;
						checkPoint.GetComponent<BoxCollider>().enabled = false;
						checkPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
						checkPointPosition = checkPoint.transform.position;
					}
				}
				if (isHoldingGrip)
				{
					if (checkPoint != null) { 
						GorillaLocomotion.Player.Instance.transform.position = checkPointPosition;
						checkPoint.GetComponent<MeshRenderer>().material.color = Color.red;
					}
				} 
				if (isHoldingGrip == false)
				{
					if (checkPoint != null) { 
						checkPoint.GetComponent<MeshRenderer>().material.color = Color.white;
					}
				}
				if (isHoldingPrimaryButton)
				{
					if (checkPoint != null) Destroy(checkPoint);
				}
			}
		}

		/* This attribute tells Utilla to call this method when a modded room is joined */
		[ModdedGamemodeJoin]
		public void OnJoin(string gamemode)
		{
			/* Activate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = true;
		}

		/* This attribute tells Utilla to call this method when a modded room is left */
		[ModdedGamemodeLeave]
		public void OnLeave(string gamemode)
		{
			/* Deactivate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = false;
		}
	}
}
