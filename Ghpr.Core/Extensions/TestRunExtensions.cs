﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Utils;
using Newtonsoft.Json;

namespace Ghpr.Core.Extensions
{
    public static class TestRunExtensions
    {
        public static ITestRun TakeScreenshot(this ITestRun testRun, string testPath, bool takeScreenshot)
        {
            if (takeScreenshot && testRun.FailedOrBroken)
            {
                var date = DateTime.Now;
                var s = new TestScreenshot(date);
                Taker.TakeScreenshot(Path.Combine(testPath, "img"), date);
                testRun.Screenshots.Add(s);
            }
            return testRun;
        }

        public static ITestRun Update(this ITestRun target, ITestRun run)
        {
            if (target.TestGuid.Equals(Guid.Empty))
            {
                target.TestGuid = GuidConverter.ToMd5HashGuid(target.FullName);
            }
            target.Screenshots.AddRange(run.Screenshots);
            target.Events.AddRange(run.Events);
            return target;
        }

        public static string GetFileName(this ITestRun testRun)
        {
            return testRun.DateTimeFinish.GetTestName();
        }
        
        public static void Save(this ITestRun testRun, string path, string name = "")
        {
            if (name.Equals(""))
            {
                name = testRun.GetFileName();
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fullPath = Path.Combine(path, name);
            using (var file = File.CreateText(fullPath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, testRun);
            }
        }
        
        public static ITestRun GetTest(this List<ITestRun> testRuns, ITestRun testRun)
        {
            return testRuns.FirstOrDefault(t => t.TestGuid.Equals(testRun.TestGuid) && !t.TestGuid.Equals(Guid.Empty))
                ?? testRuns.FirstOrDefault(t => t.FullName.Equals(testRun.FullName))
                ?? testRuns.FirstOrDefault(t => t.Name.Equals(testRun.Name)) 
                ?? new TestRun();
        }
    }
}