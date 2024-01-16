using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
	public class Glitch : MonoBehaviour
	{
		private const string MATERIAL_TRANSPARENCY = "_Transparency";
		private const string MATERIAL_CUTOUT = "_CutoutThresh";
		private const string MATERIAL_AMPLITUDE = "_Amplitude";
		private const string MATERIAL_SPEED = "_Speed";
		private const string MATERIAL_AMOUNT = "_Amount";

		[SerializeField] private Renderer[] _renderers;
		[Header("Glitch Chance Settings")]
		[Range(0f, 1f)]
		[SerializeField] private float _glitchChance = .1f;
		[SerializeField] private float _glitchCheckFrequency = .1f;
		[SerializeField] private float _glitchDurationMin = .05f;
		[SerializeField] private float _glitchDurationMax = .25f;
		[Header("Glitch Effect")]
		[Range(0f, .5f)]
		[SerializeField] private float _transparencyMin = .1f;
		[Range(0f, .5f)]
		[SerializeField] private float _transparencyMax = .25f;
		[SerializeField] private float _amplitudeMin = 15;
		[SerializeField] private float _amplitudeMax = 30;
		[SerializeField] private float _speedMin = 20;
		[SerializeField] private float _speedMax = 40;
		[Range(0f, 1f)]
		[SerializeField] private float _cutoutThreshold = .45f;
		[Range(0f, 1f)]
		[SerializeField] private float _amount = 1;

		private struct DefaultValues
		{
			public float Transparency;
			public float CutoutThreshold;
			public float Amplitude;
			public float Speed;
			public float Amount;
		}

		private Dictionary<Renderer, DefaultValues> _defaultValues;
		private WaitForSeconds _glitchDuration;

		private bool ShouldGlitch => UnityEngine.Random.Range(0f, 1f) < _glitchChance;

		private void Awake()
		{
			CacheDefaultValues();
		}

		private void OnEnable()
		{
			StartCoroutine(CheckGlitchRoutine());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
			ApplyDefaultValues();
		}

		private void CacheDefaultValues()
		{
			_defaultValues ??= new();
			_defaultValues.Clear();
			foreach (var renderer in _renderers)
			{
				_defaultValues.Add(renderer, new DefaultValues()
				{
					Transparency = renderer.material.GetFloat(MATERIAL_TRANSPARENCY),
					CutoutThreshold = renderer.material.GetFloat(MATERIAL_CUTOUT),
					Amplitude = renderer.material.GetFloat(MATERIAL_AMPLITUDE),
					Speed = renderer.material.GetFloat(MATERIAL_SPEED),
					Amount = renderer.material.GetFloat(MATERIAL_AMOUNT)
				});
			}
		}

		private void ApplyDefaultValues()
		{
			foreach (var renderer in _defaultValues.Keys)
			{
				renderer.material.SetFloat(MATERIAL_TRANSPARENCY, _defaultValues[renderer].Transparency);
				renderer.material.SetFloat(MATERIAL_CUTOUT, _defaultValues[renderer].CutoutThreshold);
				renderer.material.SetFloat(MATERIAL_AMPLITUDE, _defaultValues[renderer].Amplitude);
				renderer.material.SetFloat(MATERIAL_SPEED, _defaultValues[renderer].Speed);
				renderer.material.SetFloat(MATERIAL_AMOUNT, _defaultValues[renderer].Amount);
			}
		}

		private IEnumerator CheckGlitchRoutine()
		{
			while (true)
			{
				if (ShouldGlitch)
					StartCoroutine(GlitchRoutine());

				yield return new WaitForSeconds(_glitchCheckFrequency);
			}
		}

		private IEnumerator GlitchRoutine()
		{
			_glitchDuration = new WaitForSeconds(UnityEngine.Random.Range(_glitchDurationMin, _glitchDurationMax));

			foreach (var renderer in _defaultValues.Keys)
			{
				renderer.material.SetFloat(MATERIAL_TRANSPARENCY, UnityEngine.Random.Range(_transparencyMin, _transparencyMax));
				renderer.material.SetFloat(MATERIAL_CUTOUT, _cutoutThreshold);
				renderer.material.SetFloat(MATERIAL_AMPLITUDE, UnityEngine.Random.Range(_amplitudeMin, _amplitudeMax));
				renderer.material.SetFloat(MATERIAL_SPEED, UnityEngine.Random.Range(_speedMin, _speedMax));
				renderer.material.SetFloat(MATERIAL_AMOUNT, _amount);
			}

			yield return _glitchDuration;

			ApplyDefaultValues();
		}

	}
}
