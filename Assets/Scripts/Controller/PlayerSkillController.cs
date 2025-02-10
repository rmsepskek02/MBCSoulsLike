using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

namespace BS.Player
{
    public struct Skill
    {
        public string Name;          // 스킬 이름
        public float coolTime;   // 쿨타임
        public float damage;
        public UnityAction Action;   // 스킬 실행 시 호출할 액션

        // Skill 생성자
        public Skill(string name, float coolTime, UnityAction action, float damage = 0f)
        {
            Name = name;
            this.coolTime = coolTime;
            this.damage = damage;
            Action = action;
        }
    }
    public class PlayerSkillController : PlayerController
    {
        #region Variables
        public Dictionary<string, Skill> skillList = new Dictionary<string, Skill>();

        // 스킬 쿨타임 UI 요소를 관리할 Dictionary (각 스킬 이름 -> UI 요소)
        public Dictionary<string, Image> skillCoolTimeUI = new Dictionary<string, Image>(); // fillAmount를 조정할 UI Image
        public Dictionary<string, TextMeshProUGUI> skillCoolTimeTextUI = new Dictionary<string, TextMeshProUGUI>(); // 쿨타임 숫자를 표시할 UI Text

        // 쿨타임을 관리할 딕셔너리 (각 스킬에 대해 코루틴을 관리)
        private Dictionary<string, Coroutine> skillCoolTimeCoroutines = new Dictionary<string, Coroutine>();

        public Image UppercutCoolTimeImage;
        public TextMeshProUGUI UppercutCoolTimeText;
        public Image backHandSwingCoolTimeImage;
        public TextMeshProUGUI backHandSwingCoolTimeText;
        public Image chargingPunchCoolTimeImage;
        public TextMeshProUGUI chargingPunchCoolTimeText;
        public Image blockCoolTimeImage;
        public TextMeshProUGUI blockCoolTimeText;
        public Image dashCoolTimeImage;
        public TextMeshProUGUI dashCoolTimeText;
        #endregion

        protected override void Start()
        {
            base.Start();

            // 스킬에 대한 UI 요소를 초기화 (예시: UI 오브젝트 연결)
            // Dash 스킬의 쿨타임 UI 연결
            if (dashCoolTimeImage != null)
                skillCoolTimeUI.Add("Dash", dashCoolTimeImage); // dashCoolTimeImage는 UI Image 컴포넌트
            if (dashCoolTimeText != null)
                skillCoolTimeTextUI.Add("Dash", dashCoolTimeText); // dashCoolTimeText는 UI Text 컴포넌트

            // Block 스킬의 쿨타임 UI 연결
            if (blockCoolTimeImage != null)
                skillCoolTimeUI.Add("Block", blockCoolTimeImage); // blockCoolTimeImage는 UI Image 컴포넌트
            if (blockCoolTimeText != null)
                skillCoolTimeTextUI.Add("Block", blockCoolTimeText); // blockCoolTimeText는 UI Text 컴포넌트

            // Uppercut 스킬의 쿨타임 UI 연결
            if (UppercutCoolTimeImage != null)
                skillCoolTimeUI.Add("Uppercut", UppercutCoolTimeImage); // uppercutCoolTimeImage는 UI Image 컴포넌트
            if (UppercutCoolTimeText != null)
                skillCoolTimeTextUI.Add("Uppercut", UppercutCoolTimeText); // uppercutCoolTimeText는 UI Text 컴포넌트

            // BackHandSwing 스킬의 쿨타임 UI 연결
            if (backHandSwingCoolTimeImage != null)
                skillCoolTimeUI.Add("BackHandSwing", backHandSwingCoolTimeImage); // backHandSwingCoolTimeImage UI Image 컴포넌트
            if (backHandSwingCoolTimeText != null)
                skillCoolTimeTextUI.Add("BackHandSwing", backHandSwingCoolTimeText); // backHandSwingCoolTimeText UI Text 컴포넌트

            // ChargingPunch 스킬의 쿨타임 UI 연결
            if (chargingPunchCoolTimeImage != null)
                skillCoolTimeUI.Add("ChargingPunch", chargingPunchCoolTimeImage); // chargingPunchCoolTimeImage UI Image 컴포넌트
            if (chargingPunchCoolTimeText != null)
                skillCoolTimeTextUI.Add("ChargingPunch", chargingPunchCoolTimeText); // chargingPunchCoolTimeText UI Text 컴포넌트
        }

