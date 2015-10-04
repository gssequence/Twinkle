using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class Evaluator
    {
        private static readonly string sourceHeader = @"
            using CoreTweet;
            using System;
            using System.Linq;
            using Twinkle.Models;
            
            public class ScriptingClass
            {
                public static bool Eval(Tweet tweet)
                {
                    try
                    {
                        return (";

        private static readonly string sourceFooter = @");
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        ";

        private Assembly _assembly;

        public Evaluator()
        {

        }
        
        public bool Compile(string script)
        {
            using (var provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } }))
            {
                var cp = new CompilerParameters();
                cp.GenerateInMemory = true;
                cp.ReferencedAssemblies.Add("System.dll");
                cp.ReferencedAssemblies.Add("System.Core.dll");
                cp.ReferencedAssemblies.Add("System.Linq.dll");
                cp.ReferencedAssemblies.Add("CoreTweet.dll");
                cp.ReferencedAssemblies.Add("Livet.dll");
                cp.ReferencedAssemblies.Add("Twinkle.exe");
                var source = sourceHeader + script + sourceFooter;
                var cr = provider.CompileAssemblyFromSource(cp, source);
                if (cr.Errors.Count > 0)
                {
                    _assembly = null;
                    return false;
                }
                _assembly = cr.CompiledAssembly;
            }
            return true;
        }

        public bool Eval(Tweet tweet)
        {
            if (_assembly == null) return false;
            var type = _assembly.GetType("ScriptingClass");
            return (bool)type.InvokeMember("Eval", BindingFlags.InvokeMethod, null, null, new object[] { tweet });
        }
    }
}
