using UnityEngine;

namespace Assets.Scripts.Models.Attributes
{
    /// <summary>
    /// Draws the assigned field in the Inspector
    /// even if nested inside a field marked with ReadOnly
    /// </summary>
    public sealed class CancelGreyOutAttribute : PropertyAttribute
    {

    }
}