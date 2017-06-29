﻿using Cake.Core;
using Cake.Core.Annotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cake.Intellisense.Core
{
    public class CakeFileIntellisenseGenerator
    {
        public static readonly string ThrowNotSupportedExceptionArrowExpression = " => throw new System.NotSupportedException();";

        protected virtual string CakeFileIntellisensePath => $"{nameof(CakeFileIntellisense)}.cs";

        public void GenereteIntellisense(string dllsDirectory)
        {
            var assemblies = Directory.GetFiles(dllsDirectory, "**.dll")
                .Select(Assembly.LoadFile)
                .ToArray();
            var classBuilder = new StringBuilder();

            AppendAutoGeneratedHeader(classBuilder);

            foreach (var assembly in assemblies)
            {
                AppendAssemblyAliases(classBuilder, assembly);
            }

            classBuilder
                .AppendLine("\t}")
                .AppendLine("}");

            File.WriteAllText(CakeFileIntellisensePath, classBuilder.ToString());
        }

        protected virtual void AppendAssemblyAliases(StringBuilder builder, Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes.Where(type => type.IsStatic()))
            {
                AppendTypeAliases(builder, type);
            }
        }
        protected virtual void AppendTypeAliases(StringBuilder builder, TypeInfo type)
        {
            var aliases = Utilities.GetCakeAliases(type);
            if (!aliases.Any()) return;

            builder.AppendLine($"#region {type.Name}");
            foreach (var alias in aliases)
            {
                switch (alias.GetCustomAttribute<CakeAliasAttribute>(inherit: true))
                {
                    case CakeMethodAliasAttribute method:
                        AppendMethodSignature(builder, alias);
                        break;
                    case CakePropertyAliasAttribute property:
                        AppendPropertySignature(builder, alias);
                        break;
                    default:
                        break;
                }
            }
            builder.AppendLine("#endregion");
        }
        protected virtual void AppendMethodSignature(StringBuilder builder, MethodInfo alias)
        {
            builder.Append("\t\tprotected ");

            if (alias.ReturnType == typeof(void)) builder.Append($"void ");
            else if (alias.ReturnType.IsGenericParameter) builder.Append($"{alias.ReturnType} ");
            else builder.Append($"{Utilities.GetTypeRepresentation(alias.ReturnType)} ");

            builder.Append(alias.Name);
            if (alias.IsGenericMethod) builder.Append("<T>");

            builder
                .Append("(")
                .Append(string.Join(", ", alias.GetParameters().Skip(1)
                    .Select(parameter => $"{Utilities.GetParameterRepresentation(parameter)} {parameter.Name}")))
                .Append(")")
                .Append(ThrowNotSupportedExceptionArrowExpression)
                .AppendLine();
        }
        protected virtual void AppendPropertySignature(StringBuilder builder, MethodInfo alias)
        {
            builder
                .Append("\t\tprotected ")
                .Append(Utilities.GetTypeRepresentation(alias.ReturnType))
                .Append(" ")
                .Append(alias.Name)
                .Append(ThrowNotSupportedExceptionArrowExpression)
                .AppendLine();
        }

        protected virtual void AppendAutoGeneratedHeader(StringBuilder builder)
        {
            builder.AppendLine("//------------------------------------------------------------------------------")
   .AppendLine("// <auto-generated>")
   .AppendLine("//     This code was generated by a tool.")
   .AppendLine("//")
   .AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if")
   .AppendLine("//     the code is regenerated. ")
   .AppendLine("// </auto-generated>")
   .AppendLine("//------------------------------------------------------------------------------")
   .AppendLine()
   .AppendLine($"namespace {typeof(CakeFile).Namespace}")
   .AppendLine("{")
   .AppendLine($"\tpublic abstract partial class {nameof(CakeFileIntellisense)} : {nameof(CakeFile)}")
   .AppendLine("\t{");
        }
    }
}