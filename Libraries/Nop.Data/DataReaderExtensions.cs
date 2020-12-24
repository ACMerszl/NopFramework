//Contributor: Rick Strahl - http://codepaste.net/qqcf4x

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Nop.Data
{   /// <summary>
    /// 数据读取器扩展
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Creates a list of a given type from all the rows in a DataReader.
        /// 
        /// Note this method uses Reflection so this isn't a high performance
        /// operation, but it can be useful for generic data reader to entity
        /// conversions on the fly and with anonymous types.
        /// 从DataReader中的所有行创建给定类型的列表。
        ///注意，此方法使用反射，因此这不是一个高性能操作，
        ///但它对于动态进行实体转换的通用数据读取器和匿名类型非常有用。
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="reader">An open DataReader that's in position to read
        /// 一个处于可读取位置的开放数据阅读器</param>
        /// <param name="fieldsToSkip">Optional - comma delimited list of fields that you don't want to update
        ///可选操作-—您不想更新的用逗号分隔的字段列表 </param>
        /// <param name="piList">
        /// Optional - Cached PropertyInfo dictionary that holds property info data for this object.
        /// Can be used for caching hte PropertyInfo structure for multiple operations to speed up
        /// translation. If not passed automatically created.
        /// 可选-缓存的PropertyInfo字典，保存此对象的属性信息数据。
        ///可以用于缓存hte PropertyInfo结构的多个操作，以加快翻译速度。如果没有通过，自动创建。
        /// </param>
        /// <returns></returns>
        public static List<TType> DataReaderToObjectList<TType>(this IDataReader reader, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
            where TType : new()
        {
            if (reader == null)
                return null;

            var items = new List<TType>();

            // Create lookup list of property info objects  
            //创建属性信息对象的查找列表
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = typeof(TType).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            while (reader.Read())
            {
                var inst = new TType();
                DataReaderToObject(reader, inst, fieldsToSkip, piList);
                items.Add(inst);
            }

            return items;
        }

        /// <summary>
        /// Populates the properties of an object from a single DataReader row using
        /// Reflection by matching the DataReader fields to a public property on
        /// the object passed in. Unmatched properties are left unchanged.
        /// 
        /// You need to pass in a data reader located on the active row you want
        /// to serialize.
        /// 
        /// This routine works best for matching pure data entities and should
        /// be used only in low volume environments where retrieval speed is not
        /// critical due to its use of Reflection.
        /// 通过将DataReader字段与传入的对象上的公共属性匹配，使用反射从单个DataReader行填充对象的属性。未匹配的属性保持不变。
        ///您需要传入一个位于要序列化的活动行上的数据读取器。
        ///这个例程最适合于匹配纯数据实体
        ///仅在低容量的环境中使用，在这些环境中，由于使用反射，检索速度不是很关键。
        /// </summary>
        /// <param name="reader">Instance of the DataReader to read data from. Should be located on the correct record (Read() should have been called on it before calling this method)
        /// 要从中读取数据的DataReader的实例。应该位于正确的记录上(Read()应该在调用此方法之前被调用)</param>
        /// <param name="instance">Instance of the object to populate properties on
        /// 要在上面填充属性的对象的实例</param>
        /// <param name="fieldsToSkip">Optional - A comma delimited list of object properties that should not be updated
        /// 可选-不应更新的对象属性的逗号分隔列表</param>
        /// <param name="piList">Optional - Cached PropertyInfo dictionary that holds property info data for this object
        /// 可选-缓存的PropertyInfo字典，保存此对象的属性信息数据</param>
        public static void DataReaderToObject(this IDataReader reader, object instance, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("Data reader cannot be used because it's already closed");

            if (string.IsNullOrEmpty(fieldsToSkip))
                fieldsToSkip = string.Empty;
            else
                fieldsToSkip = "," + fieldsToSkip + ",";

            fieldsToSkip = fieldsToSkip.ToLower();

            // create a dictionary of properties to look up
            // we can pass this in so we can cache the list once 
            // for a list operation 
            // 创建一个要查找的属性字典
            //我们可以传递这个，这样我们就可以缓存一次列表操作的列表
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            for (int index = 0; index < reader.FieldCount; index++)
            {
                string name = reader.GetName(index).ToLower();
                if (piList.ContainsKey(name))
                {
                    var prop = piList[name];

                    if (fieldsToSkip.Contains("," + name + ","))
                        continue;

                    if ((prop != null) && prop.CanWrite)
                    {
                        var val = reader.GetValue(index);
                        prop.SetValue(instance, (val == DBNull.Value) ? null : val, null);
                    }
                }
            }
        }
    }
}
