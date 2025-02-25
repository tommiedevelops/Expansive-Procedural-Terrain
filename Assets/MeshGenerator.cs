﻿using UnityEngine;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMeshDataFromHeightMap(float[,] noiseMap, NoiseSettings settings) {

        var envelope = new AnimationCurve(settings.amplitudeEnvelope.keys);
        int width = settings.width;
        int length = settings.length;
        int levelOfDetail = settings.previewLOD;
        float heightMultiplier = settings.heightMultiplier;

        // I don't get the below transformation. Why is this important? Maybe try without it after done. //demo
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0 ) ? 1 : levelOfDetail * 2;
        
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        int vertexIndex = 0; 

        var meshData = new MeshData(verticesPerLine, verticesPerLine);

        for (int y = 0; y < length; y+=meshSimplificationIncrement) {
            for (int x = 0; x < width; x+=meshSimplificationIncrement) {
                meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, envelope.Evaluate(noiseMap[x,y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs [vertexIndex] = new Vector2( x / (float)width, y / (float)length);

                if (x < width - 1 && y < width - 1) {
                    meshData.AddTriangle (vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle (vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }
}

public class MeshData {
    /* This class converts MeshData into a Mesh Object */
    public int[] triangles;
    public Vector3[] vertices;
    public Vector2[] uvs;

    int triangleIndex = 0;
    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }
    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }
    public Mesh CreateMesh() {
        var mesh = new Mesh {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}