using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Extensions
{
    // ������ �������� ������ �������� �ָ� �ڽ� �ȿ� ������ ��ġ���� ��ȯ
    public static Vector3 RandomizeArea(Vector3 rightFrontPos, Vector3 leftBackPos)
    {
        float randX = Random.Range(rightFrontPos.x, leftBackPos.x);
        float randZ = Random.Range(rightFrontPos.z, leftBackPos.z);

        return new Vector3(randX, 0, randZ);
    }

    // Destroy�� ���� �� �ִ� Object�� ���� �̱��� �޼���
    public static void Singleton<T>(this T target, ref T instance) where T : Object 
    {
        // �̹� instance�� �ٸ��� �� �ִٸ�
        if (instance && instance != target)
        {
            GameObject.Destroy(target); // �������� ����
        }
        else
        {
            instance = target;
        };
    }

    // ������ ���ͷ�
    public static Vector3 ToDirection(this float value)
    {
        //                                   �����ָ� 360���� 2PI��
        return new Vector3(Mathf.Cos(value * Mathf.Deg2Rad), Mathf.Sin(value * Mathf.Deg2Rad));
    }

    /// <summary> ����,�������� �����ϸ� 3���� ������ �˷��� </summary>
    /// <param name="value"> x�� ���򰢵�, y�� �������� </param>
    public static Vector3 ToDirection(this Vector2 angles)
    {
        Vector3 result;
        // ������ ���� ������ ����
        angles.x *= Mathf.Deg2Rad;
        angles.y *= Mathf.Deg2Rad;

        result.x = Mathf.Cos(angles.x) * Mathf.Abs(Mathf.Cos(angles.y));
        result.y = Mathf.Sin(angles.x) * Mathf.Abs(Mathf.Sin(angles.y));
        result.z = Mathf.Sin(angles.y);

        return result;
    }
    public static Vector3 ToDirection(float horAngle, float verAngle)
    {
        return ToDirection(new Vector2(horAngle, verAngle));
    }

    // ���͸� ������
    public static float ToAngle(this Vector3 value)
    {
        return Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
    }
    // ������ ���򰢵� ���
    public static float ToHorizontalAngle(this Vector3 value)
    {
        value.y = 0;          // ������� Ȯ���� ���� y����
        value.Normalize();    // ���̸� �ٽ� 1�� �����ֱ�

        return Mathf.Atan2(value.z, value.x) * Mathf.Rad2Deg;
    }

    // origin ���� maxDistance ���̿� ���� ����� �༮�� ��ȯ
    public static T GetNearest<T>(this List<T> targetList, Vector3 origin, float maxDistance) where T : MonoBehaviour
    {
        float nearestDistance = maxDistance;
        T nearestCharacter = null;

        foreach(T current in targetList)
        {
            if (current == null) continue; // ����ó��, ��������� �̹��� �Ѱ�

            float currentDistance = (current.transform.position - origin).magnitude; // �÷��̾�� ��û��ġ ������ �Ÿ��� ������?
            
            // ª����� ���� �� �ٲ���
            if(currentDistance < nearestDistance)
            {
                nearestCharacter = current; //���� ����� �༮ ���
                nearestDistance = currentDistance; // ���� ����� �Ÿ��� ����
            };
        };

        return nearestCharacter;
    }

    // ���ϴ� ������ �߽����� ������ ���ϴ� �Ÿ� �̳��� ��ġ�� �޾ƿ´�
    public static Vector3 GetRandomPosition(this Vector3 origin, Vector3 distance)
    {
        Vector3 result = origin; // �߽ɿ��� ����

        result.x += Random.Range(-distance.x, distance.x);
        result.y += Random.Range(-distance.y, distance.y);
        result.z += Random.Range(-distance.z, distance.z);

        return result;
    }
    // �ּ� min�̻��� ������ �Ÿ� ������ ������ġ �޾ƿ���
    public static Vector3 GetRandomPosition(this Vector3 origin, Vector3 minDistance, Vector3 maxDistance)
    {
        Vector3 result = origin;

        result.x = Random.Range(minDistance.x, maxDistance.x);
        //            0 ~ 1.0          �״��? �ݴ��?
        result.x *= Random.value >= 0.5f ? 1 : -1;

        result.y = Random.Range(minDistance.y, maxDistance.y);
        result.y *= Random.value >= 0.5f ? 1 : -1;

        result.z = Random.Range(minDistance.z, maxDistance.z);
        result.z *= Random.value >= 0.5f ? 1 : -1;

        return result + origin;
    }

    // "����" ����� ������ ��ġ
    public static Vector3 GetRandomPosition(this Vector3 origin, float minDistance, float maxDistance)
    {
        // ���������� + (360�� ���� ������ ������ ���ͷ� ��ȯ  *  �ּҰŸ��� �ִ�Ÿ� ���� ������ ����ŭ ��)
        return origin + Random.Range(0, 360f).ToDirection() * Random.Range(minDistance, maxDistance);
    }
    // "����"�� ������ ��ġ : ������ �ּҰŸ� 0����
    public static Vector3 GetRandomPosition(this Vector3 origin, float distance)
    {
        return GetRandomPosition(origin, 0, distance);
    }

    // �ش� �ε����� 2���� �迭�� ���� ������ Ȯ���ϴ� �޼��� (�������� true)
    public static bool IsOutSide<T>(this T[,] targetArray, int primary, int second) 
    {
        return (targetArray.GetLength(0) <= primary || primary < 0 || targetArray.GetLength(1) <= second || second < 0);
    }

    // center�� �߽����� range�� ���������� �ϴ� �� �ȿ��� �̵��� �� �ִ� ���� �Է��� result�� �����ְ� bool��ȯ
    public static Vector3 RandomPoint(this Vector3 center, float range)
    {
        Vector3 result;
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return result;
            }
        }
        result = Vector3.zero;
        return result;
    }

    public static T IsChildOf<T>(this object target) where T : class
    {
        if (target.GetType() == typeof(T) || target.GetType().IsSubclassOf(typeof(T)))
            return (T)target;

        return null;
    }

    public static string Setting(this string target, Color wantColor, params string[] tags)
    {
        string result = target;

        // color rgba ��
        int red = Mathf.RoundToInt(wantColor.r * 255);
        int green = Mathf.RoundToInt(wantColor.g * 255);
        int blue = Mathf.RoundToInt(wantColor.b * 255);

        // ���� �� ����
        result = $"<#{red.ToString("X2")}{green.ToString("X2")}{blue.ToString("X2")}>" + result + "</color>";

        // �±׿� ���� ���� ����
        // <b> Bold                 ���� �۾�
        // <i> Italic                  �����
        //<u> UnderLine         ����
        //<s> Strike Through  ��Ҽ�
        foreach(var current in tags)
        {
            result = $"<{current}>" + result + $"</{current}>";
        };

        return result;
    } // ���ϴ� ���� ���� �±׷� ��������

    public static string ToText(this Rarity target)
    {
        switch (target)
        {
            case Rarity.Normal:  return "�Ϲ�";
            case Rarity.Rare:       return "����".Setting(Color.blue);
            case Rarity.Heroic:    return "����".Setting(Color.green);
            case Rarity.Legend:  return "����".Setting(Color.red);
            default: return "�˷����� ����";
        };
    }  // ��޿� ���� ������ �ٲ� string��ȯ

}
