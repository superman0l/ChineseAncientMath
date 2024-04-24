using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// �οձ���ͼ
/// </summary>
public class UIHollowImage : Graphic
{
    public float Radius = 10f;//����Բ�뾶 ͼƬ��һ�������һ��Բ�� �����൱��ͼƬʮ��֮һ�ĳ���
    public int TriangleNum = 6;//ÿ�����������θ��� ����Խ�󻡶�Խƽ��

    public RectTransform inner_trans;
    private RectTransform outer_trans;//��������

    private Vector2 inner_rt;//�ο���������Ͻ�����
    private Vector2 inner_lb;//�ο���������½�����
    private Vector2 outer_rt;//������������Ͻ�����
    private Vector2 outer_lb;//������������½�����
    [Header("�Ƿ�ʵʱˢ��")]
    [Space(25)]
    public bool realtimeRefresh;
    [Header("�Ƿ���ʾ�ο�")]
    [Space(25)]
    public bool ShowHollowOut = true;
    protected override void Awake()
    {
        base.Awake();

        outer_trans = GetComponent<RectTransform>();

        //����߽�
        CalcBounds();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        float tw = Mathf.Abs(inner_lb.x - inner_rt.x);//ͼƬ�Ŀ�
        float th = Mathf.Abs(inner_lb.y - inner_rt.y);//ͼƬ�ĸ�
        float twr = tw / 2;
        float thr = th / 2;

        if (Radius < 0)
            Radius = 0;
        float radius = tw / Radius;//�뾶������Ҫ��̬����ȷ�����ᱻ����
        if (radius > twr)
            radius = twr;
        if (radius < 0)
            radius = 0;
        if (TriangleNum <= 0)
            TriangleNum = 1;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        //0 outer���½�
        vert.position = new Vector2(outer_lb.x, outer_lb.y);
        vh.AddVert(vert);
        //1 outer���Ͻ�
        vert.position = new Vector2(outer_lb.x, outer_rt.y);
        vh.AddVert(vert);
        //2 outer���Ͻ�
        vert.position = new Vector2(outer_rt.x, outer_rt.y);
        vh.AddVert(vert);
        //3 outer���½�
        vert.position = new Vector2(outer_rt.x, outer_lb.y);
        vh.AddVert(vert);

        //4 inner���½�
        vert.position = new Vector3(inner_lb.x, inner_lb.y);
        vh.AddVert(vert);
        //5 inner���Ͻ�
        vert.position = new Vector3(inner_lb.x, inner_rt.y);
        vh.AddVert(vert);
        //6 inner���Ͻ�
        vert.position = new Vector3(inner_rt.x, inner_rt.y);
        vh.AddVert(vert);
        //7 inner���½�
        vert.position = new Vector3(inner_rt.x, inner_lb.y);
        vh.AddVert(vert);

        //����ʾ�ο�
        if (ShowHollowOut == false)
        {
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
            return;
        }
        //����������
        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(1, 4, 5);
        vh.AddTriangle(1, 5, 2);
        vh.AddTriangle(2, 5, 6);
        vh.AddTriangle(2, 6, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(4, 7, 3);
        vh.AddTriangle(0, 4, 3);


        //���ı��ζ���
        List<Vector2> commonPointOutside = new List<Vector2>();//�����б� 
        Vector2 point1 = new Vector2(inner_lb.x, inner_lb.y);//���¹���
        Vector2 point2 = new Vector2(inner_lb.x, inner_lb.y + radius);//�����״���ת����ĵ�
        commonPointOutside.Add(point1);
        commonPointOutside.Add(point2);
        point1 = new Vector2(inner_lb.x, inner_rt.y);//���Ϲ���
        point2 = new Vector2(inner_lb.x + radius, inner_rt.y);
        commonPointOutside.Add(point1);
        commonPointOutside.Add(point2);
        point1 = new Vector2(inner_rt.x, inner_rt.y);//���Ϲ���
        point2 = new Vector2(inner_rt.x, inner_rt.y - radius);
        commonPointOutside.Add(point1);
        commonPointOutside.Add(point2);
        point1 = new Vector2(inner_rt.x, inner_lb.y);//���¹���
        point2 = new Vector2(inner_rt.x - radius, inner_lb.y);
        commonPointOutside.Add(point1);
        commonPointOutside.Add(point2);


        //Բ�Ĺ�����
        List<Vector2> commonPoint = new List<Vector2>();//�����б� 
        point1 = new Vector2(inner_lb.x + radius, inner_lb.y + radius);//���¹���
        point2 = new Vector2(inner_lb.x, inner_lb.y + radius);//�����״���ת����ĵ�
        commonPoint.Add(point1);
        commonPoint.Add(point2);
        point1 = new Vector2(inner_lb.x + radius, inner_rt.y - radius);//���Ϲ���
        point2 = new Vector2(inner_lb.x + radius, inner_rt.y);
        commonPoint.Add(point1);
        commonPoint.Add(point2);
        point1 = new Vector2(inner_rt.x - radius, inner_rt.y - radius);//���Ϲ���
        point2 = new Vector2(inner_rt.x, inner_rt.y - radius);
        commonPoint.Add(point1);
        commonPoint.Add(point2);
        point1 = new Vector2(inner_rt.x - radius, inner_lb.y + radius);//���¹���
        point2 = new Vector2(inner_rt.x - radius, inner_lb.y);
        commonPoint.Add(point1);
        commonPoint.Add(point2);

        Vector2 pos2;


        float degreeDelta = (float)(Mathf.PI / 2 / TriangleNum);//ÿһ�ݵ��������εĽǶ� Ĭ��6��
        List<float> degreeDeltaList = new List<float>() { Mathf.PI, Mathf.PI / 2, 0, (float)3 / 2 * Mathf.PI };

        for (int j = 0; j < commonPoint.Count; j += 2)
        {
            float curDegree = degreeDeltaList[j / 2];//��ǰ�ĽǶ�
            AddVert(commonPointOutside[j], tw, th, vh, vert);//��������������������ι�������//TODO :����ĳ�����ĵ�
            int thrdIndex = vh.currentVertCount;//��ǰ�����εڶ���������
            int TriangleVertIndex = vh.currentVertCount - 1;//һ�����α��ֲ���Ķ�������
            List<Vector2> pos2List = new List<Vector2>();
            for (int i = 0; i < TriangleNum; i++)
            {
                curDegree += degreeDelta;
                if (pos2List.Count == 0)
                {
                    AddVert(commonPoint[j + 1], tw, th, vh, vert);
                }
                else
                {
                    vert.position = pos2List[i - 1];
                    vert.uv0 = new Vector2(pos2List[i - 1].x + 0.5f, pos2List[i - 1].y + 0.5f);
                }
                pos2 = new Vector2(commonPoint[j].x + radius * Mathf.Cos(curDegree), commonPoint[j].y + radius * Mathf.Sin(curDegree));
                AddVert(pos2, tw, th, vh, vert);
                vh.AddTriangle(TriangleVertIndex, thrdIndex, thrdIndex + 1);
                thrdIndex++;
                pos2List.Add(vert.position);
            }
        }
    }
    protected Vector2[] GetTextureUVS(Vector2[] vhs, float tw, float th)
    {
        int count = vhs.Length;
        Vector2[] uvs = new Vector2[count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vhs[i].x / tw + 0.5f, vhs[i].y / th + 0.5f);//���ε�uv����  ��Ϊuv����ԭ�������½ǣ�vh����ԭ�������� ���������0.5��uvȡֵ��Χ0~1��
        }
        return uvs;
    }
    protected void AddVert(Vector2 pos0, float tw, float th, VertexHelper vh, UIVertex vert)
    {
        vert.position = pos0;
        vert.uv0 = GetTextureUVS(new[] { new Vector2(pos0.x, pos0.y) }, tw, th)[0];
        vh.AddVert(vert);
    }

    /// <summary>
    /// ����߽�
    /// </summary>
    private void CalcBounds()
    {
        if (inner_trans == null)
        {
            return;
        }

        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(outer_trans, inner_trans);
        inner_rt = bounds.max;
        inner_lb = bounds.min;
        outer_rt = outer_trans.rect.max;
        outer_lb = outer_trans.rect.min;
    }

    private void Update()
    {
        if (realtimeRefresh == false)
        {
            return;
        }

        //����߽�
        CalcBounds();
        //ˢ��
        SetAllDirty();
    }
}
