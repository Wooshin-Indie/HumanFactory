using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace HumanFactory.Manager
{
    /// <summary>
    /// 그리드 밖 Human들을 관리하는 MapManager의 partial 클래스
    /// </summary>
    public class WaitingsManagement : MonoBehaviour
    {
        private List<List<Awaiter>> humanWaitings = new List<List<Awaiter>>()
        {
            new List<Awaiter>(), 
            new List<Awaiter>(), 
            new List<Awaiter>()
        };

        private List<List<Vector2>> waitPoints = new List<List<Vector2>>()
        {
            new List<Vector2>(),
            new List<Vector2>(),
            new List<Vector2>()
        };

        [SerializeField] private GameObject awaiterPrefab;
        [SerializeField] private Vector2 waitLength;
        [SerializeField] private int waitCount;
        [SerializeField] private float waitRandom;
        [SerializeField] private List<Vector2> waitStart = new List<Vector2>();

        public void InitWaitings()
        {
            GetPointsFromSineLines();
        }

        private void GetPointsFromSineLines()
        {
            Vector3 tmpRandom;
            Awaiter tmpHuman;
            List<Vector3> points;

            for (int i = 0; i < humanWaitings.Count; i++)
            {
                points = GenerateSinWaveEqualArcLength(waitCount, 0, Mathf.PI, i);

                int middle = points.Count/2;
                points.RemoveAt(middle);
                points.RemoveAt(middle + 1);
                points.RemoveAt(middle - 1);
                foreach (Vector3 point in points)
                {
                    waitPoints[i].Add(new Vector2(point.x, point.y));

                    tmpRandom = new Vector3(Random.Range(0f, waitRandom), Random.Range(0f, waitRandom), 0);
                    tmpHuman = Instantiate(awaiterPrefab, point + tmpRandom, Quaternion.identity).GetComponent<Awaiter>();
                    tmpHuman.GetComponent<Animator>().speed = Random.Range(0.75f, 1.25f);
                    humanWaitings[i].Add(tmpHuman);
                    // sin파 모양으로 대기중인human 생성 ( ')'모양으로 생성 )
                }
            }
        }

        //List<Vector3> GenerateSinWave(int n, float startX, float endX)
        //{
        //    List<Vector3> points = new List<Vector3>();
        //    float step = (endX - startX) / (n - 1); // X 간격 계산

        //    for (int i = 0; i < n; i++)
        //    {
        //        float y = startX + i * step;
        //        float x = Mathf.Sin(y) * waitLength.x;
        //        y *= waitLength.y;
        //        points.Add(new Vector3(x + waitStart.x, y + waitStart.y, Constants.HUMAN_POS_Z));
        //    }

        //    return points;
        //}

        List<Vector3> GenerateSinWaveEqualArcLength(int n, float start, float end, int idx)
        {
            // n이 2 미만이면 점을 만들 수 없음
            if (n < 2)
            {
                Debug.LogError("적어도 두 개 이상의 점이 필요합니다.");
                return new List<Vector3>();
            }

            // 충분한 해상도로 곡선을 샘플링하여 누적 호 길이를 계산
            int subdivisions = 10000;          // 이 값이 클수록 정확도가 높아짐
            float dx = (end - start) / subdivisions;

            // 샘플링된 x좌표와 누적 호 길이를 저장할 배열 생성
            float[] xs = new float[subdivisions + 1];
            float[] arcLengths = new float[subdivisions + 1];

            xs[0] = start;
            arcLengths[0] = 0f;

            for (int i = 1; i <= subdivisions; i++)
            {
                xs[i] = start + i * dx;
                float xPrev = xs[i - 1];
                float xCurrent = xs[i];
                float yPrev = Mathf.Sin(xPrev);
                float yCurrent = Mathf.Sin(xCurrent);
                // 선분의 길이를 피타고라스 정리로 계산
                float segmentLength = Mathf.Sqrt(dx * dx + (yCurrent - yPrev) * (yCurrent - yPrev));
                arcLengths[i] = arcLengths[i - 1] + segmentLength;
            }

            float totalLength = arcLengths[subdivisions];
            List<Vector3> points = new List<Vector3>();

            // n개의 점으로 (n-1)개의 동일한 길이의 구간을 만들기 위해 각 점이 위치해야 할 누적 호 길이를 계산
            for (int j = 0; j < n; j++)
            {
                float targetLength = totalLength * j / (n - 1);

                // 누적 호 길이 배열에서 targetLength에 해당하는 인덱스 찾기 (이진 탐색 사용)
                int index = System.Array.BinarySearch(arcLengths, targetLength);
                if (index < 0)
                {
                    index = ~index; // 삽입 지점을 구함
                }
                // 경계값 체크
                if (index == 0)
                    index = 1;
                if (index >= arcLengths.Length)
                    index = arcLengths.Length - 1;

                // 두 인접 샘플 사이에서 선형 보간하여 정확한 x 좌표 구하기
                float s0 = arcLengths[index - 1];
                float s1 = arcLengths[index];
                float t = (targetLength - s0) / (s1 - s0);
                float xValue = Mathf.Lerp(xs[index - 1], xs[index], t);
                float yValue = Mathf.Sin(xValue) * waitLength.x;
                xValue *= waitLength.y;
                points.Add(new Vector3(yValue + waitStart[idx].x, xValue + waitStart[idx].y, 0));
            }

            return points;
        }

        private float duration = 0f;
        private float intervalTime = 0.2f;
        private bool isWalking = false;

        private void Update()
        {
            for (int i = 0; i < humanWaitings.Count; i++)
            {
                for (int j = 0; j < humanWaitings[i].Count; j++)
                {
                    humanWaitings[i][j].OnUpdate();
                }
            }
        }

        public IEnumerator WaitingsCoroutine()
        {
            for (int i = 0; i < humanWaitings.Count; i++)
            {
                humanWaitings[i][0].HeadToEnd(waitPoints[i][humanWaitings[i].Count - 1]); // 맨 앞에 Awaiter는 맨 뒤의 Awiater의 위치로 이동
                Awaiter tmpAwaiter = humanWaitings[i][0];
                humanWaitings[i].RemoveAt(0);
                humanWaitings[i].Add(tmpAwaiter);
                for (int j = 0; j < humanWaitings[i].Count - 1; j++)
                {
                    yield return new WaitForSeconds(intervalTime * MapManager.Instance.CycleTime);
                    humanWaitings[i][j].WalkNextStep(waitPoints[i][j]);
                }
            }
        }
    }
}