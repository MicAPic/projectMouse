using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ParticleSystem))]
public class AttachLights2DToParticles : MonoBehaviour
{
    [SerializeField]
    public GameObject prefab;

    private ParticleSystem _particleSystem;
    private List<GameObject> _instances = new();
    private List<Light2D> _lights = new();
    private ParticleSystem.Particle[] _particles;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var count = _particleSystem.GetParticles(_particles);

        while (_instances.Count < count)
        {
            var light2d = Instantiate(prefab, _particleSystem.transform);
            _instances.Add(light2d);
            _lights.Add(light2d.GetComponent<Light2D>());
        }

        var worldSpace = (_particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
        for (var i = 0; i < _instances.Count; i++)
        {
            if (i < count)
            {
                if (worldSpace)
                    _instances[i].transform.position = _particles[i].position;
                else
                    _instances[i].transform.localPosition = _particles[i].position;
                _instances[i].SetActive(true);
                _lights[i].intensity = _particles[i].GetCurrentColor(_particleSystem).a / 255f;
            }
            else
            {
                _instances[i].SetActive(false);
            }
        }
    }
}
