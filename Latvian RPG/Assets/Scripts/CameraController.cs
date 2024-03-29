﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform startTarget;
    public bool mouseMove = true;
    public bool keyboardMove = true;

    [SerializeField] private Renderer m_levelRenderer;
    [SerializeField] private Bounds m_levelBounds;
    [SerializeField] private float m_cameraSpeed;
    [SerializeField, Tooltip("Percent of screen size"), Range(0f, 50f)] private float m_screenDragBounds = 5f;

    private float keyboardSpeedMultiplier = 6f;

    private Vector3 m_targetCamPosition;
    private Vector3 m_prevCamPosition;

    private Vector2 m_screenBounds;
    private bool initialized = false;

    public void IntializeCamera(Transform newStart, GameManager gameManager, bool loadCamPos = false)
    {
        startTarget = newStart;
        
        if (loadCamPos)
        {
            startTarget.position = new Vector3(
                gameManager.saveManager.loadedCamPosX,
                gameManager.saveManager.loadedCamPosY,
                startTarget.position.z);
        }

        m_targetCamPosition = new Vector3(startTarget.position.x, startTarget.position.y + 2, transform.position.z);
        m_prevCamPosition = transform.position;


        SetPosition(m_targetCamPosition, instant: true);

        m_screenBounds = new Vector2(Screen.width * (m_screenDragBounds * 0.01f), Screen.height * (m_screenDragBounds * 0.01f));
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;
        Vector3 dir = Vector3.zero;

        //if (mouseMove)
        //{
        //    dir += GetMouseInput();
        //}

        if (keyboardMove)
        {
            dir += GetKeyboardInput();
        }

        if (dir.magnitude > m_cameraSpeed)
        {
            dir = dir.normalized * m_cameraSpeed;
        }

        if (dir.magnitude > 0f)
        {
            m_targetCamPosition = transform.position + dir;
        }

        m_targetCamPosition.x = Mathf.Clamp(m_targetCamPosition.x, m_levelBounds.min.x + cam.orthographicSize * cam.aspect, m_levelBounds.max.x - cam.orthographicSize * cam.aspect);
        m_targetCamPosition.y = Mathf.Clamp(m_targetCamPosition.y, m_levelBounds.min.y + cam.orthographicSize, m_levelBounds.max.y - cam.orthographicSize);
    }

    public float dragSpeed = -25;
    private Vector3 dragOrigin;
    private void LateUpdate()
    {

            if (!initialized)
            return;
        #region CAMERA PANNING

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if ((Input.GetAxis("Mouse X") != 0) ||
                (Input.GetAxis("Mouse Y") != 0) )
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
                transform.Translate(move, Space.World);
                m_targetCamPosition = transform.position;
            }
        }
    
        transform.position = Vector3.Lerp(m_prevCamPosition, m_targetCamPosition, m_cameraSpeed * Time.deltaTime);
        m_prevCamPosition = transform.position;

        #endregion
    }




    public void SetPosition(Vector3 position, bool instant = false)
    {
        m_targetCamPosition = position;

        if (!instant)
            return;

        m_targetCamPosition.x = Mathf.Clamp(m_targetCamPosition.x, m_levelBounds.min.x + cam.orthographicSize * cam.aspect, m_levelBounds.max.x - cam.orthographicSize * cam.aspect);
        m_targetCamPosition.y = Mathf.Clamp(m_targetCamPosition.y, m_levelBounds.min.y + cam.orthographicSize, m_levelBounds.max.y - cam.orthographicSize);
        transform.position = m_targetCamPosition;
        m_prevCamPosition = transform.position;
    }

    private Vector3 GetMouseInput()
    {
        Vector3 dir = Vector3.zero;

        if (Input.mousePosition.y < m_screenBounds.y)
        {
            dir.y -= Mathf.Lerp(0f, m_cameraSpeed, (m_screenBounds.y - Input.mousePosition.y) / m_screenBounds.y);
        }
        else if (Input.mousePosition.y > Screen.height - m_screenBounds.y)
        {
            dir.y += Mathf.Lerp(m_cameraSpeed, 0f, (Screen.height - Input.mousePosition.y) / m_screenBounds.y);
        }

        if (Input.mousePosition.x < m_screenBounds.x)
        {
            dir.x -= Mathf.Lerp(0f, m_cameraSpeed, (m_screenBounds.x - Input.mousePosition.x) / m_screenBounds.x);
        }
        else if (Input.mousePosition.x > Screen.width - m_screenBounds.x)
        {
            dir.x += Mathf.Lerp(m_cameraSpeed, 0f, (Screen.width - Input.mousePosition.x) / m_screenBounds.x);
        }

        return dir;
    }

    private Vector3 GetKeyboardInput()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir.y += m_cameraSpeed * keyboardSpeedMultiplier;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir.y -= m_cameraSpeed * keyboardSpeedMultiplier;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dir.x -= m_cameraSpeed * keyboardSpeedMultiplier;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir.x += m_cameraSpeed * keyboardSpeedMultiplier;
        }

        return dir;
    }
}
