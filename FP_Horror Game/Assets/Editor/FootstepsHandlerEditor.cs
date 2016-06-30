using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(FootstepsHandler))]

public class FootstepsHandlerEditor : Editor
{
    // target component
    public FootstepsHandler m_Component;

    // foldouts
    public static bool m_SurfaceTypesFoldout;

    protected static GUIStyle m_HeaderStyle = null;
    protected static GUIStyle m_SmallButtonStyle = null;

    static public Texture2D blankTexture
    {
        get
        {
            return EditorGUIUtility.whiteTexture;
        }
    }

    public static GUIStyle SmallButtonStyle
    {
        get
        {
            if (m_SmallButtonStyle == null)
            {
                m_SmallButtonStyle = new GUIStyle("Button");
                m_SmallButtonStyle.fontSize = 10;
                m_SmallButtonStyle.alignment = TextAnchor.MiddleCenter;
                m_SmallButtonStyle.margin.left = 1;
                m_SmallButtonStyle.margin.right = 1;
                m_SmallButtonStyle.padding = new RectOffset(0, 4, 0, 2);
            }
            return m_SmallButtonStyle;
        }
    }


    public static GUIStyle HeaderStyleSelected
    {
        get
        {
            if (m_HeaderStyle == null)
            {
                m_HeaderStyle = new GUIStyle("Foldout");
                m_HeaderStyle.fontSize = 11;
                m_HeaderStyle.fontStyle = FontStyle.Bold;

            }
            return m_HeaderStyle;
        }
    }

    public virtual void OnEnable()
    {
        m_Component = (FootstepsHandler)target;
    }

    public override void OnInspectorGUI()
    {

        GUI.color = Color.white;

        DoSurfaceTypesFoldout();

        // update
        if (GUI.changed)
        {

            EditorUtility.SetDirty(target);

        }

    }

