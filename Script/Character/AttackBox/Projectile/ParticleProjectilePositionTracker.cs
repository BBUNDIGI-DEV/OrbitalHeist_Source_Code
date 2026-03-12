using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleProjectilePositionTracker : MonoBehaviour
{
    public Vector3 Pos
    {
        get; private set;
    }

    private ParticleSystem mPS;
    private Particle[] mBufferedParticles = new Particle[64]; 

    public void UpdatePos()
    {
        if(mPS == null)
        {
            mPS = GetComponent<ParticleSystem>();
        }

        int count = mPS.GetParticles(mBufferedParticles);
        if(count <= 0)
        {
            Pos = Vector3.zero;
        }

        Pos = mBufferedParticles[0].position;
    }

    public void ClearProjectile()
    {
        mPS.Clear(true);
        Pos = Vector3.zero;
    }

    public void SetParticleProjectileSpeed(float speed)
    {
        //Unity Docs น฿ร้
        //Particle System modules do not need to be reassigned back to the system; they are interfaces(maybe properties) and not independent objects.
        MainModule mainOption = GetComponent<ParticleSystem>().main;
        mainOption.startSpeed = speed;
    }
}
