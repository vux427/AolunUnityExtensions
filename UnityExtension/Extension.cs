using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

static public class Extension {
    /*
    static public float GetAngle(Vector3 vectorA, Vector3 vectorB)
    {
        float angle = Vector3.Angle(vectorA, vectorB);
        Vector3 cross = Vector3.Cross(vectorA, vectorB);
        return (cross.y < 0) ? -angle : angle; // clockwise +, counterclockwise -
    }
    */



    // 轉換 QT 到 Vector3
    static public float pi_180 = 3.141592f / 180f;
    static public Vector3 eEulerVector(Vector3 euler)//convert euler to vector3
    {
        float elevation = pi_180 * euler.x;
        float heading = pi_180 * euler.y;
        return new Vector3(Mathf.Cos(elevation) * Mathf.Sin(heading), Mathf.Sin(-elevation), Mathf.Cos(elevation) * Mathf.Cos(heading));
    }

    static public float GetAngle(this Vector3 forward, Vector3 target)
    {
        // the vector that we want to measure an angle from
        Vector3 referenceForward = forward;/* some vector that is not Vector3.up */
                                           // the vector perpendicular to referenceForward (90 degrees clockwise)
                                           // (used to determine if angle is positive or negative)
        Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
        // the vector of interest
        Vector3 newDirection = target;
        float angle = Vector3.Angle(newDirection, referenceForward);
        // Determine if the degree value should be negative.  Here, a positive value
        // from the dot product means that our vector is on the right of the reference vector   
        // whereas a negative value means we're on the left.
        float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
        float finalAngle = sign * angle;
        return finalAngle;
    }
    static public Vector3 GetAngleFromObserver(GameObject observer, Vector3 target)
    {
        Vector3 relative;
        Vector3 newDirection = target;
        relative = observer.transform.InverseTransformDirection(newDirection);
        float angleX = Vector3.Angle(new Vector3(newDirection.x, 0, newDirection.z), Vector3.forward);
        float angleY = Vector3.Angle(new Vector3(0, newDirection.y, newDirection.z), Vector3.forward);
        float angleZ = Vector3.Angle(new Vector3(newDirection.x, newDirection.y, 0), Vector3.up);
        angleX = newDirection.x > 0 ? angleX : -angleX;
        angleY = newDirection.y > 0 ? angleY : -angleY;
        angleZ = newDirection.x > 0 ? angleZ : -angleZ;
        return new Vector3(angleX, angleY, angleZ);
      }


    static public Vector2 GatAngleXYZ(this Vector3 forward, Vector3 target)
    {
        Vector3 fw = forward;
        Vector3 ri = Vector3.Cross(Vector3.up, fw);
        Vector3 up = Vector3.Cross(fw, ri);
        float dfw = Vector3.Dot(target, fw);
        float dri = Vector3.Dot(target, ri);
        float dup = Vector3.Dot(target, up);
        Vector2 txy = new Vector2(dfw, dri);
        Vector2 txz = new Vector2(Mathf.Sqrt((dfw * dfw) + (dri * dri)), dup);
        float angXY = Vector2.Angle(Vector2.right, txy);
        float angXZ = Vector2.Angle(Vector2.right, txz);
        if (dri < 0)
        {
            angXY = -angXY;
        }
        if (dup < 0)
        {
            angXZ = -angXZ;
        }
        //Debug.LogWarning("XY=" + angXY.ToString() + " XZ=" + angXZ.ToString());
        return new Vector2(angXY, angXZ);
        // by.Weberkkk 2016.04.27
    }

    static public Vector2 GatAngleXYZ2(this Vector3 target, Vector3 obj)
    {
        Vector3 fw = new Vector3(0, 0, 1);
        Vector3 ri = new Vector3(1, 0, 0);
        Vector3 up = new Vector3(0, 1, 0);
        Vector3 ot = (target - obj);
        float dfw = Vector3.Dot(ot, fw);
        float dri = Vector3.Dot(ot, ri);
        float dup = Vector3.Dot(ot, up);
        Vector2 txy = new Vector2(dfw, dri);
        Vector2 txz = new Vector2(Mathf.Sqrt((dfw * dfw) + (dri * dri)), dup);
        float angXY = Vector2.Angle(Vector2.right, txy);
        float angXZ = Vector2.Angle(Vector2.right, txz);
        if (dri < 0)
        {
            angXY = -angXY;
        }
        if (dup > 0)
        {
            angXZ = -angXZ;
        }
        //Debug.LogWarning("XY=" + angXY.ToString() + " XZ=" + angXZ.ToString());
        return new Vector2(angXY, angXZ);
        // by.Weberkkk 2016.04.27
    }

    /*
    /// <summary>
    /// 從Children取得未包含自己、包含未啟動的obj
    /// </summary>
    static public T[] GetComponentsInChildrenByRecursion<T>(this Transform target) where T : Component
    {
        return GetComponentsInChildrenByRecursion<T>(target,false, true);
    }

    static public T[] GetComponentsInChildrenByRecursion<T>(this Transform target, bool incudeSelf, bool incudeNoActiveObj) where T : Component
    {
        var list = new List<T>();
        if (incudeSelf && target.GetComponent<T>() != null) //get self
            list.Add(target.GetComponent<T>());

        foreach (Transform child in target)
        {
            var component = child.GetComponent<T>();
            var isActive = component.gameObject.activeInHierarchy;

            if (!incudeNoActiveObj) //only get active obj
            {
                if (component != null && isActive)
                    list.Add(component);
            }
            else {
                if (component != null)
                    list.Add(component);
            }

            if (child.childCount > 0)
            {
                foreach (var t in GetComponentsInChildrenByRecursion<T>(child,incudeSelf,incudeNoActiveObj))
                {
                    list.Add(t);
                }
            }
        }
        return list.ToArray();
    }
    */