    public virtual void DoSurfaceTypesFoldout()
    {
        if (m_Component.SurfaceTypes != null)
        {
            for (int i = 0; i < m_Component.SurfaceTypes.Count; ++i)
            {
                FootstepsHandler.FootSurfaceTypes surface = m_Component.SurfaceTypes[i];

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                surface.Foldout = EditorGUILayout.Foldout(surface.Foldout, surface.SurfaceName);
                if (i > 0)
                {
                    if (GUILayout.Button("^", GUILayout.MinWidth(15), GUILayout.MaxWidth(15), GUILayout.MinHeight(15)))
                    {
                        int newIndex = i - 1;
                        List<FootstepsHandler.FootSurfaceTypes> newSurfaces = new List<FootstepsHandler.FootSurfaceTypes>();
                        for (int x = 0; x < m_Component.SurfaceTypes.Count; x++)
                        {
                            FootstepsHandler.FootSurfaceTypes surf = m_Component.SurfaceTypes[x];
                            if (x == newIndex)
                            {
                                newSurfaces.Add(surface);
                                newSurfaces.Add(surf);
                            }
                            else if (surf != surface)
                                newSurfaces.Add(surf);
                        }
                        m_Component.SurfaceTypes = newSurfaces;
                        return;
                    }
                    GUILayout.Space(5);
                }

                if (GUILayout.Button("Remove", GUILayout.MinWidth(60), GUILayout.MaxWidth(60), GUILayout.MinHeight(15)))
                {
                    m_Component.SurfaceTypes.RemoveAt(i);
                    --i;
                }
                GUI.backgroundColor = Color.white;

                GUILayout.Space(20);

                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (surface.Foldout)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(35);
                    if (surface.SurfaceName == "")
                        surface.SurfaceName = "Surface " + (i + 1);
                    surface.SurfaceName = EditorGUILayout.TextField("Surface Name", surface.SurfaceName, GUILayout.MaxWidth(250));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(35);
                    surface.RandomPitch = EditorGUILayout.Vector2Field("Random Pitch", surface.RandomPitch);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(38);

                    if (surface.WalkSoundsFoldout)
                        surface.WalkSoundsFoldout = EditorGUILayout.Foldout(surface.WalkSoundsFoldout, "Walk Sounds", HeaderStyleSelected);
                    else
                        surface.WalkSoundsFoldout = EditorGUILayout.Foldout(surface.WalkSoundsFoldout, "Walk Sounds");

                    if (surface.RunSoundsFoldout)
                        surface.RunSoundsFoldout = EditorGUILayout.Foldout(surface.RunSoundsFoldout, "Run Sounds", HeaderStyleSelected);
                    else
                        surface.RunSoundsFoldout = EditorGUILayout.Foldout(surface.RunSoundsFoldout, "Run Sounds");

                    GUILayout.EndHorizontal();

                    if (surface.WalkSoundsFoldout)
                    {
                        if (surface.WalkSounds != null)
                        {
                            if (surface.WalkSounds.Count > 0)
                            {
                                for (int x = 0; x < surface.WalkSounds.Count; ++x)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(50);
                                    surface.WalkSounds[x] = (AudioClip)EditorGUILayout.ObjectField("", surface.WalkSounds[x], typeof(AudioClip), false);
                                    if (surface.WalkSounds[x] == null)
                                        GUI.enabled = false;
                                    if (GUILayout.Button(">", GUILayout.MinWidth(15), GUILayout.MaxWidth(15), GUILayout.MinHeight(15)))
                                    {
                                        AudioSource audio = m_Component.transform.root.GetComponentInChildren<AudioSource>();
                                        if (audio != null)
                                            audio.PlayOneShot(surface.WalkSounds[x]);
                                    }
                                    GUI.enabled = true;
                                    if (GUILayout.Button("X", GUILayout.MinWidth(15), GUILayout.MaxWidth(15), GUILayout.MinHeight(15)))
                                    {
                                        surface.WalkSounds.RemoveAt(x);
                                        --x;
                                    }
                                    GUI.backgroundColor = Color.white;
                                    GUILayout.Space(20);

                                    GUILayout.EndHorizontal();
                                }
                            }
                        }

                        if (surface.WalkSounds.Count == 0)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(50);
                            EditorGUILayout.HelpBox("There are no sounds. Click the \"Add Walk Sound\" button to add a sound.", MessageType.Info);
                            GUILayout.Space(20);
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("+Walk Sound", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
                        {
                            AudioClip clip = new AudioClip();
                            surface.WalkSounds.Add(clip);
                        }
                        GUI.backgroundColor = Color.white;
                        GUILayout.EndHorizontal();

                    }

                    if (surface.RunSoundsFoldout)
                    {
                        if (surface.RunSounds != null)
                        {
                            if (surface.RunSounds.Count > 0)
                            {
                                for (int x = 0; x < surface.RunSounds.Count; ++x)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(50);
                                    surface.RunSounds[x] = (AudioClip)EditorGUILayout.ObjectField("", surface.RunSounds[x], typeof(AudioClip), false);
                                    if (surface.RunSounds[x] == null)
                                        GUI.enabled = false;
                                    if (GUILayout.Button(">", GUILayout.MinWidth(15), GUILayout.MaxWidth(15), GUILayout.MinHeight(15)))
                                    {
                                        AudioSource audio = m_Component.transform.root.GetComponentInChildren<AudioSource>();
                                        if (audio != null)
                                            audio.PlayOneShot(surface.RunSounds[x]);
                                    }
                                    GUI.enabled = true;
                                    if (GUILayout.Button("X", GUILayout.MinWidth(15), GUILayout.MaxWidth(15), GUILayout.MinHeight(15)))
                                    {
                                        surface.RunSounds.RemoveAt(x);
                                        --x;
                                    }
                                    GUI.backgroundColor = Color.white;
                                    GUILayout.Space(20);

                                    GUILayout.EndHorizontal();
                                }
                            }
                        }

                        if (surface.RunSounds.Count == 0)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(50);
                            EditorGUILayout.HelpBox("There are no sounds. Click the \"Add Run Sound\" button to add a sound.", MessageType.Info);
                            GUILayout.Space(20);
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("+Run Sound", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
                        {
                            AudioClip clip = new AudioClip();
                            surface.RunSounds.Add(clip);
                        }
                        GUI.backgroundColor = Color.white;
                        GUILayout.EndHorizontal();

                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(38);

                    if (surface.TexturesFoldout)
                        surface.TexturesFoldout = EditorGUILayout.Foldout(surface.TexturesFoldout, "Textures", HeaderStyleSelected);
                    else
                        surface.TexturesFoldout = EditorGUILayout.Foldout(surface.TexturesFoldout, "Textures");

                    GUILayout.EndHorizontal();

                    if (surface.TexturesFoldout)
                    {
                        if (surface.Textures != null)
                        {
                            if (surface.Textures.Count > 0)
                            {
                                int counter = 0;
                                for (int x = 0; x < surface.Textures.Count; ++x)
                                {
                                    if (counter == 0)
                                    {
                                        GUILayout.BeginHorizontal(GUILayout.MinHeight(100));
                                        GUILayout.Space(50);
                                    }

                                    GUILayout.BeginVertical(GUILayout.MinHeight(90));
                                    surface.Textures[x] = (Texture)EditorGUILayout.ObjectField(surface.Textures[x], typeof(Texture), false, GUILayout.MinWidth(50), GUILayout.MaxWidth(75), GUILayout.MinHeight(50), GUILayout.MaxHeight(75));

                                    if (GUILayout.Button("Delete", GUILayout.MinWidth(50), GUILayout.MaxWidth(75)))
                                    {
                                        surface.Textures.RemoveAt(x);
                                        --x;
                                    }
                                    GUI.backgroundColor = Color.white;
                                    GUILayout.EndVertical();

                                    counter++;

                                    if (counter == 4 || x == surface.Textures.Count - 1)
                                    {
                                        GUILayout.Space(20);

                                        GUILayout.EndHorizontal();
                                        counter = 0;
                                    }
                                }
                            }
                        }

                        if (surface.Textures.Count == 0)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(50);
                            EditorGUILayout.HelpBox("There are no textures. Click the \"Add Texture\" button to add a texture.", MessageType.Info);
                            GUILayout.Space(20);
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add Texture", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
                        {
                            Texture texture = new Texture();
                            surface.Textures.Add(texture);
                        }
                        GUI.backgroundColor = Color.white;
                        GUILayout.EndHorizontal();
                    }

                    DrawSeparator();

                    GUILayout.Space(5);
                }
            }
        }

        if (m_Component.SurfaceTypes.Count == 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            EditorGUILayout.HelpBox("There are no Surface Types. Click the \"Add Surface Type\" button to add a new surface type.", MessageType.Info);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(8f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Surface Type", GUILayout.MinWidth(150), GUILayout.MinHeight(25)))
        {
            FootstepsHandler.FootSurfaceTypes surface = new FootstepsHandler.FootSurfaceTypes();
            m_Component.SurfaceTypes.Add(surface);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        DrawSeparator();
    }

    static public void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 10f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 10f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 13f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

}
