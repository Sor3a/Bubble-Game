using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField] AssetReference GameLooksAssetRef;
    [SerializeField] Vector2 position;
    GameObject GameLook;
    private void Start()
    {
        Addressables.InitializeAsync().Completed += AddressablesManagerCompleted;
    }

    void AddressablesManagerCompleted(AsyncOperationHandle<IResourceLocator> obj)
    {
        GameLooksAssetRef.InstantiateAsync().Completed += (x) =>
          {
              GameLook = x.Result;
              //Instantiate(GameLook);
              GameLook.transform.position = position;
          };
    }

    private void OnDestroy()
    {
        if(GameLook!=null)
        GameLooksAssetRef.ReleaseInstance(GameLook);
    }

}
