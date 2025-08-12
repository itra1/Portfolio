using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace it.Game.Environment.Level2
{
  public class DarkMushrooms : Environment
  {
    /*
     * Состояния:
     * 0 - закрыто
     * 1 - открыто
     */

    public bool rewriteVertexStreams = true;
    public bool GPU = false;
    [SerializeField]
    private RangeFloat _size;
    public Color particleColor = Color.white;
    public Vector3 particleRotation3D;
    public bool randomColorAlpha = true; // For MetallicSmoothness random offset
    public float xDistance = 0.25f;
    public float yDistance = 0.25f;
    public float zDistance = 0.25f;
    public int xSize = 10;
    public int ySize = 10;
    public int zSize = 10;
    public float OffsetEven = 0.125f;
    public bool updateEveryFrame = false;

    [SerializeField]
    private float upCheck = 3;
    [SerializeField]
    private float heightCheck = 10;

    [SerializeField]
    private LayerMask _layer;

    private float even;
    [SerializeField]
    private Vector3[] positions;
    private ParticleSystem ps;
    private ParticleSystemRenderer psr;
    private ParticleSystem.Particle[] particles;
    private List<Vector4> customData = new List<Vector4>();
    private List<Vector4> customData2 = new List<Vector4>();

    [SerializeField]
    private CapsuleCollider[] _colliders;

    protected override void Start()
    {
      base.Start();
      ps = GetComponent<ParticleSystem>();
      psr = GetComponent<ParticleSystemRenderer>();
      psr.material.SetVector("_Affector", transform.position + Vector3.up * 100);
      UpdateGrid();
    }

    private void OnEnable()
    {
      ps = GetComponent<ParticleSystem>();
      UpdateGrid();
    }

    [ContextMenu("Check Spawn")]
    private void CheckSpawn()
    {
      GenerateGrid();
      UpdateGrid();

    }

    [ContextMenu("Spawn")]
    public void UpdateGrid()
    {
      GenerateParticles();
      CreateOffsetVector();

      ParticleSystemRenderer psrend = GetComponent<ParticleSystemRenderer>();
      if (rewriteVertexStreams == true)
      {
        psrend.SetActiveVertexStreams(new List<ParticleSystemVertexStream>(new ParticleSystemVertexStream[] { ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Normal, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV, ParticleSystemVertexStream.Center, ParticleSystemVertexStream.Tangent, ParticleSystemVertexStream.Custom1XYZ }));

      }
      psrend.alignment = ParticleSystemRenderSpace.Local;
    }

    [ContextMenu("Generate Grid")]
    private void GenerateGrid()
    {
      List<Vector3> pos = new List<Vector3>();
      RaycastHit _hit;
      float startX = (xSize * xDistance) / 2;
      float startZ = (zSize * zDistance) / 2;
      for (int z = 0, i = 0; z < zSize; z++)
      {
        even = 0f;
        if (z % 2 == 0)
        {
          even = OffsetEven;
        }
        for (int y = 0; y < ySize; y++)
        {
          for (int x = 0; x < xSize; x++, i++)
          {
            if (Random.value <= 0.3f)
            {
              Vector3 posit = new Vector3(x * xDistance + even - startX, y * yDistance, z * zDistance - startZ) + new Vector3((xDistance / 2) * Random.value, (yDistance / 2) * Random.value, (zDistance / 2) * Random.value);
              if(Utilites.Geometry.RaycastExt.SafeRaycast(transform.position + posit + Vector3.up * upCheck, -Vector3.up,out _hit, heightCheck, _layer))
              {
                //pos.Add(_hit.point- transform.position);
                Vector3 pnt = _hit.point;
                for (int ii = 0; ii < _colliders.Length; ii++)
                {
                  pnt.y = _colliders[ii].transform.position.y;
                  if ((_colliders[ii].transform.position - pnt).magnitude <= _colliders[ii].transform.localScale.x*0.5f)
                  {
                    pos.Add(_hit.point - transform.position);
                    break;
                  }
                }

              }
            }
            //pos.Add(new Vector3(x * xDistance + even, y * yDistance, z * zDistance) + new Vector3((xDistance/2)*Random.value, (yDistance/2) * Random.value, (zDistance/2) * Random.value));
          }
        }
      }
      positions = pos.ToArray();
    }

    // Generating particles with grid based positions
    private void GenerateParticles()
    {
      particles = new ParticleSystem.Particle[positions.Length];
      for (int i = 0; i < particles.Length; i++)
      {
        particles[i].position = positions[i];
        if (randomColorAlpha == true)
          particleColor.a = Random.Range(0f, 1f);
        particles[i].startColor = particleColor;
        particles[i].startSize = _size.RandomRange;
        particles[i].rotation3D = particleRotation3D;
      }
      ps.SetParticles(particles, particles.Length);
    }

    // Creating Vector for Offset
    private void CreateOffsetVector()
    {
      ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

      for (int i = 0; i < particles.Length; i++)
      {
        if (GPU == true)
        {
          customData[i] = new Vector3(0, 1, 0);
        }
        else
        {
          customData[i] = this.gameObject.transform.up;
        }
      }

      ps.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }

    public void Activate()
    {
      SetOpen();

      Save();
    }

    private void SetOpen()
    {
      State = 1;
      for (int i = 0; i < _colliders.Length; i++)
      {
        _colliders[i].enabled = false;
      }
    }

    private void Update()
    {
      if(State == 1)
        psr.material.SetVector("_Affector", Player.PlayerBehaviour.Instance.transform.position+ Vector3.up*3);
    }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);
      if (State > 0)
        SetOpen();
    }

  }
}