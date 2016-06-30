using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootstepsHandler : MonoBehaviour
{
    [System.Serializable]
    public class FootSurfaceTypes
    {

        public Vector2 RandomPitch = new Vector2(1.0f, 1.5f); // random pitch range for footsteps
        public bool Foldout = true; // used by the editor to allow folding this surface type
        public bool WalkSoundsFoldout = true; // used by the editor to allow folding the sounds section
        public bool RunSoundsFoldout = true; // used by the editor to allow folding the sounds section
        public bool TexturesFoldout = true; // used by the editor to allow folding the textures section
        public string SurfaceName = ""; // Name of the surface for reference in the editor
        public List<AudioClip> WalkSounds = new List<AudioClip>(); // List of sounds to play randomly
        public List<AudioClip> RunSounds = new List<AudioClip>(); // List of sounds to play randomly
        public List<Texture> Textures = new List<Texture>(); // list of the textures for this surface
    }
    public List<FootSurfaceTypes> SurfaceTypes = new List<FootSurfaceTypes>(); // list of all the surfaces created

    public GameObject footstepsSource;
    public float audioStepLengthCrouch = 0.7f;
    public float audioStepLengthWalk = 0.45f;
    public float audioStepLengthRun = 0.25f;
    public float minWalkSpeed = 2.5f;
    public float maxWalkSpeed = 8.0f;
    private bool step = true;
    public CharacterController controller;
    private PlayerController player;
    private float speed, nextFootstepTime;

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        player = this.GetComponent<PlayerController>();
        step = true;
    }

    Texture GetTexture(ControllerColliderHit hit)
    {
        if (!hit.collider.gameObject.GetComponent<MeshRenderer>())
            return null;
        return hit.collider.gameObject.GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_MainTex");
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!player.canMove) return;
        speed = player.velMagnitude;

        if (player.grounded && hit.normal.y >= .8f && Time.time > nextFootstepTime)
        {
            if (speed > 0.1f)
                foreach (FootSurfaceTypes st in SurfaceTypes)
                    for (int i = 0; i < st.Textures.Count; i++)
                    {
                        if (st.Textures[i] == GetTexture(hit))
                        {
                            CalculateSound(st, i);
                            break;
                        }
                        print(i);
                        if (i == st.Textures.Count - 1)
                        {
                            CalculateSound(SurfaceTypes[0], 0);
                            break;
                        }
                    }
        }
    }

    void CalculateSound(FootSurfaceTypes fst, int iType)
    {
        print("ASD");
        step = false;
        if (fst.WalkSounds == null || fst.WalkSounds.Count == 0)
            return;

        if (fst.RunSounds == null || fst.RunSounds.Count == 0)
            return;

        if (player.inputY == 0 && !player.grounded)
            return;

        AudioClip clipToPlay = null;
        string typ = "";

        if (player.state == 1)
        {
            if (speed > 0.1f)
            {
                clipToPlay = fst.WalkSounds[Random.Range(0, fst.WalkSounds.Count)];
                nextFootstepTime = Time.time + audioStepLengthCrouch;
                typ = "Crouch";
            }
        }
        else
        {
            if (speed >= minWalkSpeed && speed < maxWalkSpeed)
            {
                clipToPlay = fst.WalkSounds[Random.Range(0, fst.WalkSounds.Count)];
                nextFootstepTime = Time.time + audioStepLengthWalk;
                typ = "Walk";
            }
            if (speed >= maxWalkSpeed + 0.1f)
            {
                clipToPlay = fst.RunSounds[Random.Range(0, fst.RunSounds.Count)];
                nextFootstepTime = Time.time + audioStepLengthRun;
                typ = "Run";
            }
        }

        if (clipToPlay == null)
        {
            step = true;
            return;
        }

        StartCoroutine(PlayFinalSound(typ, clipToPlay, iType));
    }

    IEnumerator PlayFinalSound(string stepType, AudioClip playSound, int iType)
    {
        step = false;
        float waitStepTime = 0, pitch = Random.Range(SurfaceTypes[iType].RandomPitch.x, SurfaceTypes[iType].RandomPitch.y) * Time.timeScale;

        switch (stepType)
        {
            case "Crouch":
                PlayFootstep(playSound, footstepsSource.transform.position, 0.015f, 0.5f, 1f, pitch);
                waitStepTime = audioStepLengthCrouch;
                break;

            case "Walk":
                PlayFootstep(playSound, footstepsSource.transform.position, 0.1f, 1f, 5f, pitch);
                waitStepTime = audioStepLengthWalk;
                break;

            case "Run":
                PlayFootstep(playSound, footstepsSource.transform.position, 0.25f, 1.5f, 7f, pitch);
                waitStepTime = audioStepLengthRun;
                break;
        }

        yield return new WaitForSeconds(waitStepTime);

        step = true;
    }

    void PlayFootstep(AudioClip clip, Vector3 pos, float volume, float mindist, float maxdist, float pitch)
    {
        GameObject go = new GameObject("Footstep " + clip.name);
        go.transform.position = pos;

        AudioSource source = go.AddComponent<AudioSource>();
        source.minDistance = mindist;
        source.maxDistance = maxdist;
        source.pitch = pitch;
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(go, source.clip.length);
    }
}
