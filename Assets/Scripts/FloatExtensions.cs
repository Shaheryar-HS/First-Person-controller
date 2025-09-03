using UnityEngine;

public static class FloatExtensions
{
    public static float SafeCustomFloat<T>(this T input)
    {
        float result = 0f;

        // Handle string or char inputs directly
        if (input is string || input is char)
        {
            return -99f;
        }

        // Handle numeric types manually
        if (input is float f)
        {
            result = f;
        }
        else if (input is int i)
        {
            result = i;
        }
        else if (input is double d)
        {
            result = (float)d;
        }
        else if (input is long l)
        {
            result = l;
        }
        else if (input is short s)
        {
            result = s;
        }
        else if (input is byte b)
        {
            result = b;
        }
        else
        {
            // Unsupported type
            return -99f;
        }

        // Return 0 if value is exactly -99
        return result == -99f ? 0f : result;
    }
}

