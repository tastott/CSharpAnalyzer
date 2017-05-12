﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpAnalyzer.Test.Verifiers;
using CSharpAnalyzer.Todo;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace CSharpAnalyzer.Test
{
    [TestClass]
    public class TodoRuleTests : AnalysisRuleVerifier
    {
        public TodoRuleTests()
            : base(new TodoRule())
        {

        }

        //No diagnostics expected to show up
        [TestMethod]
        public void EmptyFile()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void DetectsTodoCommentOnSeparateLine()
        {
            var test = @"
namespace MyNamespace
{
    class MyClass
    {
        public MyClass(int foo)
        {
            // TODO: Blah, blah, etc.
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "Todo",
                Message = $"TODO: Blah, blah, etc.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
    }
}
