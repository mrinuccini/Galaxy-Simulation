using UnityEngine;

[CreateAssetMenu(fileName = "New Simulation Data", menuName = "Simulation/Simulation Data")]
public class SimulationData : ScriptableObject
{
	public GameObject starPrefab;

	[Space]
	[Header("Galaxy Settings")]
	public int starAmount;
	public int galaxyRadius;
	public int galaxyThickness;
	public float initialStarsSpeedMax;
	public float initialStarsSpeedMin;
	[Tooltip("The percentage of star that has the initial speed")] public float speedProportion;
	public float blackHoleMass;

	[Space]
	[Header("Stars Materials<s")]
	public Material[] possibleMaterials;
	public float blending = 10f;

	[Space]
	[Header("GPU Instancing")]
	public ComputeShader starCompute;
	public float smoothingLength = 5f;
	[Range(0f, 1f)] public float interactionPercentage = 0.05f;
	public float timeStep = 0.001f;
}
