


//public class CharacterComboBase : MonoBehaviour
//{
//    [SerializeField, Header("����")] public static Transform enemy;
//    [SerializeField, Header("�ṥ������")] protected ComboContainerData lightCombo;
//    [SerializeField, Header("�ع�������")] protected ComboContainerData heavyCombo;
//    [SerializeField, Header("��̹���")] protected ComboContainerData runCombo;
//    [SerializeField, Header("����")] protected ComboContainerData executeCombo;
//    [SerializeField, Header("����")] protected ComboData skillCombo;
//    [SerializeField, Header("�ռ�����")] protected ComboData finishSkillCombo;
//    [SerializeField, Header("��֡��ʱ��(��)")] protected float pauseFrameTime;

//    protected ComboData currentComboData;
//    protected ComboContainerData currentCombo;
//    protected int comboIndex;
//    protected int currentIndex;//��ֹ��Ϊindex���µ���ATKת��index���ֲ���Ӧ����ֵ
//    [SerializeField] protected bool canInput = true;//��Ϊ��ʱ����ƻᵼ�¶�����֡�Ĵ������Ըĳɶ����¼�����
//    private bool canATK = true;
//    private bool hasATKCommand;
//    protected float currentComboColdTime;
//    private bool canConnect;
//    private bool canMoveInterrupt;

//    protected Animator animator;
//   // protected CharacterMoveController moveController;
//    protected int executeIndex;

//    protected virtual void Update()
//    {
//        //MoveInterrupt();
//        //CheckCanConnectCombo();
//        //UpdateAttackLookAtEnemy();
//        // UpdateComboAnimation();
//    }
//    protected virtual void Awake()
//    {

//        animator = GetComponent<Animator>();
//        moveController = GetComponent<CharacterMoveController>();
//    }
//    protected virtual void Start()
//    {

//        ReSetComboInfo();
//    }

//    protected virtual bool CanBaseComboInput()
//    {
//        if (!canInput) { return false; }
//        if (animator.AnimationAtTag("Hit")) return false;
//        if (animator.AnimationAtTag("Parry")) return false;
//        if (animator.AnimationAtTag("Execute")) return false;
//        if (animator.AnimationAtTag("Skill")) { return false; }

//        return true;
//    }
//    protected virtual bool CanRunComboInput()
//    {
//        if ((animator.AnimationAtTag("Dodge")) && canInput)
//        {
//            return true;

//        }
//        return false;
//    }
//    protected virtual void BaseComboInput()
//    {
//        if (CharacterInputSystem.MainInstance.L_Atk)
//        {
//            if (!CanBaseComboInput()) return;
//            if (!CanRunComboInput())
//            {
//                if (currentCombo == runCombo)
//                {
//                    currentCombo = lightCombo;

//                }
//                else if (currentCombo != runCombo && (currentCombo == null || currentCombo != lightCombo))
//                {

//                    currentCombo = lightCombo;
//                    ReSetComboInfo();
//                }

//            }
//            else//���У���A
//            {
//                if (currentCombo == null || currentCombo != runCombo)
//                {
//                    currentCombo = runCombo;
//                    ReSetComboInfo();
//                }

//            }

//            ExecuteBaseCombo();
//        }
//        else if (CharacterInputSystem.MainInstance.R_Atk)
//        {
//            if (!CanBaseComboInput()) return;
//            if (currentCombo == null || currentCombo != heavyCombo)
//            {
//                currentCombo = lightCombo;
//                ReSetComboInfo();
//            }
//            ExecuteBaseCombo();
//        }

//    }


//    protected virtual void ExecuteInput()
//    {

//    }

//    protected virtual void ExecuteSpecialATK()
//    {

//    }
//    protected virtual void ExecuteBaseCombo()
//    {
//        hasATKCommand = true;
//        canInput = false;
//    }
//    protected virtual void UpdateComboAnimation()
//    {
//        if (!canATK) { return; }
//        if (!hasATKCommand) { return; }

//        string comboName = currentCombo.GetComboName(comboIndex);
//        currentIndex = comboIndex;
//        if (currentCombo == runCombo)
//        {
//            comboName = currentCombo.GetComboName(0);
//            currentIndex = 0;
//        }
//        animator.CrossFadeInFixedTime(comboName, 0.2f, 0);

//        UpdateComboInfo();
//        hasATKCommand = false;
//        canATK = false;
//    }

//    protected virtual void ExecuteRunCombo()
//    {

//    }

