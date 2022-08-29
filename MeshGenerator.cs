using Godot;
using System;

public class MeshGenerator : MeshInstance2D {


    public override void _Ready() {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.AddColor(new Color(0, 1, 0));
        st.AddUv(new Vector2(0, 0));
        st.AddVertex(new Vector3(0, 0, 0));
        st.AddUv(new Vector2(0, 1));
        st.AddVertex(new Vector3(0, 100, 0));
        st.AddUv(new Vector2(1, 1));
        st.AddVertex(new Vector3(100, 100, 0));
        Mesh = st.Commit();
    }
}
