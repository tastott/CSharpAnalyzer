﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;

namespace CSharpAnalyzer.Test.Verifiers
{
    public abstract class FixableAnalysisRuleVerifier : CodeFixVerifier
    {
        private readonly FixableAnalysisRule rule;

        internal FixableAnalysisRuleVerifier(FixableAnalysisRule rule)
        {
            this.rule = rule;
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CSharpAnalyzerAnalyzer(this.rule);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CSharpAnalyzerCodeFixProvider(this.rule);
        }
    }
}
