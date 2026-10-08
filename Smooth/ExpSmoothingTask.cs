using System;
using System.Collections.Generic;

namespace yield
{
    public static class ExpSmoothingTask
    {
        public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (alpha < 0 || alpha > 1)
                throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha должен быть в диапазоне (0, 1]");

            var isFirst = true;
            double previousSmoothedY = 0;

            foreach (var point in data)
            {
                if (isFirst)
                {
                    previousSmoothedY = point.OriginalY;
                    isFirst = false;
                    yield return point.WithExpSmoothedY(previousSmoothedY);
                }
                else
                {
                    var smoothedY = alpha * point.OriginalY + (1 - alpha) * previousSmoothedY;
                    previousSmoothedY = smoothedY;
                    yield return point.WithExpSmoothedY(smoothedY);
                }
            }
        }
    }
}