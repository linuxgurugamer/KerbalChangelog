﻿using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KerbalChangelog
{
	public class ChangelogVersion : IComparable
	{
		bool versionNull = false;
		bool malformedVersionString = false;

		public int major { get; private set; }
		public int minor { get; private set; }
		public int patch { get; private set; }
		public int build { get; private set; }

		public string versionName { get; private set; } = null;
		bool versionNameExists
		{
			get
			{
				if (versionName == null)
					return false;
				return true;
			}
		}

		public ChangelogVersion(int maj, int min, int pat, int bui)
		{
			major = maj;
			minor = min;
			patch = pat;
			build = bui;
		}
		public ChangelogVersion(int maj, int min, int pat, int bui, string vName) : this(maj, min, pat, bui)
		{
			versionName = vName;
		}
		// May fail, needs a try/catch
		public ChangelogVersion(string version, string cfgDirName)
		{
			if (version == "null")
			{
				versionNull = true;
				return;
			}

			Regex pattern = new Regex("(\\d+\\.\\d+\\.\\d+(\\.\\d+)?)"); //matches version numbers starting at the beginning to the end of the string
			Regex malformedPattern = new Regex("\\d+\\.\\d+(\\.\\d+)?(\\.\\d+)?");
			if (!pattern.IsMatch(version))
			{
				if (!malformedPattern.IsMatch(version))
				{
					Debug.Log("[KCL] broken version string: " + version);
					throw new ArgumentException("version is not a valid version");
				}
				Debug.Log("[KCL] malformed version string: " + version + " in directory " + cfgDirName);
				malformedVersionString = true;
			}
			string[] splitVersions = version.Split('.');

			major = int.Parse(splitVersions[0]);
			minor = int.Parse(splitVersions[1]);
			if (!malformedVersionString)
				patch = int.Parse(splitVersions[2]);
			else
				patch = 0;
			if (splitVersions.Length > 3)
				build = int.Parse(splitVersions[3]);
			else
				build = 0;
		}
		public ChangelogVersion(string version, string cfgDirName, string vName) : this(version, cfgDirName)
		{
			versionName = vName;
		}

		public override string ToString()
		{
			if (versionNull)
				return "D.N.E";
			return $"{major}.{minor}.{patch}.{build}" + (versionNameExists ? " \"" + versionName + "\"" : "");
		}
		public string ToStringPure()
		{
			if (versionNull)
				return "D.N.E";
			return $"{major}.{minor}.{patch}.{build}";
		}

		//This comparator will sort objects from highest version to lowest version
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;

			if (obj is ChangelogVersion oCLV)
			{
				if (oCLV.major - this.major == 0)
				{
					if (oCLV.minor - this.minor == 0)
					{
						if (oCLV.patch - this.patch == 0)
						{
							return oCLV.build.CompareTo(this.build);
						}
						return oCLV.patch.CompareTo(this.patch);
					}
					return oCLV.minor.CompareTo(this.minor);
				}
				return oCLV.major.CompareTo(this.major);
			}
			else
				throw new ArgumentException("Object is not a ChangelogVersion");
		}
	}
}