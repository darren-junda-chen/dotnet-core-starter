using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Business.Helpers
{
  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public class IgnoreDeserializeAttribute : Attribute
  {
  }

  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public class IgnoreSerializeAttribute : Attribute
  {
  }

  public class CamelCaseGetOnlyContractResolver : CamelCasePropertyNamesContractResolver
  {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      var property = base.CreateProperty(member, memberSerialization);
      if (property != null)
      {
        if (property.Writable)
        {
          var attributes = property.AttributeProvider.GetAttributes(typeof(IgnoreDeserializeAttribute), true);
          if (attributes != null && attributes.Count > 0)
            property.Writable = false;
        }

        if (property.Readable)
        {
          var attributes = property.AttributeProvider.GetAttributes(typeof(IgnoreSerializeAttribute), true);
          if (attributes != null && attributes.Count > 0)
            property.Readable = false;
        }
      }
      return property;
    }
  }
}

