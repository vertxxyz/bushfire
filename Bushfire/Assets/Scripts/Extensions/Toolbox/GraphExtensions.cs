using UnityEngine;
using System.Collections;

public static class GraphExtensions {
	public static float TriangleWave (float t, float amplitude, float halfPeriod){
		return (amplitude / halfPeriod) * (halfPeriod - Mathf.Abs (t % (2 * halfPeriod) - halfPeriod));
	}

	/// <summary>
	/// A triangle wave with wait periods at the bottom and top of the wave.
	/// </summary>
	/// <param name="t">Time.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="halfPeriod">Half-period.</param>
	/// <param name="waitPeriod">Wait period.</param>
	public static float TriangleWave (float t, float amplitude, float halfPeriod, float waitPeriod){
		t += waitPeriod/2f;
		float newAmplitude = amplitude + ((waitPeriod / 2f) / halfPeriod) * amplitude * 2;
		float newPeriod = halfPeriod + waitPeriod;
		float newTriangle = TriangleWave (t, newAmplitude, newPeriod);
		newTriangle -= ((waitPeriod / 2f) / halfPeriod) * amplitude;
		return Mathf.Clamp (newTriangle, 0, amplitude);
	}

	public static float SawtoothWave (float t, float amplitude, float period){
		return amplitude * ((t % period) / period);
	}

	/// <summary>
	/// A sawtooth wave with wait periods at the bottom and top of the wave.
	/// </summary>
	/// <param name="t">Time.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="period">Period.</param>
	/// <param name="waitPeriod">Wait period.</param>
	public static float SawtoothWave (float t, float amplitude, float period, float waitPeriod){
		t += waitPeriod/2;
		float newAmplitude = amplitude + ((waitPeriod / 2f) / period) * amplitude * 2;
		float newPeriod = period + waitPeriod;
		float newTriangle = SawtoothWave (t, newAmplitude, newPeriod);
		newTriangle -= ((waitPeriod / 2f) / period) * amplitude;
		return Mathf.Clamp (newTriangle, 0, amplitude);
	}

	/// <summary>
	/// A sawtooth wave with wait periods at the bottom and top of the wave.
	/// </summary>
	/// <param name="t">Time.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="period">Period.</param>
	/// <param name="waitPeriod">Wait period.</param>
	public static float SawtoothWave (float t, float amplitude, float period, float waitPeriodTop, float waitPeriodBottom){
		t += waitPeriodBottom;
		float waitPeriod = waitPeriodTop + waitPeriodBottom;
		float newAmplitude = amplitude + ((waitPeriod / 2f) / period) * amplitude * 2;
		float newPeriod = period + waitPeriod;
		float newTriangle = SawtoothWave (t, newAmplitude, newPeriod);
		newTriangle -= ((waitPeriodBottom) / period) * amplitude;
		return Mathf.Clamp (newTriangle, 0, amplitude);
	}
}
