using System.Linq;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace yield;

[TestFixture]
public class MovingAverageTests
{
	[Test]
	public void SmoothEmptySequence()
	{
		CheckAverage(10, [], []);
	}

	[Test]
	public void SmoothSingleZero()
	{
		CheckAverage(10, [0.0], [0.0]);
	}

	[Test]
	public void SmoothSingleNonZeroValue()
	{
		CheckAverage(10, [100.0], [100.0]);
	}

	[Test]
	public void SmoothTwoValues()
	{
		CheckAverage(2, [0, 20.0], [0, 10.0]);
	}

	[Test]
	public void SmoothTwoValues2()
	{
		CheckAverage(2, [10, 0.0], [10, 5.0]);
	}

	[Test]
	public void SmoothTwoValuesWithSmallWindow()
	{
		CheckAverage(1, [10, 0.0], [10, 0.0]);
	}

	[Test]
	public void SmoothWithWindow2()
	{
		CheckAverage(2, [1, 2, 3, 4, 5, 6], [1, 1.5, 2.5, 3.5, 4.5, 5.5]);
	}

	[Test]
	public void SmoothWithWindow3()
	{
		CheckAverage(3, [3, 3, 3, 6, 6, 6], [3, 3, 3, 4, 5, 6]);
	}

	[Test]
	public void SmoothWithLargeWindow()
	{
		CheckAverage(100500, [1, 2, 3, 4, 5, 6], [1, 1.5, 2, 2.5, 3, 3.5]);
	}

	private void CheckAverage(int windowWidth, double[] ys, double[] expectedYs)
	{
		var dataPoints = ys.Select((v, index) => new DataPoint(GetX(index), v));
		var actual = Factory.CreateAnalyzer().MovingAverage(dataPoints, windowWidth).ToList();
		Assert.AreEqual(ys.Length, actual.Count);
		for (var i = 0; i < actual.Count; i++)
		{
			Assert.AreEqual(GetX(i), actual[i].X, 1e-7);
			Assert.AreEqual(ys[i], actual[i].OriginalY, 1e-7);
			Assert.AreEqual(expectedYs[i], actual[i].AvgSmoothedY, 1e-7);
		}
	}

	private double GetX(int index)
	{
		return (index - 3.0) / 2;
	}
}