    static public T[] GetComponentsInParentByRecursion<T>(this Component target) where T : Component
    {
        return GetComponentsInParentByRecursion<T>(target, false, true);
    }

    static public T[] GetComponentsInParentByRecursion<T>(this Component target, bool incudeSelf, bool incudeNoActiveObj) where T : Component
    {
        var children = GetTransformsInParentByRecursion(target.transform, incudeSelf, incudeNoActiveObj);
        var list = new List<T>();
        foreach (var child in children)
        {
            var component = child.GetComponent<T>();
            if (component != null)
                list.Add(component);
        }
        return list.ToArray();
    }

    static public T[] GetComponentsInChildrenByRecursion<T>(this Component target) where T : Component
    {
        return GetComponentsInChildrenByRecursion<T>(target, false, true);
    }

    static public T[] GetComponentsInChildrenByRecursion<T>(this Component target, bool incudeSelf, bool incudeNoActiveObj) where T : Component
    {
        var children = GetTransformsInChildrenByRecursion(target.transform, incudeSelf,incudeNoActiveObj);

        var list = new List<T>();
        foreach (var child in children) {
            Debug.Log(child);
            var component = child.GetComponent<T>();
            if (component != null)
                list.Add(component);
        }
        return list.ToArray();
    }
    
    static public Transform[] GetTransformsInParentByRecursion(this Transform target, bool incudeSelf, bool incudeNoActiveObj)
    {
        var list = new List<Transform>();
        if (incudeSelf) //get self
            list.Add(target);

        if (target.parent == null) return list.ToArray();

        if (!incudeNoActiveObj) //only get active obj{}
        {
            if (target.parent.gameObject.activeInHierarchy)
                list.Add(target.parent);
        }
        else {
            list.Add(target.parent);
        }

        foreach (Transform parent in GetTransformsInParentByRecursion(target.parent, false, incudeNoActiveObj))
            list.Add(parent);

        return list.ToArray();
    }


    static public Transform[] GetTransformsInChildrenByRecursion(this Transform target, bool incudeSelf, bool incudeNoActiveObj)
    {
        var list = new List<Transform>();
        if (incudeSelf) //get self
            list.Add(target);

        foreach (Transform child in target)
        {
            if (!incudeNoActiveObj)//only get active obj
            { 
                if (child.gameObject.activeInHierarchy)
                    list.Add(child);
            }
            else {
                list.Add(child);
            }

//            Debug.Log(string.Format("child:{0}", child));
            if (child.childCount > 0)
                foreach (var t in GetTransformsInChildrenByRecursion(child, false, incudeNoActiveObj))
                    list.Add(t);
        }

        return list.ToArray();
    }


    static public string GetStringValue(this Enum enumItem)
    {
        var attribs = enumItem
            .GetType().GetField(enumItem.ToString())
            .GetCustomAttributes(typeof(StringValue), false) as StringValue[];

        return attribs[0].value ?? null;
    }

    static public void UITransitionPlay(this Animator animator, bool enable) {
        if (enable)
            animator.Play("Start");
        else
            animator.Play("End");
    }



    static public Type GetTypeFromAssembly(this string typeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(typeName);

        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (typeName.Contains("."))
        {
            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = typeName.Substring(0, typeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();

        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);   // Load the referenced assembly
            // need inculde namespace
            type = assembly.GetType(assembly.FullName.Split(',')[0] + "." + typeName);
            //Debug.Log(type);

            if (type != null)
                return type;
        }

        Debug.LogError(string.Format("@Not found #{0}# type in assemblys.", typeName));
        return null;

    }

    static public void ForEach<T>(this IEnumerable<T> source,Action<T> action)
    {
        foreach (T element in source)
            action(element);
    }

    static public int BoolenToInt(this bool value)
    {
        return value ? 1 : 0;
    }

    static public float Arithmetic(float value1,float value2,string operation)
    {
		switch (operation)
		{
			case "+": 		return value1 + value2;
			case "-": 		return value1 - value2;
			case "*": 		return value1 * value2;
			case "division":return value1 / value2;
		}
		return 0;
	}


    static public bool IFOperator(float value1,float value2,string operation)
    {
		switch (operation)
		{
			case ">": 	return value1 > value2;
			case "<": 	return value1 < value2;
			case ">=": 	return value1 >= value2;
			case "<=":  return value1 <= value2;
            case "==":  return value1 == value2;
			case "!=":  return value1 != value2;
		}
		return false;
	}

    static public bool MoveToFront<T>(this T[] array, Predicate<T> match)
    {
        if (array.Length == 0)
            return false;

        var index = Array.FindIndex(array, match);
        if (index == -1)
            return false;

        var temp = array[index];
        Array.Copy(array, 0, array, 1, index);
        array[0] = temp;
        return true;
    }

    static public void SetCheckedText(this TMPro.TextMeshProUGUI text,string content)
	{
		if (text != null)
			text.text = content;
	}

}





public class StringValue : Attribute
{
    private string _value;
    public StringValue(string value) { _value = value; }
    public string value { get { return _value; } }
}

[HideInInspector]
public class MinMaxSlider : PropertyAttribute
{
    public readonly float max;
    public readonly float min;

    public MinMaxSlider(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}