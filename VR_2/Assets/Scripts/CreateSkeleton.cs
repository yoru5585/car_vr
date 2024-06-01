using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateSkeleton : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject _HandVisual;
    [SerializeField] private string ovrSkeletonName;//"LeftOVRHand"
    [SerializeField] private Vector3 HandScale;

    private readonly List<Transform> _bonesL = new List<Transform>();
    private List<Transform> _listOfChildren = new List<Transform>();
    private Quaternion _wristFixupRotation;
    private List<OVRBone> _bones;
    OVRSkeleton.IOVRSkeletonDataProvider dataProviderL;
    // Start is called before the first frame update
    void Start()
    {
        OVRSkeleton ovrSkeletonL = GameObject.Find(ovrSkeletonName).GetComponent<OVRSkeleton>();
        dataProviderL = ovrSkeletonL.GetComponent<OVRSkeleton.IOVRSkeletonDataProvider>();

        //�{�[���̏���C#�ŗ��p�\�ɂ��郉�b�p�[�N���X
        OVRPlugin.Skeleton skeleton = new OVRPlugin.Skeleton();

        //�{�[���̌��f�[�^�𐶐�
        OVRPlugin.GetSkeleton((OVRPlugin.SkeletonType)dataProviderL.GetSkeletonType(), out skeleton);
        InitializeBones(skeleton, _HandVisual);

        //�����������Ő��������{�[���̃��X�g���쐬
        ReadyHand(_HandVisual, _bonesL);

        _wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
    }

    private void Update()
    {
        var _dataL = dataProviderL.GetSkeletonPoseData();

        //����
        if (_dataL.IsDataValid && _dataL.IsDataHighConfidence)
        {
            //���[�g�̃��[�J���|�W�V������K�p
            _HandVisual.transform.localPosition = _dataL.RootPose.Position.FromFlippedZVector3f();
            _HandVisual.transform.localRotation = _dataL.RootPose.Orientation.FromFlippedZQuatf();

            //_HandVisual.transform.localScale =
            //    new Vector3(_dataL.RootScale, _dataL.RootScale, _dataL.RootScale);

            //�X�P�[����ω�
            _HandVisual.transform.localScale = HandScale;

            //�{�[���̃��X�g�Ɏ󂯎�����l�𔽉f
            for (int i = 0; i < _bonesL.Count; ++i)
            {
                _bonesL[i].transform.localRotation = _dataL.BoneRotations[i].FromFlippedXQuatf();

                if (_bonesL[i].name == OVRSkeleton.BoneId.Hand_WristRoot.ToString())
                {
                    _bonesL[i].transform.localRotation *= _wristFixupRotation;
                }
            }
        }

        //CheckList(_dataL);
    }


    /// <summary>
    /// Bones�𐶐�
    /// </summary>
    /// <param name="skeleton">���炩���ߗp�ӂ��ꂽ�{�[���̏��</param>
    /// <param name="hand">���E�ǂ��炩�̎�</param>
    private void InitializeBones(OVRPlugin.Skeleton skeleton, GameObject hand)
    {
        _bones = new List<OVRBone>(new OVRBone[skeleton.NumBones]);

        GameObject _bonesGO = new GameObject("Bones");
        _bonesGO.transform.SetParent(hand.transform, false);
        _bonesGO.transform.localPosition = Vector3.zero;
        _bonesGO.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < skeleton.NumBones; ++i)
        {
            OVRSkeleton.BoneId id = (OVRSkeleton.BoneId)skeleton.Bones[i].Id;
            short parentIdx = skeleton.Bones[i].ParentBoneIndex;
            Vector3 pos = skeleton.Bones[i].Pose.Position.FromFlippedXVector3f();
            Quaternion rot = skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf();

            GameObject boneGO = new GameObject(id.ToString());
            boneGO.transform.localPosition = pos;
            boneGO.transform.localRotation = rot;
            _bones[i] = new OVRBone(id, parentIdx, boneGO.transform);
        }

        for (int i = 0; i < skeleton.NumBones; ++i)
        {
            if (((OVRPlugin.BoneId)skeleton.Bones[i].ParentBoneIndex) == OVRPlugin.BoneId.Invalid)
            {
                _bones[i].Transform.SetParent(_bonesGO.transform, false);
            }
            else
            {
                _bones[i].Transform.SetParent(_bones[_bones[i].ParentBoneIndex].Transform, false);
            }
        }
    }

    /// <summary>
    /// ��̃{�[���̃��X�g���쐬
    /// ���Oculus�̎��{�[�����̃��X�g�ƏƂ炵���킹�Ēl���X�V����̂ŏ��ԂɈ�H�v���č쐬
    /// </summary>
    /// <param name="hand">�q�Ƀ{�[���������Ă����</param>
    /// <param name="bones">��̃��X�g</param>
    private void ReadyHand(GameObject hand, List<Transform> bones)
    {
        //'Bones'�Ɩ��̕t���I�u�W�F�N�g���烊�X�g���쐬����
        foreach (Transform child in hand.transform)
        {
            _listOfChildren = new List<Transform>();
            GetChildRecursive(child.transform);

            //�܂��͎w��ȊO�̃��X�g���쐬
            List<Transform> fingerTips = new List<Transform>();
            foreach (Transform bone in _listOfChildren)
            {
                if (bone.name.Contains("Tip"))
                {
                    fingerTips.Add(bone);
                }
                else
                {
                    bones.Add(bone);
                }
            }

            //�w������X�g�ɒǉ�
            foreach (Transform bone in fingerTips)
            {
                bones.Add(bone);
            }
        }

        //���I�ɐ�������郁�b�V����SkinnedMeshRenderer�ɔ��f
        SkinnedMeshRenderer skinMeshRenderer = hand.GetComponent<SkinnedMeshRenderer>();
        OVRMesh ovrMesh = hand.GetComponent<OVRMesh>();

        Matrix4x4[] bindPoses = new Matrix4x4[bones.Count];
        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
        for (int i = 0; i < bones.Count; ++i)
        {
            bindPoses[i] = bones[i].worldToLocalMatrix * localToWorldMatrix;
        }

        //Mesh�ASkinnedMeshRenderer��BindPose�ABone�𔽉f
        ovrMesh.Mesh.bindposes = bindPoses;
        skinMeshRenderer.bones = bones.ToArray();
        skinMeshRenderer.sharedMesh = ovrMesh.Mesh;
    }

    /// <summary>
    /// �q�̃I�u�W�F�N�g��Transform���ċA�I�ɑS�Ď擾
    /// </summary>
    /// <param name="obj">���g�̎q��S�Ď擾���������[�g�I�u�W�F�N�g</param>
    private void GetChildRecursive(Transform obj)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;

            if (child != obj)
            {
                _listOfChildren.Add(child);
            }

            GetChildRecursive(child);
        }
    }

    /// <summary>
    /// Transform������肷��
    /// </summary>
    /// <param name="stream">�l�̂������\�ɂ���X�g���[��</param>
    /// <param name="info">�^�C���X�^���v���ׂ̍�����񂪂����\</param>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���g�̃N���C�A���g���瑊��N���C�A���g�̓����I�u�W�F�N�g�ɑ�����
        if (stream.IsWriting)
        {
            stream.SendNext(_HandVisual.transform.localPosition);
            stream.SendNext(_HandVisual.transform.localRotation);

            //�{�[���̃��X�g�Ɏ󂯎�����l�𔽉f
            for (var i = 0; i < _bonesL.Count; ++i)
            {
                stream.SendNext(_bonesL[i].transform.localRotation);
            }
        }
        //����̃N���C�A���g���玩�g�̃N���C�A���g�̓����I�u�W�F�N�g�ɑ����Ă�����
        else
        {
            _HandVisual.transform.localPosition = (Vector3)stream.ReceiveNext();
            _HandVisual.transform.localRotation = (Quaternion)stream.ReceiveNext();

            //�{�[���̃��X�g�Ɏ󂯎�����l�𔽉f
            for (var i = 0; i < _bonesL.Count; ++i)
            {
                _bonesL[i].transform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    private void CheckList(OVRSkeleton.SkeletonPoseData skel)
    {
        TextMeshProUGUI LogText = GameObject.FindGameObjectWithTag("log").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI LogText2 = GameObject.FindGameObjectWithTag("log2").GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < _bonesL.Count; i++)
        {
            if (_bonesL[i].transform.rotation != skel.BoneRotations[i].FromFlippedXQuatf())
            {
                LogText.text += "\n" + i + ":" + _bonesL[i].transform.rotation;
                LogText2.text += "\n" + i + ":" + skel.BoneRotations[i].FromFlippedXQuatf();
            }
            
        }

    }

}
