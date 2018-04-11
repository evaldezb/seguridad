using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace seguridad.Attributes
{
    /// <summary>
    ///     Objeto Cache a Utilizar con los metodos del Facade.
    /// </summary>
    [Serializable]
    [PSerializable]
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CacheMethodAttribute : OnMethodBoundaryAspect
    {
        public CacheMethodAttribute(int cacheDuration = 10)
        {
            _cacheDuration = cacheDuration;
            
             

        }

        // This field will be set by CompileTimeInitialize and serialized at build time,  
        // then deserialized at runtime. 
        private string _methodName;
        //
        private  int _cacheDuration = 10;// Minutes

        // Method executed at build time. 
        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            this._methodName = method.DeclaringType.FullName + "." + method.Name;
        }

        private string GetCacheKey(object instance, Arguments arguments)
        {
            // If we have no argument, return just the method name so we don't uselessly allocate memory. 
            if (instance == null && arguments.Count == 0)
                return this._methodName;

            // Add all arguments to the cache key. Note that generic arguments are not part of the cache 
            // key, so method calls that differ only by generic arguments will have conflicting cache keys.
            StringBuilder stringBuilder = new StringBuilder(this._methodName);
            stringBuilder.Append('(');
            if (instance != null)
            {
                stringBuilder.Append(instance);
                stringBuilder.Append("; ");
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                stringBuilder.Append(arguments.GetArgument(i) ?? "null");
                stringBuilder.Append(", ");
            }

            return stringBuilder.ToString();
        }


         
        public IDistributedCache Cache { get; set; }

        //private static readonly ObjectCache Cache = MemoryCache.Default;

        /// <summary>
        ///     Cuando se Inicia Metodo.
        /// </summary>
        /// <param name="args"></param>
        public override void OnEntry(MethodExecutionArgs args)
        {
            string key = GetCacheKey(args.Instance, args.Arguments); // CacheHelper.GenerateKey(args.Method.Name, args.Arguments);
            //
            object result = Cache.Get(key);

            if (result != null)
            {
                var cacheNullValue = result as CacheNullValue;
                args.ReturnValue = cacheNullValue != null ? null : result;
                args.FlowBehavior = FlowBehavior.Return;
            }
        }

        /// <summary>
        ///     Cuando Se Abandona Metodo
        /// </summary>
        /// <param name="args"></param>
        public override void OnExit(MethodExecutionArgs args)
        {
            string key = GetCacheKey(args.Instance, args.Arguments);

            if (args.Exception == null)
            {             

                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, args.ReturnValue ?? new CacheNullValue());

                    Cache.Set(key,ms.ToArray() ,new DistributedCacheEntryOptions() {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_cacheDuration)});
                } 
            }
        }

        /// <summary>
        ///     En Caso de Error
        /// </summary>
        /// <param name="args"></param>
        public override void OnException(MethodExecutionArgs args)
        {
            base.OnException(args);
            args.FlowBehavior = FlowBehavior.ThrowException;
        }
    }

    public class CacheNullValue
    {
        //MSDN : The MemoryCache class does not allow null 
        //  Por eso se agrego este clase para sustituirlo con valores nulos en la cache
    }
}
