﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests.Emit
{
    public class UnmanagedTypeModifierTests : CSharpTestBase
    {
        [Fact]
        public void LoadingADifferentModifierTypeForUnmanagedConstraint()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M1<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M1

  .method public hidebysig instance void
          M2<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.InAttribute)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M2

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        var obj = new TestRef();

        obj.M1<int>();      // valid
        obj.M2<int>();      // invalid
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (9,13): error CS0570: 'T' is not supported by the language
                //         obj.M2<int>();      // invalid
                Diagnostic(ErrorCode.ERR_BindToBogus, "M2<int>").WithArguments("T").WithLocation(9, 13)
                );
        }

        [Fact]
        public void LoadingUnmanagedTypeModifier_OptionalIsError()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M1<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M1

  .method public hidebysig instance void
          M2<valuetype .ctor (class [mscorlib]System.ValueType modopt([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M2

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        var obj = new TestRef();

        obj.M1<int>();      // valid
        obj.M2<int>();      // invalid
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (9,13): error CS0570: 'T' is not supported by the language
                //         obj.M2<int>();      // invalid
                Diagnostic(ErrorCode.ERR_BindToBogus, "M2<int>").WithArguments("T").WithLocation(9, 13)
                );
        }

        [Fact]
        public void LoadingUnmanagedTypeModifier_ModreqWithoutAttribute()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M1<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M1

  .method public hidebysig instance void
          M2<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M2

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        var obj = new TestRef();

        obj.M1<int>();      // valid
        obj.M2<int>();      // invalid
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (9,13): error CS0570: 'T' is not supported by the language
                //         obj.M2<int>();      // invalid
                Diagnostic(ErrorCode.ERR_BindToBogus, "M2<int>").WithArguments("T").WithLocation(9, 13)
                );
        }

        [Fact]
        public void LoadingUnmanagedTypeModifier_AttributeWithoutModreq()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M1<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M1

  .method public hidebysig instance void
          M2<valuetype .ctor (class [mscorlib]System.ValueType) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M2

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        var obj = new TestRef();

        obj.M1<int>();      // valid
        obj.M2<int>();      // invalid
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (9,13): error CS0570: 'T' is not supported by the language
                //         obj.M2<int>();      // invalid
                Diagnostic(ErrorCode.ERR_BindToBogus, "M2<int>").WithArguments("T").WithLocation(9, 13)
                );
        }

        [Fact]
        public void ProperErrorsArePropagatedIfModreqTypeIsNotAvailable_Class()
        {
            var code = @"
namespace System
{
    public class Object {}
    public class Void {}
    public class ValueType {}
}
class Test<T> where T : unmanaged
{
}";

            CreateEmptyCompilation(code).VerifyDiagnostics(
                // (8,25): error CS0518: Predefined type 'System.Runtime.InteropServices.UnmanagedType' is not defined or imported
                // class Test<T> where T : unmanaged
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.Runtime.InteropServices.UnmanagedType").WithLocation(8, 25));
        }

        [Fact]
        public void ProperErrorsArePropagatedIfModreqTypeIsNotAvailable_Method()
        {
            var code = @"
namespace System
{
    public class Object {}
    public class Void {}
    public class ValueType {}
}
class Test
{
    public void M<T>() where T : unmanaged {}
}";

            CreateEmptyCompilation(code).VerifyDiagnostics(
                // (10,34): error CS0518: Predefined type 'System.Runtime.InteropServices.UnmanagedType' is not defined or imported
                //     public void M<T>() where T : unmanaged {}
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.Runtime.InteropServices.UnmanagedType").WithLocation(10, 34));
        }

        [Fact]
        public void ProperErrorsArePropagatedIfModreqTypeIsNotAvailable_Delegate()
        {
            var code = @"
namespace System
{
    public class Object {}
    public class Void {}
    public class ValueType {}
    public class IntPtr {}
    public class MulticastDelegate {}
}
public delegate void D<T>() where T : unmanaged;";

            CreateEmptyCompilation(code).VerifyDiagnostics(
                // (10,39): error CS0518: Predefined type 'System.Runtime.InteropServices.UnmanagedType' is not defined or imported
                // public delegate void D<T>() where T : unmanaged;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.Runtime.InteropServices.UnmanagedType").WithLocation(10, 39));
        }

        [Fact]
        public void ProperErrorsArePropagatedIfValueTypeIsNotAvailable_Class()
        {
            var code = @"
namespace System
{
    public class Object {}
    public class Void {}

    namespace Runtime
    {
        namespace InteropServices
        {
            public class UnmanagedType {}
        }
    }
}
class Test<T> where T : unmanaged
{
}";

            CreateEmptyCompilation(code).VerifyDiagnostics(
                // (15,25): error CS0518: Predefined type 'System.ValueType' is not defined or imported
                // class Test<T> where T : unmanaged
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.ValueType").WithLocation(15, 25));
        }

        [Fact]
        public void ProperErrorsArePropagatedIfValueTypeIsNotAvailable_Method()
        {
            var code = @"
namespace System
{
    public class Object {}
    public class Void {}

    namespace Runtime
    {
        namespace InteropServices
        {
            public class UnmanagedType {}
        }
    }
}
class Test
{
    public void M<T>() where T : unmanaged {}
}";

            CreateEmptyCompilation(code).VerifyDiagnostics(
                // (17,34): error CS0518: Predefined type 'System.ValueType' is not defined or imported
                //     public void M<T>() where T : unmanaged {}
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.ValueType").WithLocation(17, 34));
        }

        [Fact]
        public void ProperErrorsArePropagatedIfValueTypeIsNotAvailable_Delegate()
        {
            var code = @"
namespace System
{
    public class Object {}
    public class Void {}
    public class IntPtr {}
    public class MulticastDelegate {}

    namespace Runtime
    {
        namespace InteropServices
        {
            public class UnmanagedType {}
        }
    }
}
public delegate void M<T>() where T : unmanaged;";

            CreateEmptyCompilation(code).VerifyDiagnostics(
                // (17,39): error CS0518: Predefined type 'System.ValueType' is not defined or imported
                // public delegate void M<T>() where T : unmanaged;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.ValueType").WithLocation(17, 39));
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Virtual_Compilation()
        {
            var reference = CompileAndVerify(@"
public class Parent
{
    public virtual string M<T>() where T : unmanaged => ""Parent"";
}
public class Child : Parent
{
    public override string M<T>() => ""Child"";
}", symbolValidator: module =>
            {
                var parentTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(parentTypeParameter.HasValueTypeConstraint);
                Assert.True(parentTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, parentTypeParameter, module.ContainingAssembly.Name);

                var childTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(childTypeParameter.HasValueTypeConstraint);
                Assert.True(childTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, childTypeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Parent().M<int>());
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { reference.Compilation.EmitToImageReference() }, expectedOutput: @"
Parent
Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Virtual_Reference()
        {
            var parent = CompileAndVerify(@"
public class Parent
{
    public virtual string M<T>() where T : unmanaged => ""Parent"";
}", symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });


            var child = CompileAndVerify(@"
public class Child : Parent
{
    public override string M<T>() => ""Child"";
}", references: new[] { parent.Compilation.EmitToImageReference() }, symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Parent().M<int>());
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { parent.Compilation.EmitToImageReference(), child.Compilation.EmitToImageReference() }, expectedOutput: @"
Parent
Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Abstract_Compilation()
        {
            var reference = CompileAndVerify(@"
public abstract class Parent
{
    public abstract string M<T>() where T : unmanaged;
}
public class Child : Parent
{
    public override string M<T>() => ""Child"";
}", symbolValidator: module =>
            {
                var parentTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(parentTypeParameter.HasValueTypeConstraint);
                Assert.True(parentTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, parentTypeParameter, module.ContainingAssembly.Name);

                var childTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(childTypeParameter.HasValueTypeConstraint);
                Assert.True(childTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, childTypeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { reference.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Abstract_Reference()
        {
            var parent = CompileAndVerify(@"
public abstract class Parent
{
    public abstract string M<T>() where T : unmanaged;
}", symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });


            var child = CompileAndVerify(@"
public class Child : Parent
{
    public override string M<T>() => ""Child"";
}", references: new[] { parent.Compilation.EmitToImageReference() }, symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { parent.Compilation.EmitToImageReference(), child.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Interface_Implicit_Nonvirtual_Compilation()
        {
            var reference = CompileAndVerify(@"
public interface Parent
{
    string M<T>() where T : unmanaged;
}
public class Child : Parent
{
    public string M<T>() where T : unmanaged => ""Child"";
}", symbolValidator: module =>
            {
                var parentTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(parentTypeParameter.HasValueTypeConstraint);
                Assert.True(parentTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, parentTypeParameter, module.ContainingAssembly.Name);

                var childTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(childTypeParameter.HasValueTypeConstraint);
                Assert.True(childTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, childTypeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { reference.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Interface_Implicit_Nonvirtual_Reference()
        {
            var parent = CompileAndVerify(@"
public interface Parent
{
    string M<T>() where T : unmanaged;
}", symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });


            var child = CompileAndVerify(@"
public class Child : Parent
{
    public string M<T>() where T : unmanaged => ""Child"";
}", references: new[] { parent.Compilation.EmitToImageReference() }, symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { parent.Compilation.EmitToImageReference(), child.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Interface_Implicit_Virtual_Compilation()
        {
            var reference = CompileAndVerify(@"
public interface Parent
{
    string M<T>() where T : unmanaged;
}
public class Child : Parent
{
    public virtual string M<T>() where T : unmanaged => ""Child"";
}", symbolValidator: module =>
            {
                var parentTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(parentTypeParameter.HasValueTypeConstraint);
                Assert.True(parentTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, parentTypeParameter, module.ContainingAssembly.Name);

                var childTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(childTypeParameter.HasValueTypeConstraint);
                Assert.True(childTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, childTypeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { reference.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Interface_Implicit_Virtual_Reference()
        {
            var parent = CompileAndVerify(@"
public interface Parent
{
    string M<T>() where T : unmanaged;
}", symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });


            var child = CompileAndVerify(@"
public class Child : Parent
{
    public virtual string M<T>() where T : unmanaged => ""Child"";
}", references: new[] { parent.Compilation.EmitToImageReference() }, symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        System.Console.WriteLine(new Child().M<int>());
    }
}", references: new[] { parent.Compilation.EmitToImageReference(), child.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Interface_Explicit_Compilation()
        {
            var reference = CompileAndVerify(@"
public interface Parent
{
    string M<T>() where T : unmanaged;
}
public class Child : Parent
{
    string Parent.M<T>() => ""Child"";
}", symbolValidator: module =>
            {
                var parentTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(parentTypeParameter.HasValueTypeConstraint);
                Assert.True(parentTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, parentTypeParameter, module.ContainingAssembly.Name);

                var childTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("Parent.M").TypeParameters.Single();
                Assert.True(childTypeParameter.HasValueTypeConstraint);
                Assert.True(childTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, childTypeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        Parent obj = new Child();
        System.Console.WriteLine(obj.M<int>());
    }
}", references: new[] { reference.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToOverrides_Interface_Explicit_Reference()
        {
            var parent = CompileAndVerify(@"
public interface Parent
{
    string M<T>() where T : unmanaged;
}", symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Parent").GetMethod("M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });


            var child = CompileAndVerify(@"
public class Child : Parent
{
    string Parent.M<T>() => ""Child"";
}", references: new[] { parent.Compilation.EmitToImageReference() }, symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Child").GetMethod("Parent.M").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });

            CompileAndVerify(@"
class Program
{
    public static void Main()
    {
        Parent obj = new Child();
        System.Console.WriteLine(obj.M<int>());
    }
}", references: new[] { parent.Compilation.EmitToImageReference(), child.Compilation.EmitToImageReference() }, expectedOutput: "Child");
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToLambda_Compilation()
        {
            CompileAndVerify(@"
public delegate T D<T>() where T : unmanaged;
public class TestRef
{
    public static void Print<T>(D<T> lambda) where T : unmanaged
    {
        System.Console.WriteLine(lambda());
    }
}
public class Program
{
    static void Test<T>(T arg)  where T : unmanaged
    {
        TestRef.Print(() => arg);
    }
    
    public static void Main()
    {
        Test(5);
    }
}",
                expectedOutput: "5",
                options: TestOptions.ReleaseExe.WithMetadataImportOptions(MetadataImportOptions.All),
                symbolValidator: module =>
            {
                var delegateTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("D`1").TypeParameters.Single();
                Assert.True(delegateTypeParameter.HasValueTypeConstraint);
                Assert.True(delegateTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, delegateTypeParameter, module.ContainingAssembly.Name);

                var lambdaTypeParameter = module.ContainingAssembly.GetTypeByMetadataName("Program").GetTypeMember("<>c__DisplayClass0_0").TypeParameters.Single();
                Assert.True(lambdaTypeParameter.HasValueTypeConstraint);
                Assert.True(lambdaTypeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, lambdaTypeParameter, module.ContainingAssembly.Name);
            });
        }

        [Fact]
        public void UnmanagedTypeModreqIsCopiedToLambda_Reference()
        {
            var reference = CompileAndVerify(@"
public delegate T D<T>() where T : unmanaged;
public class TestRef
{
    public static void Print<T>(D<T> lambda) where T : unmanaged
    {
        System.Console.WriteLine(lambda());
    }
}", symbolValidator: module =>
            {
                var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("D`1").TypeParameters.Single();
                Assert.True(typeParameter.HasValueTypeConstraint);
                Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
            });


            CompileAndVerify(@"
public class Program
{
    static void Test<T>(T arg)  where T : unmanaged
    {
        TestRef.Print(() => arg);
    }
    
    public static void Main()
    {
        Test(5);
    }
}",
                expectedOutput: "5",
                references: new[] { reference.Compilation.EmitToImageReference() },
                options: TestOptions.ReleaseExe.WithMetadataImportOptions(MetadataImportOptions.All),
                symbolValidator: module =>
                {
                    var typeParameter = module.ContainingAssembly.GetTypeByMetadataName("Program").GetTypeMember("<>c__DisplayClass0_0").TypeParameters.Single();
                    Assert.True(typeParameter.HasValueTypeConstraint);
                    Assert.True(typeParameter.HasUnmanagedTypeConstraint);

                    AttributeTests_IsUnmanaged.AssertReferencedIsUnmanagedAttribute(Accessibility.Internal, typeParameter, module.ContainingAssembly.Name);
                });
        }

        [Fact]
        public void DuplicateUnmanagedTypeInReferences()
        {
            var refCode = @"
namespace System.Runtime.InteropServices
{
    public class UnmanagedType {}
}";

            var ref1 = CreateCompilation(refCode).EmitToImageReference();
            var ref2 = CreateCompilation(refCode).EmitToImageReference();

            var user = @"
public class Test<T> where T : unmanaged
{
}";

            CreateCompilation(user, references: new[] { ref1, ref2 }).VerifyDiagnostics(
                // (2,32): error CS0518: Predefined type 'System.Runtime.InteropServices.UnmanagedType' is not defined or imported
                // public class Test<T> where T : unmanaged
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "unmanaged").WithArguments("System.Runtime.InteropServices.UnmanagedType").WithLocation(2, 32));
        }

        [Fact]
        public void UnmanagedConstraintWithClassConstraint_IL()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M<class (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        new TestRef().M<int>();
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (6,23): error CS0570: 'T' is not supported by the language
                //         new TestRef().M<int>();
                Diagnostic(ErrorCode.ERR_BindToBogus, "M<int>").WithArguments("T").WithLocation(6, 23)
                );
        }

        [Fact]
        public void UnmanagedConstraintWithConstructorConstraint_IL()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M<.ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        new TestRef().M<int>();
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (6,23): error CS0570: 'T' is not supported by the language
                //         new TestRef().M<int>();
                Diagnostic(ErrorCode.ERR_BindToBogus, "M<int>").WithArguments("T").WithLocation(6, 23)
                );
        }

        [Fact]
        public void UnmanagedConstraintWithTypeConstraint_IL()
        {
            var ilSource = IsUnmanagedAttributeIL + @"
.class public auto ansi beforefieldinit TestRef
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void
          M<valuetype .ctor(class [mscorlib]System.IComparable, class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) T>() cil managed
  {
    .param type T
    .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestRef::M

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestRef::.ctor

}";

            var reference = CompileIL(ilSource, prependDefaultHeader: false);

            var code = @"
public class Test
{
    public static void Main()
    {
        new TestRef().M<int>();
    }
}";

            CreateCompilation(code, references: new[] { reference }).VerifyDiagnostics(
                // (6,23): error CS0570: 'T' is not supported by the language
                //         new TestRef().M<int>();
                Diagnostic(ErrorCode.ERR_BindToBogus, "M<int>").WithArguments("T").WithLocation(6, 23)
                );
        }

        private const string IsUnmanagedAttributeIL = @"
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly Test
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78 63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Test.dll
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003
.corflags 0x00000001

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  }
}

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.IsUnmanagedAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  }
}
";
    }
}