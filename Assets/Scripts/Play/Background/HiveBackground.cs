using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using HeathenEngineering.Events;
using UnityEngine.Rendering;
using System;

public class HiveBackground : MonoBehaviour
{
    public SpriteRenderer _Renderer;

    private void Update()
    {
        var camera = GameObject.Find("Player Camera").GetComponent<Camera>();
        var worldposition = camera.ScreenToWorldPoint(Input.mousePosition);

        var local = _Renderer.worldToLocalMatrix.MultiplyPoint(worldposition);
        var texture = _Renderer.sprite.texture; // 이 스프라이트는 단일 텍스쳐라고 가정
    
        local.x /= _Renderer.size.x;
        local.y /= _Renderer.size.y;

        local.x += 0.5f;
        local.y += 0.5f;

        bool result = texture.GetPixelBilinear(local.x, local.y).a >= 0.5f;
        _Renderer.color = result ? new Color(1, 1, 1, 0.5f) : Color.white;
    }
}
