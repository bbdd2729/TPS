public enum PoseStation
{
   Stand,
   Crouch,
   MidAir,
   ATK,
   Aim
}

public enum MoveStation
{
   Idle,
   Walk,
   Run
}

public enum CharacterName
{
   HuoHuo,
   Anby,
   KeLin
}
//public class CharacterMoveController : CharacterMoveControllerBase
//{
//    //�������ת
//    //�����PoseStation
//    //�����MoveStation
//    //Animator��ֵ����
//    [SerializeField, Header("��ɫ������")] private CharacterName characterName;
//    [SerializeField, Header("��ɫ�ĵ�ǰת��")] private float currentRotationTime;
//    [SerializeField, Header("��ɫ���ܵ�ת��")] private float runRotationTime;
//    [SerializeField, Header("��ɫidle��ת��")] private float normalRotationTime;
//    private Transform cameraTransform;
//    private float rotationTargetAngle;
//    private float currentVelocity = 0;
//    private Vector3 characterTargetDir;
//    private float turnDeltaAngle;

//    public static PoseStation poseStation;
//    //private float standThreshold = 1;
//    //private float crouchThreshold = 0;
//    //private float midAirThreshold = 2.1f;
//    [SerializeField] MoveStation moveStation;
//    [SerializeField, Header("�ﵽRun��ʱ��")] private float walkToRunTimer=4;
//    private float walkToRunDeltaTimer;
//    [SerializeField, Header("�ﵽIdle�Ļ���ʱ��")] private float toIdleBufferTimer=0.3f;
//    //private float toIdleBufferDeltaTimer;
//    //private bool hasDodge;
//    [SerializeField, Header("���ܵ��䶳ʱ��")] private float dodgeColdTime;


//    protected override void Awake()
//    {
//        base.Awake();
//        cameraTransform = Camera.main.transform;
//    }
//    protected override void Start()
//    {
//        hasDodge = false;
//        walkToRunDeltaTimer = walkToRunTimer;
//        toIdleBufferDeltaTimer = toIdleBufferTimer;
//    }
//    //protected override void Update()
//    //{
//    //    base.Update();
//    //    if (characterName== SwitchCharacter.newCharacterName.Value)
//    //    {
//    //        UpdateDodge();  
//    //        UpdatePoseStation();
//    //        SetPoseStationValue(); 
//    //        UpdateMoveStation();
//    //        SetAnimationValue();

//    //        UpdateRotationTime();
//    //    }

//    //}


//    private void LateUpdate()
//    {
//        CharacterRotation();
//    }

//    private bool CanRotation()
//    {
//        if (characterAnimator.AnimationAtTag("TurnRun")) return false;
//        return true;
//    }
//    /// <summary>
//    /// ������ת
//    /// </summary>
//    private void CharacterRotation()
//    {
//        if (poseStation == PoseStation.Aim) { return; }
//        if (characterAnimator.AnimationAtTag("TurnRun")&&characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime<0.3f)
//        { return; }
//        if ((characterAnimator.AnimationAtTag("ATK")||characterAnimator.AnimationAtTag("Skill")) && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f)
//        { return; }
//        if (characterAnimator.AnimationAtTag("RushATK")) 
//        { return; }


//            //�����ɫ����ת��
//         rotationTargetAngle = Mathf.Atan2(CharacterInputSystem.MainInstance.PlayerMove.x, CharacterInputSystem.MainInstance.PlayerMove.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
//        //�õ�Ŀ������ת��ķ���
//        //Quaternion.Euler*Vector3��ʾ���������������ת
//        characterTargetDir = Quaternion.Euler(0, rotationTargetAngle, 0) * Vector3.forward;
//        //����Ŀ�귽�����ɫ����ļн�    

//        turnDeltaAngle = DevelopmentToos.GetDeltaAngle(transform, characterTargetDir);
//        //if (characterAnimator.GetBool(AnimatorID.HasMoveInputID))
//        //{

//        //    characterAnimator.SetFloat(AnimatorID.TurnDeltaAngleID, turnDeltaAngle);

//        //    //����ת��Ӧ�õ���ɫ����
//        //    //if (!characterAnimator.AnimationAtTag("TurnRun"))
//        //    {
//        //        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationTargetAngle, ref currentVelocity, currentRotationTime);
//        //    }

//        //}

//    }
//    private void UpdateRotationTime()
//    {
//        if (characterAnimator.GetBool(AnimatorID.RunID))
//        {
//            currentRotationTime = runRotationTime;
//        }
//        else
//        {
//            currentRotationTime = normalRotationTime;
//        }
//    }
//    /// <summary>
//    /// ����Pose
//    /// </summary>
//    private void UpdatePoseStation()
//    {

//        if (!isOnGround)//�ڿ���
//        {
//            poseStation = PoseStation.MidAir;

//            if (CharacterInputSystem.MainInstance.Crouch)//�½�
//            {

//            }
//        }
//        else if (CharacterInputSystem.MainInstance.Crouch)
//        {
//            poseStation = PoseStation.Crouch;
//        }
//        else if (characterAnimator.AnimationAtTag("ATK") || characterAnimator.AnimationAtTag("RushATK") || characterAnimator.AnimationAtTag("Skill"))
//        {
//            poseStation = PoseStation.ATK;
//        }
//        else
//        {
//            poseStation = PoseStation.Stand;
//        }
//    }
//private void SetPoseStationValue()
//{
//    switch (poseStation)
//    {
//        case PoseStation.MidAir:
//            characterAnimator.SetFloat(AnimatorID.PoseStationID, midAirThreshold,0.15f,Time.deltaTime);
//            break;
//        case PoseStation.Crouch:
//            characterAnimator.SetFloat(AnimatorID.PoseStationID, crouchThreshold, 0.25f, Time.deltaTime);
//            break;
//        case PoseStation.Stand:
//            characterAnimator.SetFloat(AnimatorID.PoseStationID, standThreshold, 0.05f, Time.deltaTime);
//            break;
//        case PoseStation.ATK:
//            characterAnimator.SetFloat(AnimatorID.PoseStationID, standThreshold, 0.05f, Time.deltaTime);
//            moveStation = MoveStation.Idle;
//            break;


