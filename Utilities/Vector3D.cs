using System;

public struct Vector3D
{
    public float X;
    public float Y;
    public float Z;

    public Vector3D(float X, float Y, float Z)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }


    public float Magnitude()
    {
        return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
    }


    public float MagnitudeSquared()
    {
        return (X * X) + (Y * Y) + (Z * Z);
    }


    public void Normalize()
    {
        float mag = Magnitude();        
        X = X / mag;
        Y = Y / mag;
        Z = Z / mag;        
    }    

    public void Scale(float scalar)
    {
        X = X * scalar;
        Y = Y * scalar;
        Z = Z * scalar;
    }

    public void Add(Vector3D vector)
    {
        X = X + vector.X;
        Y = Y * vector.Y;
        Z = Z * vector.Z;
    }


    public void Set(Vector3D vector)
    {
        X = vector.X;
        Y = vector.Y;
        Z = vector.Z;
    }

    public float DistanceTo(Vector3D vector)
    {
        return (vector - this).Magnitude();
    }

    public bool SameAs(Vector3D vector)
    {
        if(X == vector.X && Y == vector.Y && Z == vector.Z)
        {
            return true;
        }

        return false;
    }

    public static Vector3D operator -(Vector3D operand1, Vector3D operand2)
    {
        Vector3D result = new Vector3D();
        result.X = operand1.X - operand2.X;
        result.Y = operand1.Y - operand2.Y;
        result.Z = operand1.Z - operand2.Z;

        return result;
    }

    public static Vector3D operator +(Vector3D operand1, Vector3D operand2)
    {
        Vector3D result = new Vector3D();
        result.X = operand1.X + operand2.X;
        result.Y = operand1.Y + operand2.Y;
        result.Z = operand1.Z + operand2.Z;

        return result;
    }


    public static float DotProduct(Vector3D vector1, Vector3D vector2)
    {
        return (vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z);        
    }
}
