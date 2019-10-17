using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roundbeargames_tutorial
{

    public class AttackInfo : MonoBehaviour
    {
        public CharacterControl Attacker = null;
        public Attack AttackAbility;
        public List<string> ColliderNames = new List<string>();
        public bool MustCollide;
        public bool MustFaceAttacker;
        public float LethalRange;
        public int MaxHits;
        public int CurrentHits;
        public bool IsRegistered;
        public bool IsFinished;

        public void ResetInfo(Attack attack)
        {
            IsRegistered = false;
            IsFinished = false;
            AttackAbility = attack;
        }

        public void Register(Attack attack, CharacterControl attacker)
        {
            IsRegistered = true;
            Attacker = attacker;

            AttackAbility = attack;
            ColliderNames = attack.ColliderNames;
            MustCollide = attack.MustCollide;
            MustFaceAttacker = attack.MustFaceAttacker;
            LethalRange = attack.LethalRange;
            MaxHits = attack.MaxHits;
            CurrentHits = 0;
        }
    }
}