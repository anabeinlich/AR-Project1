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
            UpdateSpawnedPrefab(trackedImage);

        foreach (var trackedImage in eventArgs.updated)
            UpdateSpawnedPrefab(trackedImage);

        foreach (var trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            if (spawnedPrefabs.TryGetValue(imageName, out var prefab))
                prefab.SetActive(false);
        }
    }

    private void UpdateSpawnedPrefab(ARTrackedImage trackedImage)
    {
        if (trackedImage.referenceImage.guid == System.Guid.Empty)
        {
            Debug.LogWarning("ARTrackedImage con GUID vacío. Ignorando actualización.");
            return;
        }

        string imageName = trackedImage.referenceImage.name;

        if (string.IsNullOrWhiteSpace(imageName))
        {
            Debug.LogWarning("ARTrackedImage con nombre vacío. Unity 6 bug.");
            return;
        }

        if (!spawnedPrefabs.TryGetValue(imageName, out var prefab))
        {
            Debug.LogWarning($"No hay prefab asignado a la imagen '{imageName}'.");
            return;
        }

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            prefab.SetActive(true);

            prefab.transform.SetPositionAndRotation(
                trackedImage.transform.position,
                trackedImage.transform.rotation * Quaternion.Euler(0, 180, 0) 
            );
        }
        else
        {
             prefab.SetActive(false);
        }
    }
}