using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemBox : NetworkBehaviour
{
    [SerializeField]
    Transform SelfTransform;

    [SerializeField]
    Vector3 RotateAngle = new Vector3(0.0f, 0.5f, 0.0f);

    [SyncVar]
    [SerializeField]
    string filePath;

    public string FilePath
    {
        get
        {
            return filePath;
        }
        set
        {
            filePath = value;
        }
    }

    /// <summary>
    /// 이동할 벡터
    /// </summary>
    [SerializeField]
    Vector3 MoveVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.ItemBoxManager.transform);
            inGameSceneMain.ItemBoxCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotate();
        UpdateMove();
    }

    void UpdateRotate()
    {
        Vector3 eulerAngles = SelfTransform.localRotation.eulerAngles;
        eulerAngles += RotateAngle;
        SelfTransform.Rotate(RotateAngle, Space.Self);
    }

    void UpdateMove()
    {
        SelfTransform.position += MoveVector * Time.deltaTime;
    }

    [ClientRpc]
    public void RpcSetActive(bool value)
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        OnItemCollision(other);
    }

    void OnItemCollision(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        if (player.IsDead)
            return;
        Debug.Log("OnItemCollision");
    }

}