//    }
//}
/// <summary>
/// ����MoveStation��ֵ
/// </summary>
//private void UpdateMoveStation()
//{

//    if (poseStation == PoseStation.Stand || poseStation == PoseStation.Aim)
//    {


//        if (characterAnimator.AnimationAtTag("Dodge"))
//        {
//            characterAnimator.SetBool(AnimatorID.HasInputForStopID, true);

//            moveStation = MoveStation.Run;
//        }
//        else if (characterAnimator.GetBool(AnimatorID.HasMoveInputID))
//        {
//            toIdleBufferDeltaTimer = toIdleBufferTimer;
//            characterAnimator.SetBool(AnimatorID.HasInputForStopID, true);

//            if (moveStation != MoveStation.Run)
//            {
//                moveStation = MoveStation.Walk;
//                walkToRunDeltaTimer -= Time.deltaTime;
//                if (walkToRunDeltaTimer <= 0)
//                {
//                    walkToRunDeltaTimer = walkToRunTimer;
//                    moveStation = MoveStation.Run;
//                }

//            }
//        }
//        else if (!characterAnimator.GetBool(AnimatorID.HasMoveInputID))
//        {
//            {
//                toIdleBufferDeltaTimer -= Time.deltaTime;
//                if (toIdleBufferDeltaTimer <= 0)
//                {
//                    characterAnimator.SetBool(AnimatorID.HasInputForStopID, false);
//                    moveStation = MoveStation.Idle;
//                    toIdleBufferDeltaTimer = toIdleBufferTimer;
//                    walkToRunDeltaTimer = walkToRunTimer;

//                }
//            }

//        }
//    }


//}

#region ����ϵͳ

//private void UpdateDodge()
//{
//    if (CharacterInputSystem.MainInstance.Run)
//    {
//        if (characterAnimator.GetBool(AnimatorID.HasMoveInputID))
//        {
//            //ToDo��������

//            ExecuteDodge();
//        }
//        else
//        {
//            //����ԭ�ص�����
//           // moveStation = MoveStation.Run;
//           // GameEventsManager.MainInstance.CallEvent("����Idle��̹���������");
//            ExecuteDodgeInPlace();

//        }


//    }
//}
//private void ExecuteDodgeInPlace()
//{
//    if (!hasDodge)
//    {
//        characterAnimator.CrossFadeInFixedTime("Dodge_Back", 0.1f, 0);

//        TimerManager.MainInstance.GetOneTimer(dodgeColdTime, ResetDodge);
//        hasDodge = true;


//    }
//}
//private void ExecuteDodge()
//{
//    if (!hasDodge)
//    {
//        characterAnimator.CrossFadeInFixedTime("Dodge_Front", 0.1f, 0);
//        TimerManager.MainInstance.GetOneTimer(dodgeColdTime, ResetDodge);
//        hasDodge = true;
//    }


//}

#endregion

//private void SetAnimationValue()
//{
//    characterAnimator.SetBool(AnimatorID.HasInputID, CharacterInputSystem.MainInstance.PlayerMove != Vector2.zero || poseStation == PoseStation.Crouch);
//    characterAnimator.SetBool(AnimatorID.HasMoveInputID, CharacterInputSystem.MainInstance.PlayerMove != Vector2.zero);
//    if (poseStation == PoseStation.Stand ||poseStation==PoseStation.Aim)
//    {
//        switch (moveStation)
//        {
//            case MoveStation.Idle:
//                characterAnimator.SetFloat(AnimatorID.MovementID, 0, 0.1f, Time.deltaTime);
//                if (characterAnimator.GetFloat(AnimatorID.MovementID) < 1.5)
//                {
//                    characterAnimator.SetBool(AnimatorID.RunID, false);
//                }
//                break;
//            case MoveStation.Walk:
//                characterAnimator.SetFloat(AnimatorID.MovementID,characterAnimator.GetBool(AnimatorID.RunID)? CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude * 3 : CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude * 2, 0.6f, Time.deltaTime);

//                break;
//            case MoveStation.Run:
//                characterAnimator.SetBool(AnimatorID.RunID, true);
//                characterAnimator.SetFloat(AnimatorID.MovementID, characterAnimator.GetBool(AnimatorID.RunID) ? CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude * 3 : CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude * 2, 0.5f, Time.deltaTime);
//                break;
//        }
//    }
//    else if (poseStation==PoseStation.ATK)
//    {
//        characterAnimator.SetFloat(AnimatorID.MovementID, 0, 0.1f, Time.deltaTime);
//        if (characterAnimator.GetFloat(AnimatorID.MovementID) < 1)
//        {
//            characterAnimator.SetBool(AnimatorID.RunID, false);
//        }
//    }
//    else if (poseStation == PoseStation.Crouch)
//    {
//        characterAnimator.SetFloat(AnimatorID.MovementID,  CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude * 2, 0.35f, Time.deltaTime);
//        characterAnimator.SetBool(AnimatorID.RunID, false);
//    }
//}

//private void ResetDodge()
//{
//    hasDodge = false;
//}


//public CharacterName CharacterName => characterName;

//}