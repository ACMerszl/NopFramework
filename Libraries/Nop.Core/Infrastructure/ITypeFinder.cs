using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface provide information about types 
    /// to various services in the Nop engine.
    /// 实现此接口的类提供有关类型的信息
    /// 到Nop引擎中的各种服务。
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// 获得程序集
        /// </summary>
        /// <returns></returns>
        IList<Assembly> GetAssemblies();

        /// <summary>
        /// 查找类型的类
        /// </summary>
        /// <param name="assignTypeFrom">分配类型</param>
        /// <param name="onlyConcreteClasses">只有具体类</param>
        /// <returns></returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// 查找类型的类
        /// </summary>
        /// <param name="assignTypeFrom">分配类型</param>
        /// <param name="assemblies">程序集</param>
        /// <param name="onlyConcreteClasses">只有具体类</param>
        /// <returns></returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
        /// <summary>
        /// 查找类型的类
        /// </summary>
        /// <typeparam name="T">指定范类</typeparam>
        /// <param name="onlyConcreteClasses">只有具体类</param>
        /// <returns></returns>
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);
        /// <summary>
        /// 查找类型的类
        /// </summary>
        /// <typeparam name="T">指定范类</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="onlyConcreteClasses">只有具体类</param>
        /// <returns></returns>
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }
}
