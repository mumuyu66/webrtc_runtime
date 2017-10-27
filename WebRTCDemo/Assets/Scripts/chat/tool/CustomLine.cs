using UnityEngine;
using System.Collections;

public class CustomLine : MonoBehaviour {
    public Material mat; //貼圖材質
    public GameObject startPos; //線段起點，使用物件主要是方便可以在編輯視窗中拖曳，或自己調整為Vector3
    public GameObject targetPos; //線段終點
    public static int count = 640; //節點數量

    private MeshFilter lineMeshFilter;
    private MeshRenderer lineMeshRenderer;
    private GameObject customLine; //建立一個物件來乘載MeshFilter跟MeshRenderer，避免跟Component掛載物件上的衝突

	void Start(){
		if(customLine == null)
		{
			for (int i = 0; i < c.Length; i++) {
				c [i] = new Vector3(i*2,0,0);
			}
			customLine = new GameObject ("CustomLine");
			customLine.transform.parent = this.transform;
			customLine.transform.localPosition = Vector3.zero;
			lineMeshFilter = customLine.AddComponent(typeof(MeshFilter)) as MeshFilter;
			lineMeshRenderer = customLine.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
			lineMeshRenderer.material = mat;
			Vector3[] points = CalculatePoints (startPos, targetPos, count);
			CreateMesh (points);
		}
	}
 
    public void OnUpdate () {
		Lightning();
	}
 
	private void Lightning()
	{
		index += dep;
		if (index >= count) {
			index = 0;
		}
		refreshPoints ();
		ModifyMesh (c);
	}

	private int index = 0;
	private int dep = 40;
	private Vector3[] c = new Vector3[count];
	private void refreshPoints(){
		for (int i = 0; i < dep; i++) {
			if (_data.Count > 0) {
				float n = ((float)_data.Dequeue ()) * 32767;
				if (n > 50) {
					n = 50;
				} else if (n < -50) {
					n = -50;
				}
				c [i + index].y =n;
//				Debug.Log (c [i + index].y);
			} else {
				c [i + index].y = 0;
			}
		}
	}

	private Queue _data = new Queue();
	public void SetData(float[] data){
		for (int i = 0; i < data.Length; i++) {
			_data.Enqueue (data[i]);
		}
	}
 
	private void ModifyMesh(Vector3[] points)
	{
	    //取得新的verts座標
	    Vector3[] verts = GetVerts (points);

	    //取得顏色資訊
	    Color[] colors = GetColor (verts.Length);

	    //使用新的verts跟顏色資訊調整現有的Mesh
	    Mesh lineMesh = lineMeshFilter.mesh;
	    lineMesh.vertices = verts;
	    lineMesh.colors = colors;
	}
 
    //計算新的線段座標點
	private Vector3[] CalculatePoints(GameObject from, GameObject to, int points)
	{
		Vector3[] result = new Vector3[points];
	
		for(int i = 0; i < count; ++i)
		{
			result[i] = new Vector3(i,0,0);
		}

		return result;
	}
 
    //建立Mesh
    private void CreateMesh(Vector3[] points)
    {
		Vector3[] verts = GetVerts (points); //取得vertices資訊
		Vector2[] UVs = new Vector2[(points.Length-1)*4];
		int[] tris = new int[(points.Length-1)*6];

		for(int i = 0; i < points.Length-1; ++i)
		{
		    //建立UV資訊
		    UVs[i*4] = new Vector2(0, 0);
		    UVs[i*4 + 1] = new Vector2(0, 1);
		    UVs[i*4 + 2] = new Vector2(1, 0);
		    UVs[i*4 + 3] = new Vector2(1, 1);

		    //建立三角面資訊
		    tris[i*6] = i*4 + 0;
		    tris[i*6 + 1] = i*4 + 2;
		    tris[i*6 + 2] = i*4 + 1;
		    tris[i*6 + 3] = i*4 + 1;
		    tris[i*6 + 4] = i*4 + 2;
		    tris[i*6 + 5] = i*4 + 3;
		}

		//取得顏色資訊
		Color[] colors = GetColor (verts.Length);

		//建立新的Mesh
		Mesh lineMesh = lineMeshFilter.mesh;
		lineMesh.vertices = verts;
		lineMesh.triangles = tris;
		lineMesh.uv = UVs;
		lineMesh.colors = colors;
		lineMesh.RecalculateBounds ();
		lineMesh.RecalculateNormals ();
	 }
 
    //使用AnimationCurve取得漸變顏色的資訊
    private Color[] GetColor(int count)
    {
		Color[] colors = new Color[count];
		for(int i = 0; i < colors.Length; ++i)
		{
			colors[i] = new Color(0,0,0,1);
		}
		return colors;
	}
 
	private Vector3[] GetVerts(Vector3[] points)
	{
		Vector3[] verts = new Vector3[(points.Length-1)*4];
		for(int i = 0; i < points.Length-1; ++i)
		{
			Vector3 start = points[i];
			Vector3 end = points[i+1];

			//計算線段起始點跟終點的寬
			float startPosWidth = 1f;
			float endPosWidth = 1f;

			//計算偏移的向量
			Vector3 dir = end - start;
			float theda = Mathf.Atan2 (dir.y, dir.x);
			Vector3 widthStartDir = new Vector3(Mathf.Sin (theda) * startPosWidth, -Mathf.Cos(theda) * startPosWidth, 0);
			Vector3 widthEndDir = new Vector3(Mathf.Sin (theda) * endPosWidth, -Mathf.Cos(theda) * endPosWidth, 0);

			//使用向量取得這線段Mesh的四個角的座標點
			Vector3 leftStartPos = start - widthStartDir;
			Vector3 rightStartPos = start + widthStartDir;
			Vector3 leftEndPos = end - widthEndDir;
			Vector3 rightEndPos = end + widthEndDir;
			verts[i*4] = leftStartPos;
			verts[i*4 + 1] = rightStartPos;
			verts[i*4 + 2] = leftEndPos;
			verts[i*4 + 3] = rightEndPos;
		}
		return verts;
	}
}
