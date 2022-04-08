using System;

namespace AfterDarkScreensavers
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class StartAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    internal class RenderAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    internal class PreRenderAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    internal class PostRenderAttribute : Attribute { }
}
