using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Clones;

[TestFixture]
public class CloneVersionSystem_should
{
	[Test]
	public void Learn()
	{
		var clone1 = Execute("learn 1 45", "check 1").Single();
		Assert.That(clone1, Is.EqualTo("45"));
	}

	[Test]
	public void RollbackToBasic()
	{
		var clone1 = Execute("learn 1 45", "rollback 1", "check 1").Single();
		Assert.That(clone1, Is.EqualTo("basic"));
	}

	[Test]
	public void RollbackToPreviousProgram()
	{
		var clone1 = Execute("learn 1 45", "learn 1 100500", "rollback 1", "check 1").Single();
		Assert.That(clone1, Is.EqualTo("45"));
	}

	[Test]
	public void RelearnAfterRollback()
	{
		var clone1 = Execute("learn 1 45", "rollback 1", "relearn 1", "check 1").Single();
		Assert.That(clone1, Is.EqualTo("45"));
	}

	[Test]
	public void CloneBasic()
	{
		var clone2 = Execute("clone 1", "check 2").Single();
		Assert.That(clone2, Is.EqualTo("basic"));
	}

	[Test]
	public void CloneLearned()
	{
		var clone2 = Execute("learn 1 42", "clone 1", "check 2").Single();
		Assert.That(clone2, Is.EqualTo("42"));
	}

	[Test]
	public void LearnClone_DontChangeOriginal()
	{
		var res = Execute("learn 1 42", "clone 1", "learn 2 100500", "check 1", "check 2");
		Assert.That(res, Is.EquivalentTo(new[]
		{
			"42",
			"100500"
		}));
	}

	[Test]
	public void RollbackClone_DontChangeOriginal()
	{
		var res = Execute("learn 1 42", "clone 1", "rollback 2", "check 1", "check 2");
		Assert.That(res, Is.EquivalentTo(new[]
		{
			"42",
			"basic"
		}));
	}

	[Test]
	public void ExecuteSample()
	{
		var res = Execute("learn 1 5",
			"learn 1 7",
			"rollback 1",
			"check 1",
			"clone 1",
			"relearn 2",
			"check 2",
			"rollback 1",
			"check 1");
		Assert.That(res, Is.EquivalentTo(new[]
		{
			"5",
			"7",
			"basic"
		}));
	}

	private List<string> Execute(params string[] queries)
	{
		var cvs = Factory.CreateCVS();
		var results = new List<string>();
		foreach (var command in queries)
		{
			var result = cvs.Execute(command);
			if (result != null) results.Add(result);
		}
		return results;
	}
}