//    protected virtual void UpdateComboInfo()
//    {
//        comboIndex++;
//        if (comboIndex > currentCombo.GetComboMaxCount() - 1)
//        {
//            if (currentCombo != runCombo)
//            {
//                comboIndex = 0;
//            }
//        }
//        canMoveInterrupt = false;

//        canConnect = true;

//    }
//    protected virtual void ReSetComboInfo()
//    {

//        comboIndex = 0;
//        canInput = true;
//        canConnect = true;
//        canMoveInterrupt = false;
//        canATK = true;
//    }

//    #region �����¼�
//    public void DisConnectCombo()//�¼�����
//    {
//        canConnect = false;
//    }
//    public void CanMoveInterrupt()
//    {
//        canMoveInterrupt = true;
//    }

//    public void CanInput()
//    {
//        canInput = true;

//    }
//    public void CanATK()
//    {
//        canATK = true;
//    }

//    #endregion
//    public void PlayComboFX()
//    {
//        SFX_PoolManager.MainInstance.TryGetSoundPool(currentCombo.GetComboSoundStyle(currentIndex), transform.position, Quaternion.identity);
//    }

//    //ע��ת���˺��Ķ����¼�
//    public void ATK()
//    {

//        //AttackTrigger();


//    }
//  //  #region �˺����
//    protected bool AttackDetection(ComboContainerData comboContainerData)
//    {
//        //����
//        //����
//        //�Ƕ�
//        if (enemy == null) { return false; }

//        if (DevelopmentToos.DistanceForTarget(enemy, transform) > comboContainerData.GetComboDistance(currentIndex)) { return false; }
//        if (DevelopmentToos.GetAngleForTargetDirection(enemy, transform) < 80) { return false; }
//        return true;

//    }
//    protected bool SkillDetection(ComboData comboData)
//    {
//        if (enemy == null) { return false; }

//        if (DevelopmentToos.DistanceForTarget(enemy, transform) > comboData.attackDistance) { return false; }
//        if (DevelopmentToos.GetAngleForTargetDirection(enemy, transform) < 135) { return false; }
//        return true;

//    }

////    protected int UpdateExecuteIndex(ComboContainerData containerData)
//    //{
//   //  return Random.Range(0, containerData.GetComboMaxCount()); 
//    }
//   // private void AttackTrigger()
//    {
//        //if (animator.AnimationAtTag("ATK") || animator.AnimationAtTag("RushATK"))//����ͨ���������˺��Ϳ��ܶ����������ܻ�����
//        //{
//        //    CameraHitFeel.MainInstance.CameraShake(currentCombo.GetComboShakeForce(currentIndex));
//        //    if (!AttackDetection(currentCombo)) { return; }
//        //    GameEventsManager.MainInstance.CallEvent("�����˺�",
//        //      currentCombo.GetComboDamage(currentIndex),
//        //      currentCombo.GetComboHitName(currentIndex),
//        //      currentCombo.GetComboParryName(currentIndex),
//        //      transform, enemy);
//        //    CameraHitFeel.MainInstance.PF(pauseFrameTime);


//            //
//            //}
//        else if (animator.AnimationAtTag("Skill"))
//        {
//            if (!SkillDetection(currentSkill)) { return; }
//            GameEventsManager.MainInstance.CallEvent("�����˺�", currentSkill.comboDamage, currentSkill.hitName, currentSkill.parryName, transform, enemy);

//        }
//        else//����ֻ��һ���ܻ����������ǿ����ж���˺�
//        {
//            if (!AttackDetection(executeCombo)) { return; }
//            GameEventsManager.MainInstance.CallEvent("�����˺�", executeCombo.GetComboDamage(executeIndex));
//        }


//    }
//    #endregion
//    private void UpdateAttackLookAtEnemy()
//    {
//        if (enemy == null) { return; }
//        if (animator.AnimationAtTag("ATK") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.35f)
//        {
//            if (DevelopmentToos.DistanceForTarget(transform, enemy) > 5f) return;
//            if (DevelopmentToos.DistanceForTarget(transform, enemy) < 0.4f) return;
//            transform.Look(enemy.position, 50);
//        }

//    }

//    private void MoveInterrupt()
//    {
//        if (canMoveInterrupt == false){ return; }
//        if (CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude != 0)
//        {
//            animator.CrossFadeInFixedTime("Locomotion", 0.15f, 0);
//            canMoveInterrupt = false;
//        }
//    }
//    private void CheckCanConnectCombo()
//    {
//        if (!canConnect||CharacterInputSystem.MainInstance.Run)
//        {      
//            ReSetComboInfo();
//        }
//    }


//
//}