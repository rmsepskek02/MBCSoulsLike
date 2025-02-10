using UnityEngine;
using UnityEngine.AI;

namespace BS.Enemy.Set
{
    /// <summary>
    /// 
    /// </summary>
    public class SetProperty
    {
        public SetController Controller { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        public Transform Player { get; private set; }
        public Animator Animator { get; private set; }

        public float CloseRange { get; private set; } = 3.3f;   // 근접 거리
        public float MidRange { get; private set; } = 10f;    // 중거리
        public float LongRange { get; private set; } = 14f;  // 장거리

        public float LastAttackTime { get; set; }           //공격한 시간을 체크
        public string LastAttackType { get; set; }


        public const string SET_ANIM_TRIGGER_SLASHATTACK = "SlashAttack";
        public const string SET_ANIM_TRIGGER_SLASHATTACKTHREETIMES = "SlashAttackThreeTimes";
        public const string SET_ANIM_TRIGGER_PULLATTACK = "PullAttack";
        public const string SET_ANIM_TRIGGER_ROAR = "Roar";
        public const string SET_ANIM_TRIGGER_LIGHTNINGMAGIC = "LightningMagic";
        public const string SET_ANIM_BOOL_DEAD = "IsDead";
        public const string SET_ANIM_BOOL_FAINTING = "IsFainting";
        public const string SET_ANIM_BOOL_ATTACK = "IsAttack";
        public const string SET_ANIM_BOOL_IDLE = "IsIdle";
        public const string SET_ANIM_BOOL_CHASE = "IsChase";
        public const string PLAYER_LAYER = "Player";
        public const string OBSTACLE_LAYER = "Wall";


        public SetProperty(SetController controller, Animator animator, NavMeshAgent agent, Transform player)
        {
            Controller = controller;
            Agent = agent;
            Player = player;
            Animator = animator;
            LastAttackTime = Time.time;
        }
    }
}