using System;
using UnityEngine;

namespace Settings
{
    public class EsconderButtons : MonoBehaviour
    {
        private void Update()
        {
            var obj = GameObject.FindWithTag("TelaFinal");
            if (!obj) return;
            if (obj.activeSelf == false) return;
            Destroy(gameObject);
        }
    }
}