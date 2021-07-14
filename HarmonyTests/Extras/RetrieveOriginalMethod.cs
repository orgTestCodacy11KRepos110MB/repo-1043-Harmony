using HarmonyLib;
using NUnit.Framework;
using System.Diagnostics;
using System.Reflection;

namespace HarmonyLibTests.Extras
{
	[TestFixture]
	class RetrieveOriginalMethod : TestLogger
	{
		[Test]
		public void Test0()
		{
			var harmony = new Harmony("test-original-method");

			var originalMethod = AccessTools.Method(typeof(RetrieveOriginalMethod), nameof(RetrieveOriginalMethod.PatchTarget));
			var dummyPrefix = AccessTools.Method(typeof(RetrieveOriginalMethod), nameof(RetrieveOriginalMethod.DummyPrefix));

			_ = harmony.Patch(originalMethod, new HarmonyMethod(dummyPrefix));

			PatchTarget();
		}

		private static void ChecksStackTrace()
		{
			var st = new StackTrace(1, false);
			var method = Harmony.GetMethodFromStackframe(st.GetFrame(0));

			// Replacement will be HarmonyLibTests.Extras.RetrieveOriginalMethod.PatchTarget_Patch1
			// We should be able to go from this method, back to HarmonyLibTests.Extras.PatchTarget
			if (method is MethodInfo replacement)
			{
				var original = Harmony.GetOriginalMethod(replacement);
				Assert.NotNull(original);
				Assert.Equals(original, AccessTools.Method(typeof(RetrieveOriginalMethod), nameof(RetrieveOriginalMethod.PatchTarget)));
			}
		}

		internal static void PatchTarget()
		{
			ChecksStackTrace();
		}

		internal static void DummyPrefix()
		{

		}
	}
}
