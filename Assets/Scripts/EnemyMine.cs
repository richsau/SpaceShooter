using System.Collections;
using UnityEngine;

public class EnemyMine : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PulseColor());
        Destroy(this.gameObject, 5f);
    }

    IEnumerator PulseColor()
    {
        Color _colorDim = new Color(196, 14, 219);
        Color _colorBright = new Color(147, 12, 164);
        Material _material = GetComponent<Renderer>().material;

        while (true)
        {
            var ratio = Mathf.Abs(Mathf.Sin(Time.time * 4f));
            _material.color = Color.Lerp(_colorDim, _colorBright, ratio);
            yield return new WaitForSeconds(.5f);
        }
    }
}
