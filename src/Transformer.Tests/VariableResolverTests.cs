using System.Collections.Generic;
using System.Linq;
using Transformer.Model;
using Transformer.Template;
using Xunit;

namespace Transformer.Tests
{
    public class VariableResolverTests
    {
         [Fact]
         public void Resolve_One_Variable_Test()
         {
             var variableList = new List<Variable>() { new Variable() { Name = "Var1", Value = "Jack Bauer" } };

             var target = new VariableResolver(variableList);
             var result = target.TransformVariables("Hello ${Var1}!");

             Assert.Equal("Hello Jack Bauer!", result);
         }

        [Fact]
        public void Resolve_Multiple_Variables_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName} ${LastName}!");

            Assert.Equal("Hello Jack Bauer!", result);
        }

        [Fact]
        public void Resolve_On_Multiple_Lines_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                    new Variable() { Name = "SecondLine", Value = "2" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables(@"Hello ${FirstName} ${LastName}!\r\nThis is another line:${SecondLine}");

            Assert.Equal(@"Hello Jack Bauer!\r\nThis is another line:2", result);
        }

        [Fact]
        public void Resolve_With_Default_Value_Set_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName:Mila} ${LastName:Kunis}!");

            Assert.Equal("Hello Jack Bauer!", result);
        }

        [Fact]
        public void Resolve_Using_Default_Value_Test()
        {
            var variableList = new List<Variable>();

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName:Mila} ${LastName:Kunis}!");

            Assert.Equal("Hello Mila Kunis!", result);
        }

        [Fact]
        public void Resolve_Variable_In_Variable_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Mila" },
                    new Variable() { Name = "LastName", Value = "Kunis" },
                    new Variable() { Name = "Fullname", Value = "${FirstName} ${LastName}" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${Fullname}!");

            Assert.Equal("Hello Mila Kunis!", result);
        }

        [Fact]
        public void Resolve_Variable_In_Variable_Test_Doppelpunkt()
        {
            var variableList = new List<Variable>()
                {
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${Fullname:11:00:00}!");

            Assert.Equal("Hello 11:00:00!", result);
        }

        [Fact]
        public void Resolve_Variable_In_Variable_In_Variable_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "A", Value = "A + ${B}" },
                    new Variable() { Name = "B", Value = "B + ${C}" },
                    new Variable() { Name = "C", Value = "C" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("${A}");

            Assert.Equal("A + B + C", result);
        }

        [Fact]
        public void Resolve_Not_Existing_Variable_Returns_Null()
        {
            var variableList = new List<Variable>()
                               {
                                   new Variable() {Name = "A", Value = "A"},
                               };

            var target = new VariableResolver(variableList);
            var result = target.Resolve("b");

            Assert.Null(result);
        }

        [Fact]
        public void Resolve_Variable_In_Variable_In_Variable_Directly_Test()
        {
            var variableList = new List<Variable>()
                               {
                                   new Variable() {Name = "a", Value = "a${b}" },
                                   new Variable() {Name = "b", Value = "b" }
                               };

            var target = new VariableResolver(variableList);
            var result = target.Resolve("a");

            Assert.Equal("ab", result);
        }

        [Fact]
        public void List_Missing_Variables_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName} ${LastName}!");

            Assert.Equal(1, target.VariableUsageList.Count(v => v.IsMissingValue));
            Assert.Equal("Hello Jack !!Missing variable for LastName!!!", result);
        }

        [Fact]
        public void List_Multiple_Missing_Variables_Test()
        {
            var target = new VariableResolver(new List<Variable>());

            target.TransformVariables("${a} ${b} ${c}");

            Assert.Equal(3, target.VariableUsageList.Count(v => v.IsMissingValue));
        }

        [Fact]
        public void Resolve_Default_Value_With_Variable_In_It_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${Fullname:$[FirstName] $[LastName]}!");

            Assert.Equal("Hello Jack Bauer!", result);
        }

        [Fact(Skip = "TODO")]
        public void Set_Subenv_Variable_To_Empty_String_If_Not_Provided()
        {
            var variableList = new List<Variable>()
                               {
                                   new Variable() { Name = "xxx", Value = "yyy" }
                               };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("${subenv}");

            Assert.Equal(string.Empty, result);
        }

        ////[Test]
        ////[TestCase("true")]
        ////[TestCase("TRUE")]
        ////[TestCase("on")]
        ////[TestCase("1")]
        ////[TestCase("enabled")]
        ////public void Conditional_Is_Visible_With_True(string condition)
        ////{
        ////    var target = new VariableResolver(new List<Variable>());
        ////    var result = target.TransformVariables("Hello\n<!--[if " + condition + "]-->\ntobi!\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello\r\ntobi!", result);
        ////}

        ////[Test]
        ////[TestCase("false")]
        ////[TestCase("FALSE")]
        ////[TestCase("off")]
        ////[TestCase("0")]
        ////[TestCase("disabled")]
        ////public void Conditional_Is_Not_Visible_With_True(string condition)
        ////{
        ////    var target = new VariableResolver(new List<Variable>());
        ////    var result = target.TransformVariables("Hello\n<!--[if not " + condition + "]-->\ntobi!\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello\r\ntobi!", result);
        ////}

        ////[Test]
        ////[TestCase("true")]
        ////[TestCase("TRUE")]
        ////[TestCase("on")]
        ////[TestCase("1")]
        ////[TestCase("enabled")]
        ////public void Conditional_Is_Visible_With_True_Variable(string condition)
        ////{
        ////    var variableList = new List<Variable>()
        ////    {
        ////        new Variable() {Name = "condition", Value = condition}
        ////    };

        ////    var target = new VariableResolver(variableList);
        ////    var result = target.TransformVariables("Hello\n<!--[if ${condition}]-->\ntobi!\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello\r\ntobi!", result);
        ////}

        ////[Test]
        ////[TestCase("false")]
        ////[TestCase("False")]
        ////[TestCase("FALSE")]
        ////[TestCase("off")]
        ////[TestCase("0")]
        ////[TestCase("disable")]
        ////public void Conditional_Is_Not_Visible_With_True_Variable(string condition)
        ////{
        ////    var variableList = new List<Variable>()
        ////    {
        ////        new Variable() {Name = "condition", Value = condition}
        ////    };

        ////    var target = new VariableResolver(variableList);
        ////    var result = target.TransformVariables("Hello\n<!--[if not ${condition}]-->\ntobi!\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello\r\ntobi!", result);
        ////}

        ////[Test]
        ////[TestCase("false")]
        ////[TestCase("FALSE")]
        ////[TestCase("off")]
        ////[TestCase("0")]
        ////[TestCase("disabled")]
        ////public void Conditional_Not_Visible_With_False(string condition)
        ////{
        ////    var target = new VariableResolver(new List<Variable>());
        ////    var result = target.TransformVariables("Hello\n<!--[if " + condition + "]-->\ntobi!\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello", result);
        ////}

        ////[Test]
        ////[TestCase("true")]
        ////[TestCase("TRUE")]
        ////[TestCase("on")]
        ////[TestCase("1")]
        ////[TestCase("enabled")]
        ////public void Conditional_Not_Visible_With_False_Negated(string condition)
        ////{
        ////    var target = new VariableResolver(new List<Variable>());
        ////    var result = target.TransformVariables("Hello\n<!--[if not " + condition + "]-->\ntobi!\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello", result);
        ////}

        ////[Test]
        ////[TestCase("false")]
        ////[TestCase("FALSE")]
        ////[TestCase("off")]
        ////[TestCase("0")]
        ////[TestCase("disabled")]
        ////public void Conditional_Is_Visible_With_False_Variable(string condition)
        ////{
        ////    var variableList = new List<Variable>()
        ////    {
        ////        new Variable() {Name = "condition", Value = condition}
        ////    };

        ////    var target = new VariableResolver(variableList);
        ////    var result = target.TransformVariables("Hello\r\n<!--[if ${condition}]-->\r\ntobi!\r\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello", result);
        ////}

        ////[Test]
        ////[TestCase("true")]
        ////[TestCase("TRUE")]
        ////[TestCase("on")]
        ////[TestCase("1")]
        ////[TestCase("enabled")]
        ////public void Conditional_Is_Visible_With_False_Variable_Negated(string condition)
        ////{
        ////    var variableList = new List<Variable>()
        ////    {
        ////        new Variable() {Name = "condition", Value = condition}
        ////    };

        ////    var target = new VariableResolver(variableList);
        ////    var result = target.TransformVariables("Hello\r\n<!--[if not ${condition}]-->\r\ntobi!\r\n<!--[endif]-->");

        ////    Assert.AreEqual("Hello", result);
        ////}

        ////[Test]
        ////[TestCase("true", "true", "content 1\r\ncontent 2")]
        ////[TestCase("true", "false", "content 1")]
        ////[TestCase("false", "false", "")]
        ////[TestCase("${true.condition}", "${true.condition}", "content 1\r\ncontent 2")]
        ////[TestCase("${false.condition}", "${false.condition}", "")]
        ////public void Multiple_Conditional_Transforming(string condition1, string condition2, string assert)
        ////{
        ////    var variableList = new List<Variable>()
        ////    {
        ////        new Variable() { Name = "true.condition", Value = "true" },
        ////        new Variable() { Name = "false.condition", Value = "false" },
        ////    };

        ////    var target = new VariableResolver(variableList);
            
        ////    var result = target.TransformVariables("<!-- [if " + condition1 + "] -->" + System.Environment.NewLine +
        ////                                           "content 1" + System.Environment.NewLine +
        ////                                           "<!-- [endif] -->" + System.Environment.NewLine +
        ////                                           "<!-- [if " + condition2 + "] -->" + System.Environment.NewLine +
        ////                                           "content 2" + System.Environment.NewLine +
        ////                                           "<!-- [endif] -->");

        ////    Assert.AreEqual(assert, result);
        ////}

        ////[Fact]
        ////public void Dont_Resolve_Escaped_Variable()
        ////{
        ////    var target = new VariableResolver(new List<Variable>());
        ////    var result = target.TransformVariables("Hello _$_{Escaped}!");

        ////    Assert.Equal("Hello ${Escaped}!", result);
        ////}

        ////[Test]
        ////[Ignore] // TODO: add warning also for default values
        ////public void Resolve_Default_Value_With_Missing_Variable_Test()
        ////{
        ////    var variableList = new List<Variable>()
        ////        {
        ////            new Variable() { Name = "FirstName", Value = "Jack" }
        ////        };

        ////    var target = new VariableResolver(variableList);
        ////    var result = target.TransformVariables("Hello ${FullName=$[gugus]}");

        ////    Assert.AreEqual(1, target.VariableUsageList.Count(vu => vu.IsMissingValue));
        ////}
    }
}