using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PunConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _avatar;
    [SerializeField] private Transform _pos;

    private const int _PLAYER_UPPER_LIMIT = 2;

    //ルームオプションのプロパティー
    private RoomOptions _roomOptions = new RoomOptions()
    {
        MaxPlayers = _PLAYER_UPPER_LIMIT, //人数制限
        IsOpen = true, //部屋に参加できるか
        IsVisible = true, //この部屋がロビーにリストされるか
    };

    private void Start()
    {
        //PhotonServerSettingsに設定した内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    //マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Test"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        PhotonNetwork.JoinOrCreateRoom("Test", _roomOptions, TypedLobby.Default);
    }

    //部屋への接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("connect");
        //アバターを生成
        GameObject avatar = PhotonNetwork.Instantiate(
            _avatar.name,
            Vector3.zero,
            Quaternion.identity);
        //プレイヤーの位置に合わせて位置修正
        avatar.transform.position = new Vector3(_pos.position.x + 0.3f, _pos.position.y, _pos.position.z);
        avatar.transform.rotation = _pos.rotation;

        avatar.name = _avatar.name + "_vis";
    }
}

