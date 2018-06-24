using System;
using System.Collections;

public class SparkUtilities
{
	public static T Cast<T>(object input) where T : class
	{
		if (input is T)
		{
			return (input as T);
		}
		return null;
	}
}