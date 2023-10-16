using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PianoManager : MonoBehaviour
{
    public GameObject fingerTip;
    public GameObject piano;
    public GameObject leftAnchor;
    public GameObject frontAnchor;
    const string leftKey = "PianoLeft";
    const string frontKey = "PianoFront";

    bool _isCalibShowing = false;
    public bool isCalibShowing
    {
        get { return _isCalibShowing; }
        set
        {
            _isCalibShowing = value;
            fingerTip.GetComponent<MeshRenderer>().enabled = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PersistentLoad(leftAnchor, leftKey);
        PersistentLoad(frontAnchor, frontKey);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            _ = CreateAndSave(leftAnchor, leftKey);
        }
        if (Input.GetKeyDown(KeyCode.F) || OVRInput.GetDown(OVRInput.Button.PrimaryShoulder))
        {
            _ = CreateAndSave(frontAnchor, frontKey);
        }
        if (Input.GetKeyDown(KeyCode.S) || OVRInput.GetDown(OVRInput.RawTouch.A))
        {
            isCalibShowing = !isCalibShowing;
        }
    }

    public void OnCalibButtonPressed()
    {
        isCalibShowing = !isCalibShowing;
    }
    public void OnLeftButtonPressed()
    {
        _ = CreateAndSave(leftAnchor, leftKey);
    }
    public void OnFrontButtonPressed()
    {
        _ = CreateAndSave(frontAnchor, frontKey);
    }



    void OrientPiano()
    {
        if (leftAnchor.GetComponent<OVRSpatialAnchor>() == null || frontAnchor.GetComponent<OVRSpatialAnchor>() == null) return;
        var temp = frontAnchor.transform.position - leftAnchor.transform.position;
        var right = new Vector3(temp.x, 0, temp.z) * -1;
        var forward = Quaternion.Euler(0, -90, 0) * right;
        piano.transform.forward = forward;
    }

    async Task CreateAndSave(GameObject anchorGO, string key)
    {
        var previousAnchor = anchorGO.GetComponent<OVRSpatialAnchor>();
        if (previousAnchor != null)
        {
            var b = await previousAnchor.EraseAsync();
            Debug.Log("erased anchor: " + b);
            Destroy(previousAnchor);
            await Task.Delay(500);
        }
        anchorGO.transform.position = fingerTip.transform.position;
        var anchorOVR = anchorGO.AddComponent<OVRSpatialAnchor>();
        StartCoroutine(PersistentSave(anchorOVR, key));
    }

    IEnumerator PersistentSave(OVRSpatialAnchor anchor, string key)
    {
        // keep checking for a valid and localized spatial anchor state
        while (!anchor.Created && !anchor.Localized)
        {
            yield return new WaitForEndOfFrame();
        }

        anchor.Save((anchor, success) =>
        {
            Debug.Log("spatial anchor: " + success);
            PlayerPrefs.SetString(key, anchor.Uuid.ToString());

            OrientPiano(); // todo: async Task
        });
    }

    void PersistentLoad(GameObject anchorGO, string key)
    {
        if (!PlayerPrefs.HasKey(key)) return;
        var _uuid = new Guid(PlayerPrefs.GetString(key));
        Load(new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = new Guid[] { _uuid }
        }, anchorGO);
    }

    private void Load(OVRSpatialAnchor.LoadOptions options, GameObject anchorGO) => OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
    {
        if (anchors == null)
        {
            Debug.Log("Query failed.");
            return;
        }

        foreach (var anchor in anchors)
        {
            if (anchor.Localized)
            {
                OnLocalized(anchor, true, anchorGO);
            }
            else if (!anchor.Localizing)
            {
                anchor.Localize((unboundAnchor, success) => OnLocalized(unboundAnchor, success, anchorGO));
            }
        }
    });

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success, GameObject anchorGO)
    {
        if (!success)
        {
            Debug.Log($"{unboundAnchor} Localization failed!");
            return;
        }

        var anchorOVR = anchorGO.AddComponent<OVRSpatialAnchor>();
        unboundAnchor.BindTo(anchorOVR);
        OrientPiano(); // todo: async Task
    }
}
