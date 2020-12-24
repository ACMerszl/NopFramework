using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection; 
using Nop.Data.Mapping;
using Nop.Domain;
namespace Nop.Data
{
    /// <summary>
    /// Object context
    /// ����������
    /// </summary>
    public class NopObjectContext : DbContext, IDbContext
    {
        #region CtorCtor���캯��

        public NopObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }
        
        #endregion

        #region Utilities

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //������ص㣬EF��Ĭ��ʹ��"dbo"����������ݿ⣬���Mysql��Oracle����
            //https://blog.csdn.net/u010293395/article/details/89164985
            modelBuilder.HasDefaultSchema(null);

            //dynamically load all configuration
            //��̬������������
            //System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(NopEntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                //��ģ��ʵ��������ӵ�DbModelBuilder��
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,�����������ֶ�����������,
            //modelBuilder.Configurations.Add(new LanguageMap());



            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// ��һ��ʵ�帽�ӵ������ģ����߷���һ���Ѿ����ӵ�ʵ��(����Ѿ�������)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            //����ʵ��������֧�ִ洢���̣����򣬼���ʵ��ĵ�������ֱ��ʵ�帽�ӵ�������ʱ�Ż����
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                //attach new entity������ʵ��
                Set<TEntity>().Attach(entity);
                return entity;
            }

            //entity is already loadedʵ���Ѿ�����
            return alreadyAttached;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create database script�������ݿ�ű�
        /// </summary>
        /// <returns>SQL to generate database��SQL�������ݿ�</returns>
        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// Execute��ִ�У��洢���̲���������ʵ���б�
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text�����ı�</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entitiesʵ�弯</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //add parameters to command��������Ӳ���
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter�������
                        commandText += " output";
                    }
                }
            }

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            //performance hack applied as described here����hackӦ������������ - http://www.nopcommerce.com/boards/t/25483/fix-very-important-speed-improvement.aspx
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// ����һ��ԭʼSQL��ѯ�����ظ����������͵�Ԫ�ء�
        /// �����Ϳ����Ǿ������ѯ���ص���������ƥ������Ե��κ����ͣ�
        /// Ҳ�����Ǽ򵥵Ļ�Ԫ���͡������Ͳ�����ʵ�����͡���ʹ���صĶ���������ʵ�����ͣ�
        /// �ò�ѯ�Ľ��Ҳ�����������ĸ��١�
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.��ѯ���صĶ�������͡�</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.Ӧ����SQL��ѯ�ַ����Ĳ�����</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// �����ݿ�ִ�и�����DDL/DML���
        /// DDL���ݿⶨ�����ԣ������Ͳ�����ṹ
        /// DML���ݲ������ԣ�"ѡ��"��"����"��"����" �� "ɾ��" 
        /// </summary>
        /// <param name="sql">The command string�����ַ���</param>
        /// <param name="doNotEnsureTransaction">false - the transaction creation is not ensured; true - the transaction creation is ensured.
        /// false����ȷ������Ĵ���;true��ȷ�����񴴽���</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used
        /// ��ʱֵ������Ϊ��λ����ֵ��ʾ��ʹ�û����ṩ�����Ĭ��ֵ</param>
        /// <param name="parameters">The parameters to apply to the command string.
        /// Ӧ���������ַ����Ĳ���</param>
        /// <returns>The result returned by the database after executing the command.
        /// ���ݿ�ִ������󷵻صĽ����</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                //����֮ǰ��ʱ
                previousTimeout = ((IObjectContextAdapter) this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = timeout;
            }

            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                //������ǰ�ĳ�ʱ
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }

        /// <summary>
        /// Detach an entity����һ��ʵ��
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// ��ȡ������һ��ֵ����ֵָʾ�Ƿ����ô���������(��EF��ʹ��)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// ��ȡ������һ��ֵ����ֵָʾ�Ƿ������Զ�����������(��EF��ʹ��)��
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        #endregion
    }
}