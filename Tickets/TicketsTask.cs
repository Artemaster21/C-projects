using System.Numerics;

namespace Tickets;

public class TicketsTask
{
    public static BigInteger Solve(int halfLen, int totalSum)
    {
        if (totalSum % 2 != 0)
            return 0;

        var halfSum = totalSum / 2;

        if (halfSum < 0 || halfSum > 9 * halfLen)
            return 0;

        var dp = new BigInteger[halfLen + 1, halfSum + 1];

        dp[0, 0] = 1;

        for (var k = 1; k <= halfLen; k++)
        {
            for (var s = 0; s <= halfSum; s++)
            {
                for (var d = 0; d <= 9 && d <= s; d++)
                    dp[k, s] += dp[k - 1, s - d];
            }
        }

        var ways = dp[halfLen, halfSum];
        return ways * ways;
    }
}