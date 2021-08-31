using System;

namespace SimpleMath.Supports
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContentsTypeAttribute : Attribute
    {
        public ContentsType ContentsType { get; set; }
    }
}
