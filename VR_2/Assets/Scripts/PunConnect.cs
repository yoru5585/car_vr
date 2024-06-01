using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PunConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _avatar;
    [SerializeField] private Transform _pos;

    private const int _PLAYER_UPPER_LIMIT = 2;

    //���[���I�v�V�����̃v���p�e�B�[
    private RoomOptions _roomOptions = new RoomOptions()
    {
        MaxPlayers = _PLAYER_UPPER_LIMIT, //�l������
        IsOpen = true, //�����ɎQ���ł��邩
        IsVisible = true, //���̕��������r�[�Ƀ��X�g����邩
    };

    private void Start()
    {
        //PhotonServerSettings�ɐݒ肵�����e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    //�}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        // "Test"�Ƃ������O�̃��[���ɎQ������i���[����������΍쐬���Ă���Q������j
        PhotonNetwork.JoinOrCreateRoom("Test", _roomOptions, TypedLobby.Default);
    }

    //�����ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        Debug.Log("connect");
        //�A�o�^�[�𐶐�
        GameObject avatar = PhotonNetwork.Instantiate(
            _avatar.name,
            Vector3.zero,
            Quaternion.identity);
        //�v���C���[�̈ʒu�ɍ��킹�Ĉʒu�C��
        avatar.transform.position = new Vector3(_pos.position.x + 0.3f, _pos.position.y, _pos.position.z);
        avatar.transform.rotation = _pos.rotation;

        avatar.name = _avatar.name + "_vis";
    }
}