        protected void OnEnable()
        {
            // 각 스킬에 대해 초기 쿨타임 설정
            foreach (var skill in skillList)
            {
                skillCoolTimeCoroutines[skill.Value.Name] = null; // 초기화
            }
            foreach (var skill in skillList.Keys)
            {
                var action = m_Input.playerActionMap.FindAction(skill);
                if (action != null)
                {
                    action.performed += OnSkillInput;
                }
            }
        }

        protected void OnDisable()
        {
            foreach (var skill in skillList.Keys)
            {
                var action = m_Input.playerActionMap.FindAction(skill);
                if (action != null)
                {
                    action.performed -= OnSkillInput;
                }
            }
        }

        // 스킬 입력 처리
        private void OnSkillInput(InputAction.CallbackContext context)
        {
            if (animator.GetBool("IsAttacking")) return;
            string key = context.action.name;
            if (skillList.ContainsKey(key))
            {
                // qwe 공격 스킬 시전 중 다른 스킬을 불가
                if ((key == "Q" || key == "W" || key == "E") && !ps.canSkill) { return; }
                // qwe 공격 스킬 시전 중 다른 행동이 불가되었을 경우
                if (!ps.isDashable && !ps.isBlockable) return;
                ExecuteSkill(key);
            }
        }

        private void ExecuteSkill(string key)
        {
            if (skillList.TryGetValue(key, out var skill))
            {
                // 스킬 사용 가능 여부 체크
                if (!CanUseSkill(skill.Name)) return;

                // 쿨타임이 끝났는지 확인
                if (skillCoolTimeCoroutines[skill.Name] == null) // 아직 쿨타임이 돌고 있지 않으면
                {
                    skill.Action?.Invoke(); // 스킬 실행
                    ps.currentSkillName = key;
                    ps.prevTransform = this.transform;
                    skillCoolTimeCoroutines[skill.Name] = StartCoroutine(CoolTimeCoroutine(skill.Name, skill.coolTime, key)); // 쿨타임 코루틴 시작
                }
            }
        }

        // 쿨타임을 처리하는 코루틴
        private IEnumerator CoolTimeCoroutine(string skillName, float cooldownDuration, string skillKey)
        {
            float cooldownTimeLeft = cooldownDuration;

            // 쿨타임 동안 UI 갱신
            while (cooldownTimeLeft > 0f)
            {
                cooldownTimeLeft -= Time.deltaTime;

                // 쿨타임 UI 업데이트 (Image UI를 사용해 네모난 배경에 채워지기)
                if (skillCoolTimeUI.ContainsKey(skillName))
                {
                    float fillAmount = cooldownTimeLeft / cooldownDuration;
                    skillCoolTimeUI[skillName].fillAmount = fillAmount; // 네모난 이미지의 진행 상태 표시
                }

                // 쿨타임 숫자 업데이트 (Text UI에 소수점 한자리까지 남은 시간 표시)
                if (skillCoolTimeTextUI.ContainsKey(skillName))
                {
                    skillCoolTimeTextUI[skillName].text = $"{Mathf.Max(0, cooldownTimeLeft):F1}"; // 소수점 1자리까지 표시
                }

                yield return null; // 한 프레임 대기
            }

            skillCoolTimeCoroutines[skillName] = null; // 쿨타임이 끝났으면 코루틴을 null로 설정하여 다시 사용할 수 있도록

            // 쿨타임 끝나면 UI 초기화
            if (skillCoolTimeUI.ContainsKey(skillName))
            {
                skillCoolTimeUI[skillName].fillAmount = 0f; // UI의 쿨타임이 끝났으므로 채움
            }

            if (skillCoolTimeTextUI.ContainsKey(skillName))
            {
                if (skillKey == "Space") skillKey = "Sp";
                skillCoolTimeTextUI[skillName].text = skillKey; // 쿨타임 끝나면 "Ready"로 표시
            }

            //Debug.Log($"{skillName} is ready to use.");
        }
        // 스킬 사용 가능 여부를 검사하는 함수
        private bool CanUseSkill(string skillName)
        {
            switch (skillName)
            {
                case "Dash":
                    return animator.GetBool("IsDashing") == false
                        && animator.GetBool("IsBlocking") == false
                        && ps.isDashable;
                default:
                    return true; // 기본적으로 다른 스킬은 실행 가능
            }
        }
    }
}
