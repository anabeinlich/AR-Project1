using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public class ImagePrefabPair
{
    public string imageName;
    public GameObject prefab;
}

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageSpawner : MonoBehaviour
{
    [SerializeField] private List<ImagePrefabPair> imagePrefabPairs;

    private ARTrackedImageManager trackedImageManager;
    private readonly Dictionary<string, GameObject> spawnedPrefabs = new();

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void Start()
    {
        foreach (var pair in imagePrefabPairs)
        {
            var newPrefab = Instantiate(pair.prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = pair.imageName;
            newPrefab.SetActive(false);
            spawnedPrefabs[pair.imageName] = newPrefab;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateSpawnedPrefab(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateSpawnedPrefab(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedPrefabs.TryGetValue(trackedImage.referenceImage.name, out var prefab))
            {
                prefab.SetActive(false);
            }
        }
    }

    private void UpdateSpawnedPrefab(ARTrackedImage trackedImage)
    {
        var imageName = trackedImage.referenceImage.name;

        if (spawnedPrefabs.TryGetValue(imageName, out var prefab))
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                prefab.SetActive(true);
                prefab.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
            }
            else
            {
                prefab.SetActive(false);
            }
        }
    }
}