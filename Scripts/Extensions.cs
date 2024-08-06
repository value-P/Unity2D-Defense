using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Extensions
{
    // 우측앞 꼭짓점과 좌측뒤 꼭짓점을 주면 박스 안에 랜덤한 위치값을 반환
    public static Vector3 RandomizeArea(Vector3 rightFrontPos, Vector3 leftBackPos)
    {
        float randX = Random.Range(rightFrontPos.x, leftBackPos.x);
        float randZ = Random.Range(rightFrontPos.z, leftBackPos.z);

        return new Vector3(randX, 0, randZ);
    }

    // Destroy를 가질 수 있는 Object에 대한 싱글톤 메서드
    public static void Singleton<T>(this T target, ref T instance) where T : Object 
    {
        // 이미 instance에 다른게 들어가 있다면
        if (instance && instance != target)
        {
            GameObject.Destroy(target); // 못들어오게 삭제
        }
        else
        {
            instance = target;
        };
    }

    // 각도를 벡터로
    public static Vector3 ToDirection(this float value)
    {
        //                                   곱해주면 360도를 2PI로
        return new Vector3(Mathf.Cos(value * Mathf.Deg2Rad), Mathf.Sin(value * Mathf.Deg2Rad));
    }

    /// <summary> 수평,수직각도 전달하면 3차원 방향을 알려줌 </summary>
    /// <param name="value"> x에 수평각도, y에 수직각도 </param>
    public static Vector3 ToDirection(this Vector2 angles)
    {
        Vector3 result;
        // 각도를 라디안 값으로 조정
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

    // 벡터를 각도로
    public static float ToAngle(this Vector3 value)
    {
        return Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
    }
    // 벡터의 수평각도 계산
    public static float ToHorizontalAngle(this Vector3 value)
    {
        value.y = 0;          // 수평길이 확인을 위해 y제거
        value.Normalize();    // 길이를 다시 1로 맞춰주기

        return Mathf.Atan2(value.z, value.x) * Mathf.Rad2Deg;
    }

    // origin 부터 maxDistance 사이에 가장 가까운 녀석을 반환
    public static T GetNearest<T>(this List<T> targetList, Vector3 origin, float maxDistance) where T : MonoBehaviour
    {
        float nearestDistance = maxDistance;
        T nearestCharacter = null;

        foreach(T current in targetList)
        {
            if (current == null) continue; // 예외처리, 비어있으며 이번거 넘겨

            float currentDistance = (current.transform.position - origin).magnitude; // 플레이어와 요청위치 사이의 거리가 얼마인지?
            
            // 짧은기록 갱신 시 바꿔줌
            if(currentDistance < nearestDistance)
            {
                nearestCharacter = current; //가장 가까운 녀석 등록
                nearestDistance = currentDistance; // 가장 가까운 거리도 갱신
            };
        };

        return nearestCharacter;
    }

    // 원하는 지점을 중심으로 랜덤한 원하는 거리 이내의 위치를 받아온다
    public static Vector3 GetRandomPosition(this Vector3 origin, Vector3 distance)
    {
        Vector3 result = origin; // 중심에서 시작

        result.x += Random.Range(-distance.x, distance.x);
        result.y += Random.Range(-distance.y, distance.y);
        result.z += Random.Range(-distance.z, distance.z);

        return result;
    }
    // 최소 min이상은 떨어진 거리 내에서 랜덤위치 받아오기
    public static Vector3 GetRandomPosition(this Vector3 origin, Vector3 minDistance, Vector3 maxDistance)
    {
        Vector3 result = origin;

        result.x = Random.Range(minDistance.x, maxDistance.x);
        //            0 ~ 1.0          그대로? 반대로?
        result.x *= Random.value >= 0.5f ? 1 : -1;

        result.y = Random.Range(minDistance.y, maxDistance.y);
        result.y *= Random.value >= 0.5f ? 1 : -1;

        result.z = Random.Range(minDistance.z, maxDistance.z);
        result.z *= Random.value >= 0.5f ? 1 : -1;

        return result + origin;
    }

    // "도넛" 모양의 랜덤한 위치
    public static Vector3 GetRandomPosition(this Vector3 origin, float minDistance, float maxDistance)
    {
        // 기준점에서 + (360도 내의 랜덤한 각도를 벡터로 전환  *  최소거리와 최대거리 내의 랜덤한 값만큼 곱)
        return origin + Random.Range(0, 360f).ToDirection() * Random.Range(minDistance, maxDistance);
    }
    // "원형"의 랜덤한 위치 : 도넛의 최소거리 0으로
    public static Vector3 GetRandomPosition(this Vector3 origin, float distance)
    {
        return GetRandomPosition(origin, 0, distance);
    }

    // 해당 인덱스가 2차원 배열의 범위 밖인지 확인하는 메서드 (나갔으면 true)
    public static bool IsOutSide<T>(this T[,] targetArray, int primary, int second) 
    {
        return (targetArray.GetLength(0) <= primary || primary < 0 || targetArray.GetLength(1) <= second || second < 0);
    }

    // center를 중심으로 range를 반지름으로 하는 원 안에서 이동할 수 있느 점을 입력한 result로 보내주고 bool반환
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

        // color rgba 값
        int red = Mathf.RoundToInt(wantColor.r * 255);
        int green = Mathf.RoundToInt(wantColor.g * 255);
        int blue = Mathf.RoundToInt(wantColor.b * 255);

        // 글자 색 수정
        result = $"<#{red.ToString("X2")}{green.ToString("X2")}{blue.ToString("X2")}>" + result + "</color>";

        // 태그에 맞춰 글자 수정
        // <b> Bold                 굵은 글씨
        // <i> Italic                  기울임
        //<u> UnderLine         밑줄
        //<s> Strike Through  취소선
        foreach(var current in tags)
        {
            result = $"<{current}>" + result + $"</{current}>";
        };

        return result;
    } // 원하는 글자 색과 태그로 변경해줌

    public static string ToText(this Rarity target)
    {
        switch (target)
        {
            case Rarity.Normal:  return "일반";
            case Rarity.Rare:       return "레어".Setting(Color.blue);
            case Rarity.Heroic:    return "영웅".Setting(Color.green);
            case Rarity.Legend:  return "전설".Setting(Color.red);
            default: return "알려지지 않음";
        };
    }  // 등급에 맞춰 색상이 바뀐 string반환

}
