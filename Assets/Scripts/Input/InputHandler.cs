using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AmoaebaUtils;

public class InputHandler : ScriptableObject
{
    [SerializeField]
    private InputSchemeVar currentScheme;
    public InputScheme currentInput => currentScheme.Value;

    [SerializeField]
    private InputScheme customInputScheme;

    private Dictionary<string, KeyCode> keymap = new Dictionary<string, KeyCode>();

    private CoroutineRunner runner = null;
    private IEnumerator nextKeyRoutine = null;

    public delegate void GetKeyCallback(KeyCode code);

    private Action storedGetNextKeyCancel;

    private void OnEnable() 
    {
        keymap.Clear();
        Array values = Enum.GetValues(typeof(KeyCode));    
        foreach(KeyCode code in values)
        {
            keymap[code.ToString().ToUpper()] = code;
        }
    }

    private void OnDestroy()
    {
        runner = null;
        nextKeyRoutine = null;
    }


    public Vector2 GetMoveAxis()
    {
        if(currentScheme.Value.UseJoysticks)
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        

        float posX = ValueFromKey(currentScheme.Value.PositiveXKey);
        float negX = -ValueFromKey(currentScheme.Value.NegativeXKey);
        float posY = ValueFromKey(currentScheme.Value.PositiveYKey);
        float negY = -ValueFromKey(currentScheme.Value.NegativeYKey);
        return new Vector2(posX + negX, posY + negY);
    }

    public Vector2 GetShootAxis()
    {
        
        if(currentScheme.Value.UseJoysticks)
        {

            Vector2 vector = new Vector2(Input.GetAxis("FireH"), Input.GetAxis("FireV"));
            Debug.Log(""  + vector);

            return vector;
        }
        float posX = ValueFromKey(currentScheme.Value.PositiveShootXKey);
        float negX = -ValueFromKey(currentScheme.Value.NegativeShootXKey);
        float posY = ValueFromKey(currentScheme.Value.PositiveShootYKey);
        float negY = -ValueFromKey(currentScheme.Value.NegativeShootYKey);
        return new Vector2(posX + negX, posY + negY);
    }

    public bool IsShooting()
    {
        return !Mathf.Approximately(GetShootAxis().magnitude,0);
    }

    public bool IsReversing()
    {
        return Input.GetKey(currentScheme.Value.ReverseTimeKey);
    }

    public bool IsPauseDown()
    {
        return Input.GetKeyDown(currentScheme.Value.PauseKey);
    }

    private float ValueFromKey(KeyCode code)
    {
        return Input.GetKey(code) ? 1 : 0;
    }

    public void GetNextKey(GetKeyCallback callback, Action cancelCallback)
    {
        if(runner == null)
        {
            runner = CoroutineRunner.Instantiate(this.name +"_KeyGetter");
        }
        else if(nextKeyRoutine != null)
        {
            if(storedGetNextKeyCancel != null)
            {
                storedGetNextKeyCancel.Invoke();
            }
            runner.StopCoroutine(nextKeyRoutine);
        }
        storedGetNextKeyCancel = cancelCallback;
        nextKeyRoutine = GetNextKeyRoutine(callback);
        runner.StartCoroutine(nextKeyRoutine);
    }

    private IEnumerator GetNextKeyRoutine(GetKeyCallback callback)
    {
        KeyCode extractedCode = KeyCode.None;
        while(extractedCode == KeyCode.None)
        {
            extractedCode = GetNextKey();
            if(extractedCode != KeyCode.None)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        storedGetNextKeyCancel = null;
        callback(extractedCode);
    }

    private KeyCode GetNextKey()
    {
        if(!Input.anyKey)
        {
            return KeyCode.None;
        }

        foreach(string key in keymap.Keys)
        {
            if(Input.GetKey(keymap[key]))
            {
                return keymap[key];
            }
        }
        return KeyCode.None;
    }

    public void CopyCurrentSchemeToCustom(bool useCustom = true)
    {
        if(currentScheme.Value == customInputScheme)
        {
            return;
        }
        customInputScheme.CopyFrom(currentScheme.Value);
        if(useCustom)
        {
            currentScheme.Value = customInputScheme;
        }
    }

    
}
