using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    private AsyncOperationHandle m_HatLoadingHandle;

    private ApplyRemoteConfigSettings remoteConfigScript;

    private GameObject m_HatInstance;
    private AsyncOperationHandle<IList<IResourceLocation>> m_HatsLocationsOpHandle;
    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;
    private readonly List<string> m_Keys = new() { "Hats", "Halloween" };

    void Start()
    {   
        // Get the instance of ApplyRemoteConfigSettings
        remoteConfigScript = ApplyRemoteConfigSettings.Instance;

        // Call the FetchConfigs() to see if there's any new settings
        remoteConfigScript.FetchConfigs();
        
        //If the condition is met, then a hat has been unlocked
        if(GameManager.s_ActiveHat >= 0)
        {
            // Fetch the correct hat variable from the ApplyRemoteConfigSettings instance
            if (ApplyRemoteConfigSettings.Instance.season == "Default")
            {
                SetHat(string.Format("Hat{0:00}", Random.Range(0, 4)));

                //Debug.Log("Formatted String 2 " + string.Format("Hat{0:00}", remoteConfigScript.activeHat));
                //SetHat(string.Format("Hat{0:00}", remoteConfigScript.activeHat));
            }

            else if (ApplyRemoteConfigSettings.Instance.season == "Winter")
            {
                SetHat(string.Format("Hat{0:00}", "04"));
            }

            else if (ApplyRemoteConfigSettings.Instance.season == "Halloween")
            {
                //SetHat(string.Format("Hat{0:00}", "05"));

                // loading resources based on halloween season
                m_HatsLocationsOpHandle = Addressables.LoadResourceLocationsAsync(
                    m_Keys,
                    Addressables.MergeMode.Intersection);

                m_HatsLocationsOpHandle.Completed += OnHatLocationsLoadComplete;
            }

            //hatKey is an Addressable Label
            //Debug.Log("Hat String: " + string.Format("Hat{0:00}", UnityEngine.Random.Range(0, 4)));
        }
    }

    public void SetHat(string hatKey)
    {
        // We are using the InstantiateAsync function on the Addressables API, the non-Addressables way 
        // looks something like the following line, however, this version is not Asynchronous
        // GameObject.Instantiate(prefabToInstantiate);
        m_HatLoadingHandle = Addressables.InstantiateAsync(hatKey, m_HatAnchor, false);

        m_HatLoadingHandle.Completed += OnHatInstantiated;
    }

    private void OnHatInstantiated(AsyncOperationHandle obj)
    {
        // We can check for the status of the InstantiationAsync operation:
        // Failed, Succeeded or None
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Hat instantiated successfully");
        }

        m_HatLoadingHandle.Completed -= OnHatInstantiated;
    }

    private void OnHatLocationsLoadComplete(
        AsyncOperationHandle<IList<IResourceLocation>> asyncOperationHandle)
    {
        Debug.Log("AsyncOperationHandle Status: " + asyncOperationHandle.Status);

        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<IResourceLocation> results = asyncOperationHandle.Result;
            for (int i = 0; i < results.Count; i++)
            {
                Debug.Log("Hat: " + results[i].PrimaryKey);
            }

            LoadInRandomHat(results);
        }
    }

    private void LoadInRandomHat(IList<IResourceLocation> resourceLocations)
    {
        int randomIndex = Random.Range(0, resourceLocations.Count);
        IResourceLocation randomHatPrefab = resourceLocations[randomIndex];

        m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(randomHatPrefab);
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;
    }

    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
        }
    }

    private void OnDisable()
    {
        m_HatLoadingHandle.Completed -= OnHatInstantiated;
        m_HatsLocationsOpHandle.Completed -= OnHatLocationsLoadComplete;
    }
}
