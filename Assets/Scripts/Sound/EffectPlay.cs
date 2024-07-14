using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectPlay : MonoBehaviour
{
    Button button;
    [SerializeField]string effectName;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            SoundManager.Instance.EffectPlayStr(effectName);
        });
    }

}
