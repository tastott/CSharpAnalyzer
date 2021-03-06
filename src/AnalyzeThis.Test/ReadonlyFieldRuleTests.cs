﻿using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using AnalyzeThis.ReadonlyField;
using AnalyzeThis.Test.Verifiers;

namespace AnalyzeThis.Test
{
    [TestClass]
    public class ReadonlyFieldRuleTests : FixableAnalysisRuleVerifier
    {
        public ReadonlyFieldRuleTests()
            : base(new ReadonlyFieldRule())
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
        public void DetectsUnassignedReadOnlyProperty()
        {
            var test = @"
    namespace MyNamespace
    {
        class MyClass
        {
            private readonly int blah;
            private readonly int foo;

            public MyClass(int foo)
            {
                this.foo = foo;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = ReadonlyFieldRule.Id,
                Message = $"Readonly field(s) not assigned in constructor: blah.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IgnoresChainedThisConstructor()
        {
            var test = @"
    namespace MyNamespace
    {
        class MyClass
        {
            private readonly int foo;

            public MyClass(int foo)
            {
                this.foo = foo;
            }

            public MyClass()
                : this(4)
            {
            }
        }
    }";

            VerifyNoCSharpDiagnostics(test);
        }

        [TestMethod]
        public void DoesntIgnoreChainedBaseConstructor()
        {
            var test = @"
    namespace MyNamespace
    {
        class BaseClass {}

        class MyClass : BaseClass
        {
            private readonly int foo;

            public MyClass(int foo)
                : base()
            {
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = ReadonlyFieldRule.Id,
                Message = $"Readonly field(s) not assigned in constructor: foo.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IgnoresFieldAssignedOnDeclaration()
        {
            var test = @"
    namespace MyNamespace
    {
        class MyClass
        {
            private readonly int foo = 1;

            public MyClass()
            {
            }
        }
    }";

            VerifyNoCSharpDiagnostics(test);
        }

        [TestMethod]
        public void FixAddsConstructorParameterAndAssignment()
        {
            var test = @"
namespace MyNamespace
{
    class MyClass
    {
        private readonly int foo;

        public MyClass()
        {
        }
    }
}";

            var expected = @"
namespace MyNamespace
{
    class MyClass
    {
        private readonly int foo;

        public MyClass(int foo)
        {
            this.foo = foo;
        }
    }
}";

            VerifyCSharpFix(test, expected);
        }

        [TestMethod]
        public void FixAddsConstructorParameterInLowerCase()
        {
            var test = @"
namespace MyNamespace
{
    class MyClass
    {
        public readonly int FooBar;

        public MyClass()
        {
        }
    }
}";

            var expected = @"
namespace MyNamespace
{
    class MyClass
    {
        public readonly int FooBar;

        public MyClass(int fooBar)
        {
            this.FooBar = fooBar;
        }
    }
}";

            VerifyCSharpFix(test, expected);
        }

        [TestMethod]
        public void FixAddsAssignmentOnlyIfParameterAlreadyPresent()
        {
            var test = @"
namespace MyNamespace
{
    class MyClass
    {
        public readonly int Foo;

        public MyClass(int foo)
        {
        }
    }
}";

            var expected = @"
namespace MyNamespace
{
    class MyClass
    {
        public readonly int Foo;

        public MyClass(int foo)
        {
            this.Foo = foo;
        }
    }
}";

            VerifyCSharpFix(test, expected);
        }

        [TestMethod]
        public void FixDoesNotAssignIfExistingParameterIsOfWrongType()
        {
            var test = @"
namespace MyNamespace
{
    class MyClass
    {
        public readonly int Foo;

        public MyClass(string foo)
        {
        }
    }
}";

            var expected = @"
namespace MyNamespace
{
    class MyClass
    {
        public readonly int Foo;

        public MyClass(string foo)
        {
        }
    }
}";

            VerifyCSharpFix(test, expected);
        }
    }
}