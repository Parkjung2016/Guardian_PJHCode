using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeWriter : MonoBehaviour
{
    TMP_Text _tmpProText;
    string writer;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;
    [SerializeField] private EventReference _typeWriterSound;

    void Start()
    {
        _tmpProText = GetComponent<TMP_Text>()!;
    }

    public void SetTypeWriter(string text)
    {
        if (_tmpProText != null)
        {
            writer = text;
            _tmpProText.text = "";

            StartCoroutine(TypeWriterTMP());
        }
    }

    public IEnumerator SetTypeWriterCoroutine(string text)
    {
        if (_tmpProText != null)
        {
            writer = text;
            _tmpProText.text = "";

            yield return TypeWriterTMP();
        }

        yield return null;
    }


    IEnumerator TypeWriterTMP()
    {
        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (_tmpProText.text.Length > 0)
            {
                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }

            _tmpProText.text += c;
            _tmpProText.text += leadingChar;
            SoundManager.PlaySFX(_typeWriterSound);
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }
    }
}