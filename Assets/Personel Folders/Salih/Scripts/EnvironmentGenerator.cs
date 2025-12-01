using UnityEngine;
using UnityEngine.U2D;

public class EnvironmentChunk : MonoBehaviour
{
    [SerializeField] private SpriteShapeController _spriteShapeController;
    [SerializeField] private int _levelLength = 50;
    [SerializeField] private float _xMultiplier = 2f;
    [SerializeField] private float _yMultiplier = 2f;
    [SerializeField] private float _curveSmoothness = 0.5f;
    [SerializeField] private float _bottom = 10f;

    // This generates the ground based on a "Noise Offset" so the wave is continuous
    public void GenerateChunk(float noiseStep, float globalNoiseOffset)
    {
        var spline = _spriteShapeController.spline;
        spline.Clear();

        Vector3 lastPos = Vector3.zero;

        // Generate the top terrain points
        for (int i = 0; i < _levelLength; i++)
        {
            // We use (globalNoiseOffset + i) so this chunk continues exactly where the last one ended
            float currentNoiseX = (globalNoiseOffset + i) * noiseStep;

            float y = Mathf.PerlinNoise(0, currentNoiseX) * _yMultiplier;
            lastPos = new Vector3(i * _xMultiplier, y, 0);

            spline.InsertPointAt(i, lastPos);

            // Smooth tangents
            if (i != 0 && i != _levelLength - 1)
            {
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                spline.SetLeftTangent(i, Vector3.left * _xMultiplier * _curveSmoothness);
                spline.SetRightTangent(i, Vector3.right * _xMultiplier * _curveSmoothness);
            }
        }

        // Close the bottom of the shape
        spline.InsertPointAt(_levelLength, new Vector3(lastPos.x, -_bottom, 0));
        spline.InsertPointAt(_levelLength + 1, new Vector3(0, -_bottom, 0));

        // Update the collider
        _spriteShapeController.BakeCollider();
    }

    // Helper to tell the manager how wide this chunk is
    public float GetChunkWidth()
    {
        return (_levelLength - 1) * _xMultiplier;
    }
}