//
// Copyright (C) Microsoft. All rights reserved.
//

using System;
using System.IO;
using System.Collections;
using UnityEngine;

#if !UNITY_EDITOR
using Windows.Storage;
#endif

public class MovieTexturePlayer : MonoBehaviour
{
    public string TargetMovie = "vrt_expanded_ui_FG_MG_Combo_30fps_512x650.ogg";
    public Renderer[] TargetRenderers;

    private string m_moviePath;
    private MovieTexture m_movieTexture;

    private void Awake()
    {

#if UNITY_EDITOR
        m_moviePath = Application.streamingAssetsPath + "/";
#else
        m_moviePath = UnityEngine.Application.dataPath + "/StreamingAssets/";
#endif
        m_moviePath += TargetMovie;
    }

    private void Start()
    {
        StartCoroutine("LoadMovie", m_moviePath);
    }

    private IEnumerator LoadMovie(string path)
    {
        path = "file://" + path;

        WWW movieStream = new WWW(path);

        m_movieTexture = movieStream.movie;
        m_movieTexture.wrapMode = TextureWrapMode.Clamp;
        m_movieTexture.loop = true;

        // Wait for the movie to be ready to play
        while (!m_movieTexture.isReadyToPlay)
        {
            yield return new WaitForEndOfFrame();
        }

        // Iterate through the list of renderers and apply the streamed texture
        for (int i = 0; i < TargetRenderers.Length; i++)
        {
            TargetRenderers[i].material.mainTexture = m_movieTexture;
        }
 
        m_movieTexture.Play();
        movieStream.Dispose();
    }
}
