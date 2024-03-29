﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class StandingAI : AI
{
	// Private variables
	private GameObject trackedObject;
	private Vector3 lastPlayerPosition;
	private NavMeshAgent navMeshAgent;
	private float reactionTimer;
	private float trackingTimer;
	private bool trackingStarted = false;
	private bool isTracking = false;
	private Vector3 startPosition;

	void Start()
	{
		startPosition = this.transform.position;
		this.GetComponent<SphereCollider>().radius = awareness;
		navMeshAgent = this.GetComponent<NavMeshAgent>();
		warningText.SetActive(false);
	}

	void Update()
	{
		// Deals with the reaction time before the track starts
		if (!isTracking && trackingStarted && reactionTimer - Time.realtimeSinceStartup < 0f)
			StartTracking();

		// Check if the player has loose or not, and if he is hiding or not
		if (isTracking || trackingStarted)
		{
			trackingTimer -= Time.deltaTime;
			if (trackingTimer <= 0f)
				GameManager.Instance.GameOver();

			if (trackedObject.GetComponent<ThirdPersonUserControl>().isHiding)
				GoBackToStartingPoint();
		}
	}

	private void StartTracking()
	{
		navMeshAgent.speed = movingSpeed;
		navMeshAgent.acceleration = acceleration;
		navMeshAgent.destination = lastPlayerPosition;
		isTracking = true;
	}

	private void GoBackToStartingPoint()
	{
		navMeshAgent.speed = movingSpeed;
		navMeshAgent.destination = startPosition;
		isTracking = false;
		trackingStarted = false;
		trackingTimer = 1000f;
		trackedObject = null;
		warningText.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		// If the player is seen by the AI, it will start moving to the last position of the player (after reactionTime seconds)
		// The player will loose if trackingTimer is <= 0
		if (!trackingStarted && other.gameObject.CompareTag("Player"))
		{
			lastPlayerPosition = other.gameObject.transform.position;
			reactionTimer = Time.realtimeSinceStartup + reactionTime;
			trackingTimer = timeBeforeLoose;
			trackingStarted = true;
			trackedObject = other.gameObject;
			warningText.SetActive(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		// If the player escape in time, the AI will just return to his starting point
		if (other.gameObject.CompareTag("Player"))
			GoBackToStartingPoint();
	}
}