using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
struct Star
{
    public Vector3 position;
    public Vector3 speed;
    public int bodyID;

    public Star(Vector3 position, int bodyID) 
    {
        this.position = position;
        this.bodyID = bodyID;
        speed = Vector3.zero;
    }
}

public enum GenerationMode 
{
    Galaxy,
    Collision
}

public class NBodySimulation : MonoBehaviour
{
    [Header("Simulation Data")]
    public SimulationData simData;

    [Space]
    [Header("Galaxy Data")]
	[SerializeField] Transform galaxy1;
    [SerializeField] Transform galaxy2;

    [Space]
    [Header("Collision Mode Data")]
    [SerializeField] Material[] collisionMaterials;
	GameObject starPrefab;

    [Space]
    [Header("Generation Settings")]
    public GenerationMode generationMode;

	int starAmount;
	int galaxyRadius;
    int galaxyThickness;
    float initialStarsSpeedMax;
    float initialStarsSpeedMin;
	[Tooltip("The percentage of star that has the initial speed")] float speedProportion;
    float blackholeMass;

    Material[] possibleMaterials;
    float blending;

    ComputeShader starCompute;
    float smoothingLength = 5f;
	float interactionPercentage = 0.05f;
    float timeStep = 0.001f;

	[Space]
    [Header("Editor Settings")]
	[SerializeField] bool generateOnPlay;
	[SerializeField] bool clearOnGenerate;

    List<CelestialBody> celestialBodies = new List<CelestialBody>();
    Star[] stars;
    public bool simulationRunning = false;

    /* Initializes the simulation */
    void Start()
    {
        if (generateOnPlay) 
        {
            /* Generate the simulation based on the generation mode */
            switch (generationMode) 
            {
                case GenerationMode.Galaxy:
                    CreateGalaxy();
                    break;
                case GenerationMode.Collision:
                    CreateCollision();
                    break;
            }
        }
    }

    /* Use to load the data from the SimulationData scriptable object */
    void LoadSimData() 
    {
        starPrefab = simData.starPrefab;

        starAmount = simData.starAmount;
        galaxyRadius = simData.galaxyRadius;
        galaxyThickness = simData.galaxyThickness;
        initialStarsSpeedMax = simData.initialStarsSpeedMax;
        initialStarsSpeedMin = simData.initialStarsSpeedMin;
		speedProportion = simData.speedProportion;
        blackholeMass = simData.blackHoleMass;

        possibleMaterials = simData.possibleMaterials;
        blending = simData.blending;

        starCompute = simData.starCompute;
        smoothingLength = simData.smoothingLength;
        interactionPercentage = simData.interactionPercentage;
        timeStep = simData.timeStep;
    }

    /* Create a galaxy of a certain radius, thickness and star density (Galaxy Generation Mode) */
    public void CreateGalaxy() 
    {
        // Load the simulation data
        LoadSimData();

        // Initialize our lists
		stars = new Star[starAmount];
        celestialBodies.Clear();

        // Clear the galaxy if needed
		if (clearOnGenerate) ClearGalaxy();

        /* Generate as many stars as we need */
        for(int i = 0; i < starAmount; i++) 
        {
            GameObject newStar = Instantiate(starPrefab);

            // Compute the position of the star
            Vector3 randomSpherePos = Random.insideUnitSphere * galaxyRadius;
            Vector3 starPos = new Vector3(randomSpherePos.x, Random.Range(-galaxyThickness, galaxyThickness), randomSpherePos.z);

            /* Set the parent and position of the sphere */
            newStar.transform.position = starPos;

            newStar.transform.SetParent(galaxy1);
            
            celestialBodies.Add(newStar.GetComponent<CelestialBody>());

            /* Generate the back-end data (for the compute shader) */
			Star newStarData = new Star(starPos, i);

            if(Random.Range(0, 100) < speedProportion)
                newStarData.speed = new Vector3(Random.Range(initialStarsSpeedMin, initialStarsSpeedMax), 0, Random.Range(initialStarsSpeedMin, initialStarsSpeedMax));

            stars[i] = newStarData;

            /* Changes the material based on the distance of the star to the center of the galaxy (little mixing applied) */
			float distanceToCenter = Mathf.Clamp(Vector3.Distance(newStar.transform.position, Vector3.zero) + Random.Range(-blending, blending), 0, galaxyRadius - 1);
            newStar.GetComponent<MeshRenderer>().material = possibleMaterials[(int)(distanceToCenter / galaxyRadius * simData.possibleMaterials.Length)];;
		}
    }

