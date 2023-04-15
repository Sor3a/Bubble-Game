using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum particleType
{
    DestoryBall,
    PrepearToBlow,
    Blow,
    ScoreText,
}
[System.Serializable]
public class particlePrefab
{
    public particlePrefab(particleType type, GameObject p)
    {
        this.type = type;
        this.prefab = p;
    }
    public particleType type;
    public GameObject prefab;
}
public class ObjectsPooler : MonoBehaviour
{
    //private int spawnAmount = 100;

    [SerializeField] List<particlePrefab> particles;
    Dictionary<particleType, GameObject> particlesObjects;
    List<particlePrefab> _pool;
    public static ObjectsPooler instance;
    Color textColor;
  
    //let the particle face some direction
    public void SpawnParticleFromPool(particleType type, Vector3 position, Vector3 direction, float timeToDisactivate = 1f)
    {
        var particle = spawnFromPool(type);
        var particleTransform = particle.transform;
        particleTransform.position = position;
        particleTransform.forward = direction;
        particle.GetComponent<ParticleSystem>().Play();
        DisableObject(particle, timeToDisactivate);
    }
    public void SpawnParticleFromPool(particleType type, Vector3 position,Color color, float timeToDisactivate = 1f)
    {
        var particle = spawnFromPool(type);
        var particleTransform = particle.transform;
        particleTransform.position = position;
        ParticleSystem ParticleSys = particle.GetComponent<ParticleSystem>();
        ParticleSys.startColor = color;
        ParticleSys.Play();
        DisableObject(particle, timeToDisactivate);
    }
    public void SpawnParticleFromPool(particleType type, Vector3 position, float timeToDisactivate = 1f)
    {
        var particle = spawnFromPool(type);
        particle.transform.position = position;
        particle.GetComponent<ParticleSystem>().Play();
        DisableObject(particle, timeToDisactivate);
    }
    public void SpawnTextFromPool(particleType type,Vector3 position,string text,float timeToDisactivate = 1f)
    {
        var TextUI = spawnFromPool(type);
        TextUI.transform.position = position;

       TextUI.GetComponentInChildren<TextMeshProUGUI>().text = text;
        DisableObject(TextUI, timeToDisactivate);
    }
    private void Awake()
    {
        particlesObjects = new Dictionary<particleType, GameObject>();
        foreach (var item in particles)
            particlesObjects.Add(item.type, item.prefab);

        _pool = new List<particlePrefab>();

        foreach (var item in particlesObjects)
            AddToPool(item.Key);

        textColor = new Color(0.5f,0.5f,0.5f,1f);
        instance = this;
    }
    GameObject spawnFromPool(particleType type)
    {
        var item = _pool.Find(x => x.type == type);
        if (item.prefab != null)
        {
            if (item.prefab.activeSelf == false)
                item.prefab.SetActive(true);
            else
            {
                item = AddToPool(type);
                item.prefab.SetActive(true);
            }
        }
        else
        {
            item = AddToPool(type);
            item.prefab.SetActive(true);
            return item.prefab;
        }

        return item.prefab;
    }
    void DisableObject(GameObject obj)
    {
        var item = _pool.Find(x => x.prefab == obj);
        if (item != null && item.prefab != null)
            item.prefab.SetActive(false);
    }
    void DisableObject(GameObject obj, float time)
    {
        StartCoroutine(diableAfterTime(obj, time));
    }
    particlePrefab AddToPool(particleType type)
    {
        var obj = new particlePrefab(type, spawnIntoPool(type));
        _pool.Add(obj);
        obj.prefab.SetActive(false);
        return obj;
    }
    public void DeleteFromPool(GameObject obj)
    {
        var item = _pool.Find(x => x.prefab == obj);
        if (item != null)
            _pool.Remove(item);
    }

    IEnumerator diableAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        DisableObject(obj);
    }

    GameObject spawnIntoPool(particleType type)
    {
        return Instantiate(particlesObjects[type], transform);
    }

}
