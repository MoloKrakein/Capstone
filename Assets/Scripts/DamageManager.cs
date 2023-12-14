using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour{


public BattleFlow battleFlow;

public AudioSource SFXSource;
public AudioClip PhysicalAttack;
public AudioClip FireAttack;
public AudioClip EarthAttack;
public AudioClip DarknessAttack;
public AudioClip LightAttack;
public AudioClip GenericHitSound;


public ParticleSystem GenericHit;
public ParticleSystem FireHit;
public ParticleSystem EarthHit;
public ParticleSystem DarknessHit;
public ParticleSystem LightHit;

public ParticleSystem SummoningEffect;

public void PlayAttackSound(DmgType dmgType)
{
    switch (dmgType)
    {
        case DmgType.Physical:
            SFXSource.PlayOneShot(PhysicalAttack);
            break;
        case DmgType.Fire:
            SFXSource.PlayOneShot(FireAttack);
            break;
        case DmgType.Earth:
            SFXSource.PlayOneShot(EarthAttack);
            break;
        case DmgType.Darkness:
            SFXSource.PlayOneShot(DarknessAttack);
            break;
        case DmgType.Light:
            SFXSource.PlayOneShot(LightAttack);
            break;
        default:
            break;
    }

}

public void PlayHitEffect(DmgType dmgType)
{
    switch (dmgType)
    {
        case DmgType.Physical:
            GenericHit.Play();
            break;
        case DmgType.Fire:
            FireHit.Play();
            break;
        case DmgType.Earth:
            EarthHit.Play();
            break;
        case DmgType.Darkness:
            DarknessHit.Play();
            break;
        case DmgType.Light:
            LightHit.Play();
            break;
        default:
            break;
    }

}

public void PlayHitSoundEffect()
{
    SFXSource.PlayOneShot(GenericHitSound);
    // SFXSource.PlayOneShot(Summoning);  
}

}