    /* Use to generate two galaxy, each with a different color (Collision Generation Mode) */
    void CreateCollision() 
    {
        // Load the simulation data
		LoadSimData();

        /* Create our lists */
		stars = new Star[starAmount];
		celestialBodies.Clear();

        /* Clear the galaxy if needed */
		if (clearOnGenerate) ClearGalaxy();

        /* Generate as many stars as we need */
		for (int i = 0; i < starAmount; i++)
		{
			GameObject newStar = Instantiate(starPrefab);

			celestialBodies.Add(newStar.GetComponent<CelestialBody>());

			// Compute the position of the star
			Vector3 randomSpherePos = Random.insideUnitSphere * galaxyRadius;
            Vector3 starPos = Vector3.zero;

            /* Give the star a different color and a position offset depending on its galaxy */
			if (i % 2 == 0)
			{
				starPos = new Vector3(randomSpherePos.x, Random.Range(-galaxyThickness, galaxyThickness), randomSpherePos.z + simData.galaxyRadius * 2 + 25);
				starPos = new Vector3(starPos.y, starPos.x, starPos.z);

				newStar.GetComponent<MeshRenderer>().material = collisionMaterials[0];

				newStar.transform.SetParent(galaxy2);
			}
			else
			{
				starPos = new Vector3(randomSpherePos.x, Random.Range(-galaxyThickness, galaxyThickness), randomSpherePos.z);

				newStar.GetComponent<MeshRenderer>().material = collisionMaterials[collisionMaterials.Length - 1];

				newStar.transform.SetParent(galaxy1);
			}

			newStar.transform.position = starPos;

            /* Generate the back-end data (for the computer shader) */
			Star newStarData = new Star(starPos, i);

			if (Random.Range(0, 100) < speedProportion)
				newStarData.speed = new Vector3(Random.Range(initialStarsSpeedMin, initialStarsSpeedMax), 0, Random.Range(initialStarsSpeedMin, initialStarsSpeedMax));
			
            stars[i] = newStarData;
		}
	}

    /* Use to clear the current galaxy */
    public void ClearGalaxy() 
    {
        foreach(Transform children in galaxy1) 
        {
            Destroy(children.gameObject);
        }

		foreach (Transform children in galaxy2)
		{
			Destroy(children.gameObject);
		}
	}

    /* Use to clear the current galaxy (for Editor Scripts) */
    public void ClearGalaxyEditor() 
    {
		foreach (Transform children in galaxy1)
		{
			DestroyImmediate(children.gameObject);
		}

		foreach (Transform children in galaxy2)
		{
			DestroyImmediate(children.gameObject);
		}
	}

    /* Called every frame */
    void Simulation() 
    {
        if (!simulationRunning) return;

        /* Compute the size (in bytes) of a star buffer */
        int vector3Size = sizeof(float) * 3;
        int intSize = sizeof(int);
        int totalSize = vector3Size * 2 + intSize;

        /* Fills our compute buffer and compue shader with data */
        ComputeBuffer starBuffer = new ComputeBuffer(stars.Length, totalSize);
		starBuffer.SetData(stars);

        starCompute.SetBuffer(0, "stars", starBuffer);
        starCompute.SetInt("starAmount", starAmount);
        starCompute.SetFloat("step", timeStep);
		starCompute.SetFloat("smoothingLength", smoothingLength); 
        starCompute.SetFloat("interactionPercentage", interactionPercentage);
		starCompute.SetFloat("blackHoleMass", blackholeMass);

		starCompute.Dispatch(0, stars.Length / 100, 1, 1);

        /* Retrieve the data back from the compute shader */
        starBuffer.GetData(stars);

        for(int i = 0; i < stars.Length; i++) 
        {
            Star star = stars[i];
			CelestialBody body = celestialBodies[i];

            body.speed = star.speed;
            body.transform.position = star.position;
        }

        /* Dispose the compute Shader */
        starBuffer.Dispose();
    }

    /* Run the simulation every frame */
	private void Update()
	{
        Simulation();		
	}
    
    /* Use to restart the simulation */
    public IEnumerator RestartSimulation() 
    {
        /* Wait for the compute shader to finish executing */
        yield return new WaitForEndOfFrame();

        /* Stop and clear the simulation and then restart it */
        simulationRunning = false;

        ClearGalaxy();
        Start();
    }
}
