using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensRainSystems : MonoBehaviour 
{
	public ParticleSystem[] m_Systems; // Particle system array

	public float size = 1;
	public float amount = 1;
	public float direction = 1;

	private void Start()
	{
		SetParameters();
	}
		
	// Set parameters from the effect on the particle systems
	public void SetParameters()
	{
		for(int i = 0; i < m_Systems.Length; i++) // Iterate particle systems
		{
			var main = m_Systems[i].main; // Get main module
			main.startSize = new ParticleSystem.MinMaxCurve(size, size * 0.5f); // Set size
			var emission = m_Systems[i].emission; // Get emission module
			emission.rateOverTime = new ParticleSystem.MinMaxCurve(amount * 2f); // Set amount
			var subEmitters = m_Systems[i].subEmitters; // Get sub emitters
			bool hasSubmitters = subEmitters.subEmittersCount > 0 ? true : false; // Does the system have subemitters?
			if(hasSubmitters) // If the system has subemitters
			{
				ParticleSystem childSystem = subEmitters.GetSubEmitterSystem(0); // Get sub emitter
				if(childSystem)
				{
					var forceOverLifeTime = childSystem.forceOverLifetime; // Get force over lifetime
					forceOverLifeTime.x = new ParticleSystem.MinMaxCurve(direction, 0); // Set direction
				}
			}
		}
	}
}
