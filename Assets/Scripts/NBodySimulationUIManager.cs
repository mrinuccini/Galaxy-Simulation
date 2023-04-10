using System;
using TMPro;
using UnityEngine;

public class NBodySimulationUIManager : MonoBehaviour
{
    [SerializeField] NBodySimulation nBodySimulation;
	[SerializeField] GameObject container;

	[SerializeField] TMP_Dropdown modeDropDown;

	[Space]
	[Header("Buttons")]
	[SerializeField] TextMeshProUGUI pauseButtonText;

	[Space]
	[Header("Simulation Data UI")]
	[SerializeField] TMP_Dropdown simulationDataDropdown;
	[SerializeField] SimulationData[] simulationDatas;

	[Space]

	[SerializeField] TextMeshProUGUI FPSCounter;

	private void Start()
	{
		/* Add dropdowns event listeners */
		modeDropDown.onValueChanged.AddListener(delegate { OnModeDropDownValueChanged(); });
		simulationDataDropdown.onValueChanged.AddListener(delegate { OnSimulationDataDropdownValueChanged(); });

		/* Initialize te simulation data dropdown */
		simulationDataDropdown.options.Clear();

		foreach (SimulationData simData in simulationDatas) 
		{
			simulationDataDropdown.options.Add(new TMP_Dropdown.OptionData(simData.name));
		}

		simulationDataDropdown.value = simulationDatas.Length - 1;
	}

	private void Update()
	{
		/* Used to disable or enable the UI */
		if (Input.GetKeyDown(KeyCode.F1)) 
		{
			container.SetActive(!container.activeSelf);
		}

		/* Update the FPS Counter */
		FPSCounter.SetText($"FPS : {Math.Round(1 / Time.deltaTime, 2)} ({Math.Round(Time.deltaTime * 1000, 2)}ms). \nPhysics call per frame : {Math.Round(Time.deltaTime / Time.fixedDeltaTime, 2)}");
	}

	/* When the simulation data preset dropdown value changes */
	void OnSimulationDataDropdownValueChanged()
	{
		int index = simulationDataDropdown.value;

		nBodySimulation.simData = simulationDatas[index];
		StartCoroutine(nBodySimulation.RestartSimulation());

		pauseButtonText.SetText(nBodySimulation.simulationRunning ? "Pause" : "Play");
	}

	/* When the simulation type dropdown value changes */
	void OnModeDropDownValueChanged() 
	{
		int index = modeDropDown.value;

		if(index == 0) 
		{
			nBodySimulation.generationMode = GenerationMode.Galaxy;
		}else if(index == 1) 
		{
			nBodySimulation.generationMode = GenerationMode.Collision;
		}

		StartCoroutine(nBodySimulation.RestartSimulation());

		pauseButtonText.SetText(nBodySimulation.simulationRunning ? "Pause" : "Play");
	}

	/* When the restart button is pressed */
	public void RestartSimulationButton() 
	{
		StartCoroutine(nBodySimulation.RestartSimulation());

		pauseButtonText.SetText(nBodySimulation.simulationRunning ? "Pause" : "Play");
	}

	/* When the pause button is pressed */
	public void PauseSimulationButton() 
	{
		nBodySimulation.simulationRunning = !nBodySimulation.simulationRunning;

		pauseButtonText.SetText(nBodySimulation.simulationRunning ? "Pause" : "Play");
	}
}
