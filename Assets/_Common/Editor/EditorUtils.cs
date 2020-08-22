using UnityEditor;

namespace Com.Github.Knose1.Common.Editor
{
	public static class EditorUtils
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyPath"></param>
		/// <returns></returns>
		/// <source>
		/// https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
		/// </source>
		public static System.Type GetType(SerializedProperty property)
		{
			System.Type parentType = property.serializedObject.targetObject.GetType();
			System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
			return fi.FieldType;
		}
	}
}
